using System.Collections.Generic;

namespace Gameplay.Analytics
{
    public class RunState
    {
        public int AdsShownCount;
        public bool IsVictory;

        public readonly List<float> DecisionTimes = new();
        
        public int TotalMoneyEarned;
        public int TotalMoneySpent;

        public void Reset()
        {
            AdsShownCount = 0;
            IsVictory = false;
        }
    }
}