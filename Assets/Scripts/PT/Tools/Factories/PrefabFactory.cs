using PT.Tools.Addressables;
using UnityEngine;
using Zenject;

namespace PT.Tools.Factories
{
    public sealed class PrefabFactory
    {
        [Inject] private IAssetResolver _assetResolver;
        
        public GameObject Get(AssetKey key)
        {
            var obj = _assetResolver.Get<GameObject>(key);
            return obj;
        }

        public GameObject Create(AssetKey key, Vector3 position, Quaternion rotations, Transform parent)
        {
            return Object.Instantiate(Get(key), position, rotations, parent);
        }
    }
}