using System;
using PT.Tools.Helper;
using UnityEngine;

namespace PT.GameplayAdditional.Input
{
    public class SwipeDetector
    {
        private Vector2 _startPos;
        private float _startTime;
        private readonly float _minDistance = 60f;
        private readonly float _maxTime = 0.5f;

        private event Action<DirectionEnum> _onSwipe;
        private bool _hasPressed;

        public SwipeDetector(Action<DirectionEnum> onSwipe)
        {
            _onSwipe = onSwipe;
        }

        public void OnPress(Vector2 pos)
        {
            _hasPressed = true;
            _startPos = pos;
            _startTime = Time.time;
        }

        public void OnRelease(Vector2 pos)
        {
            if (!_hasPressed) return;
            _hasPressed = false;

            float duration = Time.time - _startTime;
            if (duration > _maxTime) return;      

            float dist = (pos - _startPos).magnitude;
            if (dist < _minDistance) return;      

            Vector2 dir = pos - _startPos;
            dir.NormalizeIntoCardinalDirection();

            dir.y = -dir.y; 

            _onSwipe?.Invoke(Utils.GetDirectionEnumFromCardinalDirection(dir));
        }
    }
}