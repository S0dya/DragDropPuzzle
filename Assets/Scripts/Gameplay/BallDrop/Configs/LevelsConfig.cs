using Gameplay.BallDrop.Levels;
using Gameplay.BallDrop.Levels.Types;
using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Configs
{
    [CreateAssetMenu(menuName = "Configs/LevelConfig", fileName = "LevelConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private SerializableKeyValue<LevelChunkName, LevelChunk> levels; 
        
        public SerializableKeyValue<LevelChunkName, LevelChunk> Levels => levels;
    }
}