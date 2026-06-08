using System;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using Zenject;

namespace PT.Tools.Addressables
{
    public class SceneUnloadReleaser : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private AssetsConfig _AssetsConfig;
        [Inject] private IAssetProvider _assetProvider;

        public void Initialize()
        {
            _signalBus.Subscribe<SceneUnloadedSignal>(OnSceneUnloaded);
        }
        public void Dispose()
        {
            _signalBus.Unsubscribe<SceneUnloadedSignal>(OnSceneUnloaded);
        }

        private void OnSceneUnloaded(SceneUnloadedSignal signal)
        {
            if (!_AssetsConfig.SceneAssets.Dictionary.TryGetValue(signal.SceneName, out var keys))
            {
                DebugManager.Log(DebugCategory.Addressables, $"No AssetsConfig for scene {signal.SceneName}");
                return;
            }
            
            foreach (var key in keys)
            {
                _assetProvider.Release(key);
            }
        }
    }
}