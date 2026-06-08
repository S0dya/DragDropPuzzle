using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Tools.Debugging;
using UnityEngine;

namespace PT.Tools.TimeRelated
{
    public class TimeManager : MonoBehaviour 
    {
        class TimeRequest
        {
            public float Scale { get; private set; }
            public float InDuration { get; private set; }
            public float OutDuration { get; private set; }

            public TimeRequest(float scale, float inDuration, float outDuration)
            {
                Scale = Mathf.Clamp(scale, 0f, 1f);
                InDuration = Mathf.Max(0f, inDuration);
                OutDuration = Mathf.Max(0f, outDuration);
            }
        }
        
        private readonly Dictionary<string, TimeRequest> _requests = new();
        private CancellationTokenSource _cts = new();

        public void RequestTimeScale(string key, float scale, float inDuration = 0, float outDuration = 0)
        {
            DebugManager.Log(DebugCategory.Time, $"Requesting : {key} -> {scale} (in: {inDuration}, out: {outDuration})");
            
            _requests[key] = new TimeRequest(scale, inDuration, outDuration);
            UpdateTimeScale(true);
        }

        public void RemoveRequest(string key)
        {
            DebugManager.Log(DebugCategory.Time, $"Removing Request : {key}");

            if (_requests.Remove(key))
            {
                UpdateTimeScale(false);
            }
        }
        
        private void UpdateTimeScale(bool useInDuration)
        {
            float target = 1;
            float duration = 0;

            if (_requests.Count > 0)
            {
                target = Mathf.Min(1f, _requests.Values.Min(r => r.Scale));
                var controlling = _requests.Values.FirstOrDefault(r => Mathf.Approximately(r.Scale, target));
                
                if (controlling != null) duration = useInDuration ? controlling.InDuration : controlling.OutDuration;
            }
            
            SetTimeScaleSmooth(target, duration).Forget();
        }
        
        private async UniTaskVoid SetTimeScaleSmooth(float target, float duration)
        {
            _cts?.Cancel();
            _cts = new();

            if (duration <= 0f)
            {
                await UniTask.DelayFrame(1);
                
                Time.timeScale = target; 
                return;
            }

            float start = Time.timeScale;
            float time = 0f;

            try
            {
                while (time < duration)
                {
                    time += Time.unscaledDeltaTime;
                    Time.timeScale = Mathf.Lerp(start, target, time / duration);
                    await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
                }
            }
            catch (OperationCanceledException) { }

            Time.timeScale = target;
        }
    }
}