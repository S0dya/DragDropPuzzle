using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Tools.Helper;
using PT.Tools.Windows;
using PT.UI.Windows;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Logic.PersistentScene
{
    public class LoadingWindowManager : MonoBehaviour
    {
        [Inject] private LoadingManager _loadingManager;
        [Inject(Id = "Persistent")] private WindowsManager _windowsManager;

        private LoadingWindow _loadingWindow;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _loadingManager.OnLoadStarted += ShowLoad;
            _loadingManager.OnLoadFinished += HideLoadingWindow;
        }
        
        public async UniTask ShowFakeLoadAsync(float duration = 1)
        {
            _cts?.Cancel();
            _cts = new();
            var token = _cts.Token;

            await OpenLoadingWindow();

            float stepLength = duration / 4;
            float timer = 0;
            while (timer < duration)
            {
                token.ThrowIfCancellationRequested();

                var step = Utils.GetRandomNext(stepLength);
                if (duration - timer < step) step = duration - timer;
                
                await UniTask.Delay(TimeSpan.FromSeconds(step), cancellationToken: token);
                timer += step;
                
                _loadingWindow.SetProgress(Mathf.Clamp01(timer / duration));
            }

            _loadingWindow.SetProgress(1f);
            await UniTask.Delay(300, cancellationToken: token);
            
            HideLoadingWindow();
        }

        private void ShowLoad() => ShowLoadAsync().Forget();
        private async UniTask ShowLoadAsync()
        {
            _cts?.Cancel();
            _cts = new();

            await OpenLoadingWindow();

            _loadingManager.LoadProgress
                .Subscribe(p => _loadingWindow.SetProgress(p))
                .AddTo(_loadingWindow); 
        }
        
        private async UniTask OpenLoadingWindow()
        {
            var window = await _windowsManager.Open<LoadingWindow>();
            _loadingWindow = window as LoadingWindow;
        }
        private void HideLoadingWindow()
        {
            _windowsManager.Close<LoadingWindow>().Forget();
            
            _cts?.Cancel(); _cts = null;
        }

        private void OnDestroy()
        {
            _loadingManager.OnLoadStarted -= ShowLoad;
            _loadingManager.OnLoadFinished -= HideLoadingWindow;
            
            _cts?.Cancel();
        }
    }
}