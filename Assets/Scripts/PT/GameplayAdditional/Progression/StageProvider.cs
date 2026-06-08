using System.Linq;
using PT.Logic.Save;
using UnityEngine;

namespace PT.GameplayAdditional.Progression
{
    public class StageProvider
    {
        private StageInfo[] _stageInfos;
        
        public StageProvider(StagesConfig config)
        {
             _stageInfos = config.StageInfos;
        }
        
        public StageInfo GetCurrentStageInfo()
        {
            var currentLevel = (int)GameDataRegistry.Get(GameDataKey.LevelIndex);

            var next = _stageInfos.FirstOrDefault(x => x.StartingLevel > currentLevel);

            if (next == null) return _stageInfos.Last();

            int index = System.Array.IndexOf(_stageInfos, next);
            return _stageInfos[Mathf.Max(0, index - 1)];
        }

        public StageInfo GetNextStageInfo()
        {
            var currentLevel = (int)GameDataRegistry.Get(GameDataKey.LevelIndex);

            var next = _stageInfos.FirstOrDefault(x => x.StartingLevel > currentLevel);
            return next ?? _stageInfos.Last(); 
        }
        
        public float GetStageProgress()
        {
            var currentLevel = (int)GameDataRegistry.Get(GameDataKey.LevelIndex);

            var next = _stageInfos.FirstOrDefault(x => x.StartingLevel > currentLevel);
            var nextLevel = next?.StartingLevel ?? (currentLevel + 1);
            var current = GetCurrentStageInfo();

            int stageStart = current.StartingLevel;
            int stageEnd = nextLevel;

            float progress = Mathf.InverseLerp(stageStart, stageEnd, currentLevel);
            return Mathf.Clamp01(progress);
        }
    }
}