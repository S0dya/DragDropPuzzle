using UnityEngine;

namespace PT.GameplayAdditional.Progression
{
    [CreateAssetMenu(menuName = "Configs/StageConfig", fileName = "Stage Config")]
    public class StagesConfig : ScriptableObject
    {
        [SerializeField] private StageInfo[] stageInfos;
        
        public StageInfo[] StageInfos => stageInfos;
    }
}