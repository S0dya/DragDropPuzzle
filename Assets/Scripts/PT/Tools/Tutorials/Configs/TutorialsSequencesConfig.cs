using UnityEngine;

namespace PT.Tools.Tutorials.Configs
{
    [CreateAssetMenu(menuName = "Configs/TutorialSequencesConfig", fileName = "TutorialSequencesConfig")]
    public class TutorialsSequencesConfig : ScriptableObject
    {
        [SerializeField] private TutorialSequenceConfig[] tutorialConfigs;
        
        public TutorialSequenceConfig[] TutorialConfigs => tutorialConfigs;
    }
}