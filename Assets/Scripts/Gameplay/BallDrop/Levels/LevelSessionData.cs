using System.Collections.Generic;

namespace Gameplay.BallDrop.Levels
{
    public class LevelSessionData
    {
        public Dictionary<float, LevelSizeChangingTrigger> LevelSizeTriggers { get; private set; }
        
        public void InitLevelSizeTriggers(Dictionary<float, LevelSizeChangingTrigger> triggers) => LevelSizeTriggers = triggers;
    }
}