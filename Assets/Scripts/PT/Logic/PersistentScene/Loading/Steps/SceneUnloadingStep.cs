using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Logic.Dependency.Signals;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace PT.Logic.PersistentScene.Loading.Steps
{
    public class SceneUnloadingStep : ILoadingStep
    {
        private readonly SceneNameEnum _sceneName;
        private readonly ReactiveProperty<float> _progress;
        private readonly SignalBus _signalBus;

        public SceneUnloadingStep(SceneNameEnum sceneName, ReactiveProperty<float> progress, SignalBus signalBus)
        {
            _sceneName = sceneName;
            _progress = progress;
            _signalBus = signalBus;
        }

        public async UniTask Load(CancellationToken token)
        {
            _signalBus.Fire(new SceneUnloadedSignal(_sceneName));
            
            var op = SceneManager.UnloadSceneAsync((int)_sceneName);

            while (!op.isDone)
            {
                token.ThrowIfCancellationRequested();
                _progress.Value = op.progress * 0.9f;
                await UniTask.Yield(token);
            }

            _progress.Value = 0.9f;
        }
    }
}