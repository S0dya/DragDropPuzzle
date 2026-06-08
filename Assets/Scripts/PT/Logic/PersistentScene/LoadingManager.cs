using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Logic.PersistentScene.Loading;
using PT.Logic.PersistentScene.Loading.Steps;
using PT.Tools.Addressables;
using PT.Tools.Debugging;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PT.Logic.PersistentScene
{
    public enum SceneNameEnum
    {
        Menu = 1, 
        Game = 2,
    }

    public class LoadingManager : MonoBehaviour
    {
        public event Action OnLoadStarted;
        public event Action OnLoadFinished;
        
        public ReactiveProperty<float> LoadProgress { get; } = new();

        [Inject] private SignalBus _signalBus;
        [Inject (Optional = true)] private IAssetsSceneLoader _assetsSceneLoader;

        private CancellationTokenSource _loadCts;
        
        public async UniTask LoadSteps(List<ILoadingStep> steps, Action onDone = null)
        {
            CancelPreviousLoading();
            _loadCts = new();
            var token = _loadCts.Token;

            try
            {
                DebugManager.Log(DebugCategory.Loading, $"Starting loading process with {steps.Count} steps...");

                OnLoadStarted?.Invoke();

                int index = 0;

                foreach (var step in steps)
                {
                    index++;
                    DebugManager.Log(DebugCategory.Loading, $"Step {index}/{steps.Count} - {step.GetType().Name} started...");

                    await step.Load(token);

                    DebugManager.Log(DebugCategory.Loading, $"Step {index}/{steps.Count} - {step.GetType().Name} finished.");
                }

                LoadProgress.Value = 1f;

                await UniTask.Delay(300, cancellationToken: token);

                onDone?.Invoke();
            }
            catch (OperationCanceledException)
            {
                DebugManager.Log(DebugCategory.Loading, "Loading process canceled", LogType.Warning);
            }
            catch (Exception e)
            {
                DebugManager.Log(DebugCategory.Errors, $"Scene loading error: {e}", LogType.Error);
            }

            OnLoadFinished?.Invoke();
        }

        public void UnloadScene(SceneNameEnum sceneName)
        {
            DebugManager.Log(DebugCategory.Loading, $"Unloading scene: {sceneName}");
            SceneManager.UnloadSceneAsync((int)sceneName);
        }

        public SceneLoadingStep GetSceneLoadingStep(SceneNameEnum sceneName)
        {
            DebugManager.Log(DebugCategory.Loading, $"Creating SceneLoadingStep for: {sceneName}");
            return new SceneLoadingStep(sceneName, LoadProgress, _assetsSceneLoader);
        }
        public SceneUnloadingStep GetSceneUnloadingStep(SceneNameEnum sceneName)
        {
            DebugManager.Log(DebugCategory.Loading, $"Creating SceneUnloadingStep for: {sceneName}");
            return new SceneUnloadingStep(sceneName, LoadProgress, _signalBus);
        }

        private void CancelPreviousLoading()
        {
            if (_loadCts != null)
            {
                DebugManager.Log(DebugCategory.Loading, "Canceling previous loading process...");
                _loadCts.Cancel();
                _loadCts.Dispose();
                _loadCts = null;
            }
        }

        private void OnDestroy()
        {
            CancelPreviousLoading();
        }
    }
}
