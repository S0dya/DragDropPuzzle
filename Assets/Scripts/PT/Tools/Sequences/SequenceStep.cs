using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PT.Tools.Sequences
{
    [Serializable]
    public abstract class SequenceStep
    {
        public abstract UniTask PlayStep(SequenceInfo sequenceInfo, CancellationToken token);
        public abstract void SkipStep(SequenceInfo sequenceInfo);
    }
}