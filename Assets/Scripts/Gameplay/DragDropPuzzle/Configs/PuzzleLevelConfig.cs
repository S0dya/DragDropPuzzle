using System;
using System.Collections.Generic;
using Gameplay.DragDropPuzzle.Data;
using UnityEngine;

namespace Gameplay.DragDropPuzzle.Configs
{
    [CreateAssetMenu(menuName = "Configs/PuzzleLevelConfig", fileName = "PuzzleLevelConfig")]
    public class PuzzleLevelConfig : ScriptableObject
    {
        [SerializeField] private List<PuzzleLevelData> levels;

        public List<PuzzleLevelData> Levels => levels;

        public PuzzleLevelData GetLevel(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < levels.Count)
                return levels[levelIndex];
            return null;
        }
    }

    [Serializable]
    public class PuzzleLevelData
    {
        [SerializeField] private string levelName;
        [SerializeField] private PuzzleLevel level;

        public string LevelName => levelName;
        public PuzzleLevel Level => level;
    }
}
