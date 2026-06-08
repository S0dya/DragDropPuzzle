using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PT.Logic.PersistentScene.Loading.Steps
{
    public class FakeWaitingStep : ILoadingStep
    {
        private readonly float _duration;

        public FakeWaitingStep(float duration) => _duration = duration;

        public async UniTask Load(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: token);
        }
    }
}