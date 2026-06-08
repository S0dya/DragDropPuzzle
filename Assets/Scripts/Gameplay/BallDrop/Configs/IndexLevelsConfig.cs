using Gameplay.BallDrop.Levels;
using Gameplay.BallDrop.Levels.Types;
using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Configs
{
    [CreateAssetMenu(menuName = "Configs/IndexLevelsConfig", fileName = "IndexLevelsConfig")]
    public class IndexLevelsConfig : ScriptableObject
    {
        [SerializeField] private SerializableKeyValue<int, LevelChunkName[]> levelsNames; 
        
        //import database and take from it 
        
        public SerializableKeyValue<int, LevelChunkName[]> LevelsNames => levelsNames;
    }
}