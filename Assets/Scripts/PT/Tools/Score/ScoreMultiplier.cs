using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PT.Tools.Score
{
    public class ScoreMultiplier : IDisposable
    {
        private float _baseMultiplier = 1f;
        private float _bonusMultiplier = 1f;

        private CancellationTokenSource _cts;

        public float Current => _baseMultiplier * _bonusMultiplier;

        public void SetBase(float value)
        {
            _baseMultiplier = value;
        }

        public void SetBonus(float value, float duration)
        {
            _bonusMultiplier = value;

            _cts?.Cancel();
            _cts = new();

            StartTimer(duration, _cts.Token).Forget();
        }

        private async UniTaskVoid StartTimer(float seconds, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            _bonusMultiplier = 1f;
        }

        public void Reset()
        {
            _cts?.Cancel();
            _bonusMultiplier = 1f;
            _baseMultiplier = 1f;
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }
    }
}