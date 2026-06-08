using System;
using UnityEngine;

namespace PT.GameplayAdditional.Progression
{
    [Serializable]
    public class StageInfo
    {
        [SerializeField] private int startingLevel;
        [Space]
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite pattern;
        
        public int StartingLevel => startingLevel;
        
        public Sprite Icon => icon;
        public Sprite Pattern => pattern;
    }
}