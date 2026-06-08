using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PT.Tools.Sequences.Steps
{
    [Serializable]
    public class DelayStep : SequenceStep
    {
        [SerializeField] private float delay;
        
        public override async UniTask PlayStep(SequenceInfo sequenceInfo, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), 
                    cancellationToken: token);
            }
            catch (OperationCanceledException){}
        }
        
        public override void SkipStep(SequenceInfo sequenceInfo)
        {
        }
    }
}