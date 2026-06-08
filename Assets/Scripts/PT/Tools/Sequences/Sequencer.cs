using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Tools.Debugging;
using UnityEngine;

namespace PT.Tools.Sequences
{
    public class Sequencer : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private SequenceStep[] steps; 
        
        public event Action<Sequencer> OnSequenceStarted;
        public event Action<Sequencer> OnSequenceFinished;

        private CancellationTokenSource _cts;

        private SequenceInfo _currentInfo;
        private int _currentStepIndex = -1;

        private void OnDisable()
        {
            Cancel();
        }
        
        public async UniTask Play() => await Play(gameObject);
        public async UniTask Play(GameObject target)
        {
            DebugManager.Log(DebugCategory.Sequence, $"Starting Sequence on {target.name}");
            
            Cancel();
            _cts = new ();

            _currentInfo = new SequenceInfo(target.transform);

            try
            {
                OnSequenceStarted?.Invoke(this);

                for (_currentStepIndex = 0; _currentStepIndex < steps.Length; _currentStepIndex++)
                {
                    var step = steps[_currentStepIndex];
                    if (step == null) continue;
                    
                    if (_cts.IsCancellationRequested) break;

                    await step.PlayStep(_currentInfo, _cts.Token);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _currentStepIndex = -1;
             
                DebugManager.Log(DebugCategory.Sequence, $"Finishing Sequence");
                
                OnSequenceFinished?.Invoke(this);
            }
        }

        public void Skip()
        {
            if (steps == null || _currentInfo == null)
            {
                DebugManager.Log(DebugCategory.Sequence, $"Sequence cant be skipped {_currentInfo.TargetTransform.name}");
                return;
            }
            
            DebugManager.Log(DebugCategory.Sequence, $"Skipping Sequence on {_currentInfo.TargetTransform.name}");
            
            Cancel();
            
            if (_currentStepIndex >= 0 && _currentStepIndex < steps.Length)
            {
                steps[_currentStepIndex]?.SkipStep(_currentInfo);
            }
            
            for (int i = _currentStepIndex + 1; i < steps.Length; i++)
            {
                steps[i]?.SkipStep(_currentInfo);
            }
        }

        private void Cancel()
        {
            _cts?.Cancel();
        }
    }
}