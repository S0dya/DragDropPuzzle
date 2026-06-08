using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PT.Logic.Save;
using PT.Tools.Debugging;
using PT.Tools.Tutorials.Configs;
using PT.Tools.Tutorials.Target;
using PT.Tools.Tutorials.UI;
using UnityEngine;
using Zenject;

namespace PT.Tools.Tutorials
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialUIController ui;

        [Inject] private TutorialsSequencesConfig _tutorialsConfig;
        
        private readonly Dictionary<string, UniTaskCompletionSource> _signalMap = new();
        private readonly List<TutorialTargetsRegistry> _targetsRegistries = new();
        
        private static readonly Dictionary<TutorialSequenceEnum, GameDataKey> _map = new()
        {
            // { TutorialSequenceEnum.Drop, GameDataKey.TutorialDrop },
        };

        public async UniTask StartTutorial(TutorialSequenceEnum tutorialEnum, bool skipIfCompleted = true)
        {
            var seq = _tutorialsConfig.TutorialConfigs.First(x => x.SequenceEnum == tutorialEnum);
            
            if (seq == null)
            {
                DebugManager.Log(DebugCategory.Tutorial, $"No tutorial sequence found for {tutorialEnum}", LogType.Warning);
                
                return;
            }
            if (skipIfCompleted && (bool)GameDataRegistry.Get(_map[seq.SequenceEnum])) return;
        
            DebugManager.Log(DebugCategory.Tutorial, $"Starting tutorial {seq.SequenceEnum.ToString()}");
            
            foreach (var step in seq.Steps)
            {
                await RunStep(step);
            }
            
            DebugManager.Log(DebugCategory.Tutorial, $"Finished tutorial {seq.SequenceEnum.ToString()}");
            
            GameDataRegistry.Set(_map[seq.SequenceEnum], true);
        }
        
        public void AddTargetsRegistry(TutorialTargetsRegistry registry)
        {
            if (!_targetsRegistries.Contains(registry))
                _targetsRegistries.Add(registry);
        }

        public void RemoveTargetsRegistry(TutorialTargetsRegistry registry)
        {
            _targetsRegistries.Remove(registry);
        }

        private async UniTask RunStep(TutorialStepData step)
        {
            if (step.LockInput) ui.BlockInput();

            ui.HideAllUI();
            
            switch (step.StepType)
            {
                case TutorialStepType.ShowScreen:
                    ui.ShowText(step.Title, step.Description);
                    await WaitForAnyTap();
                    break;

                case TutorialStepType.TapTarget:
                    ui.ShowText(step.Title, step.Description);
                    await RunTapTargetStep(step);
                    break;

                case TutorialStepType.WaitSeconds:
                    await UniTask.Delay(TimeSpan.FromSeconds(step.Duration));
                    break;
            }

            ui.HideAllUI();
            ui.UnblockInput(); 

            await UniTask.Yield();
        }
        
        private async UniTask WaitForAnyTap()
        {
            var tcs = new UniTaskCompletionSource();

            while (!tcs.Task.Status.IsCompleted())
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    tcs.TrySetResult();
                    break;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    tcs.TrySetResult();
                    break;
                }

                await UniTask.Yield();
            }

            await tcs.Task;
        }
        private async UniTask RunTapTargetStep(TutorialStepData step)
        {
            var target = GetTarget(step);
            if (target == null) return;

            target.Show();

            ui.HighlightAt(target.GetScreenPosition());

            if (step.ShowPointer)
            {
                var pos = target.GetScreenPosition();
                ui.ShowPointer(pos + step.PointerOffset); 
            }

            bool tapped = await WaitForTapOnTarget(target);
            if (tapped)
                target.InvokeAction();
        }
        private TutorialTargetBase GetTarget(TutorialStepData step)
        {
            foreach (var registry in _targetsRegistries)
            {
                if (registry.Resolve(step.TutorialTargetEnum, out TutorialTargetBase target))
                    return target;
            }

            DebugManager.Log(DebugCategory.Tutorial, $"Could not resolve target for {step.TutorialTargetEnum}", LogType.Warning);
            return null;
        }
        private async UniTask<bool> WaitForTapOnTarget(TutorialTargetBase target)
        {
            var tcs = new UniTaskCompletionSource<bool>();

            while (!tcs.Task.Status.IsCompleted())
            {
                Vector2? inputPos = null;

                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    inputPos = Input.GetTouch(0).position;
                else if (Input.GetMouseButtonDown(0))
                    inputPos = Input.mousePosition;

                if (inputPos.HasValue)
                {
                    if (target.IsHit(inputPos.Value))
                    {
                        tcs.TrySetResult(true);
                        break;
                    }
                    else
                    {
                        ui.PulseInvalid();
                    }
                }

                await UniTask.Yield();
            }

            return await tcs.Task;
        }
    }
}