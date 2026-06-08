using System;
using Cysharp.Threading.Tasks;
using PT.Logic.Configs;
using PT.Tools.Debugging;
using Zenject;

namespace PT.Tools.Addressables
{
    public class AddressablesDiagnostics : IInitializable, IDisposable
    {
        [Inject] private GameConfig _gameConfig;
        [Inject] private AddressablesAssetsService _assetsService;

        private bool _running;

        public void Initialize()
        {
            _running = true;
            LogLoop().Forget();
        }
        public void Dispose()
        {
            _running = false;
        }

        private async UniTaskVoid LogLoop()
        {
            while (_running)
            {
                LogState();
                await UniTask.Delay(TimeSpan.FromSeconds(_gameConfig.AddressablesDiagnosticsInterval));
            }
        }

        private void LogState()
        {
            var handles = _assetsService.GetLoadedHandles();

            DebugManager.Log(DebugCategory.Addressables, $"Loaded assets count: {handles.Count}");

            foreach (var pair in handles)
            {
                DebugManager.Log(DebugCategory.Addressables, $" - {pair.Key} | Status: {pair.Value.Status}");
            }
        }
    }
}