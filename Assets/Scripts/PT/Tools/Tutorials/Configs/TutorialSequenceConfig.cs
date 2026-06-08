using System.Collections.Generic;
using UnityEngine;

namespace PT.Tools.Tutorials.Configs
{
    [CreateAssetMenu(menuName = "Configs/TutorialSequence", fileName = "TutorialSequence")]
    public class TutorialSequenceConfig : ScriptableObject
    {
        [SerializeField] private TutorialSequenceEnum sequenceEnum;
        [SerializeField] private List<TutorialStepData> steps = new();
        
        public TutorialSequenceEnum SequenceEnum => sequenceEnum;
        public List<TutorialStepData> Steps => steps;
    }
}