using UnityEngine;

namespace PT.Tools.Sequences
{
    public class SequenceInfo
    {
        public Transform TargetTransform { get; }

        public SequenceInfo(Transform targetTransform)
        {
            TargetTransform = targetTransform;
        }
    }
}