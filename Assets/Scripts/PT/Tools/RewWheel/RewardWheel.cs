using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace PT.Tools.RewWheel
{
    public class RewardWheel : MonoBehaviour
    {
        [Serializable]
        internal class RewardWheelSegment
        {
            [SerializeField] private float startAngle;
            [SerializeField, Min(1)] private float multiplier = 1;
            [SerializeField] private TextMeshProUGUI segmentText;
            
            public float StartAngle => startAngle;
            public float Multiplier => multiplier;

            public void SetText()
            {
                if (segmentText != null)
                    segmentText.text = $"x{multiplier}";
            }
        }

        [SerializeField] private int maxAngle = 180;
        [SerializeField] private float spinDuration = 1.5f;
        [SerializeField] private Ease spinEase = Ease.InOutSine;
        [Space]
        [SerializeField] private RewardWheelSegment[] rewardWheelSegments;
        [Space]
        [SerializeField] private RewardWheelView rewardWheelView;
        
        public readonly ReactiveProperty<float> CurrentMultiplier = new(1);
        public readonly ReactiveProperty<int> CurrentRewardValue = new(0);

        private Tween _spinTween;
        private bool _isSpinning;
        
        private int _baseRewardAmount;
        private float _currentAngle;
        private RewardWheelSegment _lastSegment;

        private void Start()
        {
            foreach (var segment in rewardWheelSegments)
                segment.SetText();

            rewardWheelView.SetAngle(0);
            UpdateSegment(0);
        }

        public void StartSpinning(int baseAmount)
        {
            if (_isSpinning) return;
            _isSpinning = true;

            _baseRewardAmount = baseAmount;
            
            ResetWheel();

            StopAndKillTween();
            _spinTween = DOVirtual.Float(0, maxAngle, spinDuration, UpdateAngle)
                            .SetEase(spinEase)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetTarget(this) 
                            .OnKill(() => _isSpinning = false);
        }
        public int StopSpinningGetReward()
        {
            if (!_isSpinning)
                return Mathf.RoundToInt(_baseRewardAmount * CurrentMultiplier.Value);

            StopSpinning();

            return Mathf.RoundToInt(_baseRewardAmount * CurrentMultiplier.Value);
        }
        public void StopSpinning()
        {
            if (!_isSpinning) return;
            
            _isSpinning = false;
            StopAndKillTween();
        }

        private void UpdateSegment(float angle)
        {
            RewardWheelSegment currentSegment = null;

            foreach (var segment in rewardWheelSegments)
            {
                if (angle >= segment.StartAngle)
                    currentSegment = segment;
            }

            if (_lastSegment != currentSegment)
            {
                _lastSegment = currentSegment;

                CurrentMultiplier.Value = currentSegment.Multiplier;
                CurrentRewardValue.Value = Mathf.RoundToInt(_baseRewardAmount * currentSegment.Multiplier);

                rewardWheelView.SetText($"x{currentSegment.Multiplier}");
            }
        }

        private void UpdateAngle(float angle)
        {
            _currentAngle = angle;
            rewardWheelView.SetAngle(angle);
            UpdateSegment(angle);
        }

        private void ResetWheel()
        {
            StopAndKillTween();
            _currentAngle = 0;
            UpdateAngle(0);
        }

		private void StopAndKillTween()
        {
            if (_spinTween != null && _spinTween.IsActive())
            {
                _spinTween.Kill();
                _spinTween = null;
            }
        }
    }
}
