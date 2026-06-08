using System.Collections.Generic;
using Gameplay.BallDrop.Configs;
using Gameplay.Session;
using PT.Tools.Factories;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Levels.Generation
{
    public class LengthLevelGenerationService : ILevelGenerationService
    {
        [Inject] private LengthLevelsConfig _config;
        [Inject] private GameSessionData _session;
        [Inject] private IFactoryZenject _factoryZenject;

        public LevelBuildResult GenerateLevel(Transform parent)
        {
            var triggers = new List<LevelSizeChangingTrigger>();
            var levelChunks = new List<LevelChunk>();
            
            var levelData = _config.LevelsDatas.Dictionary[_session.LevelLengthType];

            int obstaclesCount = levelData.ObstaclesAmountRangeRange.GetRandomValue();
            int transitionsCount = levelData.TransitionsAmountRangeRange.GetRandomValue();
            int spacing = 0;
            if (transitionsCount > 0 && obstaclesCount > 0) spacing = Mathf.Max(1, obstaclesCount / transitionsCount);
            int transitionIndex = 0;
            float lastY = 0f;

            for (int i = 0; i < obstaclesCount; i++)
            {
                levelChunks.Add(SpawnChunk(_config.ObstacleChunks.GetRandomElement()));

                if ((i + 1) % spacing == 0 && transitionIndex < transitionsCount)
                {
                    levelChunks.Add(SpawnChunk(_config.TransitionChunks.GetRandomElement()));
                    transitionIndex++;
                }
            }

            return new LevelBuildResult(triggers.ToArray(), lastY, levelChunks.ToArray());

            LevelChunk SpawnChunk(LevelChunk prefab)
            {
                var chunk = _factoryZenject.InstantiateObject(prefab.gameObject, parent).GetComponent<LevelChunk>();
                chunk.transform.localPosition = new Vector3(0f, lastY - Mathf.Abs(prefab.Height/2), 0f);

                lastY -= Mathf.Abs(prefab.Height);

                triggers.Add(chunk.SizeChangingTrigger);

                return chunk;
            }
        }
    }
}