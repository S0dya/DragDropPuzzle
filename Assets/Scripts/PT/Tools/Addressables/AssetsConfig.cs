using PT.Logic.PersistentScene;
using PT.Tools.Helper;
using UnityEngine;

namespace PT.Tools.Addressables
{
    [CreateAssetMenu(menuName = "Configs/AssetsConfig", fileName = "AssetsConfig")]
    public class AssetsConfig : ScriptableObject
    {
        [SerializeField] private AssetKey[] globalAssets;
        [Space]
        [SerializeField] private SerializableKeyValue<SceneNameEnum, AssetKey[]> sceneAssets;
        
        public AssetKey[] GlobalAssets => globalAssets;
        
        public SerializableKeyValue<SceneNameEnum, AssetKey[]> SceneAssets => sceneAssets;
    }
}