using UnityEngine;

namespace Gameplay.BallDrop.Levels.Generation
{
    public interface ILevelGenerationService
    {
        LevelBuildResult GenerateLevel(Transform parent);
    }
}