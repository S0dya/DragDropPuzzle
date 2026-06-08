using System.Collections.Generic;
using Gameplay.BallDrop.Configs;
using Gameplay.Session;
using PT.Tools.Factories;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Levels.Generation
{
    public class IndexLevelGenerationService : ILevelGenerationService
    {
        [Inject] private GameSessionData _sessionData;
        [Inject] private IndexLevelsConfig _levelsConfig;
        [Inject] private LevelsConfig _levels;
        [Inject] private IFactoryZenject _factoryZenject;
        
        public LevelBuildResult GenerateLevel(Transform parent)
        {
            var sizeChangingTriggers = new List<LevelSizeChangingTrigger>();
            var levelChunks = new List<LevelChunk>();
            
            var levelChunksToSpawn = _levelsConfig.LevelsNames.Dictionary[_sessionData.SelectedLevelIndex];

            float lastY = 0;
            
            foreach (var levelChunkName in levelChunksToSpawn)
            {
                var chunkPrefab = _levels.Levels.Dictionary[levelChunkName];
                var levelChunkObj = _factoryZenject.InstantiateObject(chunkPrefab.gameObject, parent);
                var levelChunk = levelChunkObj.GetComponent<LevelChunk>();
                levelChunks.Add(levelChunk);
                
                levelChunkObj.transform.position = new(0, - Mathf.Abs(levelChunk.Height/2));
                lastY -= Mathf.Abs(levelChunk.Height);
                
                if (levelChunk.SizeChangingTrigger != null) sizeChangingTriggers.Add(levelChunk.SizeChangingTrigger);
            }
            
            return new LevelBuildResult(sizeChangingTriggers.ToArray(), lastY, levelChunks.ToArray());
        }
    }
}