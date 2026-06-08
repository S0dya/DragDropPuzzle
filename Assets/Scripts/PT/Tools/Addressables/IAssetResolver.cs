namespace PT.Tools.Addressables
{
    public interface IAssetResolver
    {
        T Get<T>(AssetKey key) where T : UnityEngine.Object;
    }
}