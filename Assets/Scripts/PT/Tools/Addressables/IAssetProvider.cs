using Cysharp.Threading.Tasks;

namespace PT.Tools.Addressables
{
    public interface IAssetProvider
    {
        UniTask Load<T>(AssetKey key) where T : UnityEngine.Object;
        void Release(AssetKey key);
    }
}