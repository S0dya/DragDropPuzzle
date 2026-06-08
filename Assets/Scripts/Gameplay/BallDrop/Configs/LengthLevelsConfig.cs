using System;
using Gameplay.BallDrop.Levels;
using Gameplay.BallDrop.Levels.Types;
using NaughtyAttributes;
using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Configs
{
    [CreateAssetMenu(menuName = "Configs/LengthLevelsConfig", fileName = "LengthLevelsConfig")]
    public class LengthLevelsConfig : ScriptableObject
    {
        [SerializeField] private SerializableKeyValue<LevelLengthType, LevelLengthData> levelsDatas;
        [SerializeField] private LevelChunk[] obstacleChunks;
        [SerializeField] private LevelChunk[] transitionChunks;

        public SerializableKeyValue<LevelLengthType, LevelLengthData> LevelsDatas => levelsDatas;
        public LevelChunk[] ObstacleChunks => obstacleChunks;
        public LevelChunk[] TransitionChunks => transitionChunks;
    }

    [Serializable]
    public class LevelLengthData
    {
        [SerializeField][MinMaxSlider(3, 40)] private Vector2Int obstaclesAmountRange;
        [SerializeField][MinMaxSlider(3, 40)] private Vector2Int transitionsAmountRange;
        
        public Vector2Int ObstaclesAmountRangeRange => obstaclesAmountRange;
        public Vector2Int TransitionsAmountRangeRange => transitionsAmountRange;
    }
}