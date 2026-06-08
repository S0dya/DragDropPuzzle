using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Logic.PersistentScene;
using Zenject;

namespace PT.Tools.Addressables
{
    public interface IAssetsSceneLoader
    {
        UniTask Load(SceneNameEnum scene, CancellationToken token, IProgress<float> progress = null);
    }
    
    public class AddressablesSceneLoader : IAssetsSceneLoader
    {
        [Inject] private AssetsConfig _assetsConfig; 
        [Inject] private IAssetProvider _assetProvider; 
        
        public async UniTask Load(SceneNameEnum sceneName, CancellationToken token, IProgress<float> progress = null)
        {
            if (!_assetsConfig.SceneAssets.Dictionary.TryGetValue(sceneName, out var keys)) return;

            int loaded = 0;

            foreach (var key in keys)
            {
                token.ThrowIfCancellationRequested();
                await _assetProvider.Load<UnityEngine.Object>(key);
                loaded++;
                
                if (progress != null) progress.Report(loaded / (float)keys.Length);
            }
        }
    }
}