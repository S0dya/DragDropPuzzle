using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace PT.Tools.Sequences.Steps
{
    [Serializable]
    public class TimelineStep : SequenceStep
    {
        [SerializeField] private PlayableDirector director;
        
        public override async UniTask PlayStep(SequenceInfo sequenceInfo, CancellationToken token)
        {
            if (!director) return;

            director.time = 0;
            director.Play();

            try
            {
                await UniTask.WaitUntil(
                    () => director.state != PlayState.Playing,
                    cancellationToken: token
                );
            }
            catch (OperationCanceledException)
            {
                SkipStep(sequenceInfo);
            }
        }

        public override void SkipStep(SequenceInfo sequenceInfo)
        {
            if (!director) return;

            director.time = director.duration;
            director.Evaluate();
            director.Stop();
        }
    }
}