using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PT.Tools.Sequences.Steps
{
    [Serializable]
    public class ParticleStep : SequenceStep
    {
        [SerializeField] private float duration = 0.2f;

        private ParticleSystem _particle;
        
        public override async UniTask PlayStep(SequenceInfo sequenceInfo, CancellationToken token)
        {
            if (!_particle) _particle = sequenceInfo.TargetTransform.GetComponent<ParticleSystem>();
            
            _particle.Play();
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            }
            catch (OperationCanceledException) { }
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
        }

        public override void SkipStep(SequenceInfo sequenceInfo)
        {
            if (!_particle) return;
            
            _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        } 
    }
}