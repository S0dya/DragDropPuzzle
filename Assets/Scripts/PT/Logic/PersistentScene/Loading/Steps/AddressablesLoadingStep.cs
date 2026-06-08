using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Tools.Addressables;
using Object = UnityEngine.Object;

namespace PT.Logic.PersistentScene.Loading.Steps
{
    public class AddressablesLoadingStep : ILoadingStep
    {
        private readonly IAssetProvider _assetProvider;
        private readonly AssetKey[] _keys;
        
        public AddressablesLoadingStep(IAssetProvider assetProvider, AssetKey[] keys)
        {
            _assetProvider = assetProvider;
            _keys = keys;
        }
        
        public async UniTask Load(CancellationToken token)
        {
            foreach (var key in _keys)
            {
                token.ThrowIfCancellationRequested();

                await _assetProvider.Load<Object>(key);
                // await _assetProvider.Load<Object>(key, token);
            }
        }
    }
}