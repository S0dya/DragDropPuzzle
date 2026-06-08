using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Tools.Addressables;
using UniRx;
using UnityEngine.SceneManagement;

namespace PT.Logic.PersistentScene.Loading.Steps
{
    public class SceneLoadingStep : ILoadingStep
    {
        private readonly SceneNameEnum _sceneName;
        private readonly ReactiveProperty<float> _progress;
        private readonly IAssetsSceneLoader _assetsSceneLoader;

        public SceneLoadingStep(SceneNameEnum sceneName, ReactiveProperty<float> progress, IAssetsSceneLoader assetsSceneLoader = null)
        {
            _sceneName = sceneName;
            _progress = progress;
            _assetsSceneLoader = assetsSceneLoader;
        }

        public async UniTask Load(CancellationToken token)
        {
            var sceneOp = SceneManager.LoadSceneAsync((int)_sceneName, LoadSceneMode.Additive);
            sceneOp.allowSceneActivation = true;

            while (!sceneOp.isDone)
            {
                token.ThrowIfCancellationRequested();
                _progress.Value = sceneOp.progress * 0.5f;
                await UniTask.Yield(token);
            }

            if (_assetsSceneLoader != null)
            {
                await _assetsSceneLoader.Load(_sceneName, token,
                    new Progress<float>(p => _progress.Value = 0.5f + p * 0.5f));
            }

            _progress.Value = 1f;
        }
    }
}