using DG.Tweening;
using UnityEngine;

namespace PT.GameplayAdditional.Animations
{
    public class SimpleScaleLoop : MonoBehaviour
    {
        [SerializeField] private Vector2 minScale = new(0.8f, 0.8f);
        [SerializeField] private Vector2 maxScale = new(1.2f, 1.2f);
        [SerializeField] private float speed = 1f; 

        private Tween _tween;
        private Vector2 _startScale;

        private void OnEnable()
        {
            _startScale = transform.localScale;
            Scale();
        }
        private void OnDisable()
        {
            StopScale();
            transform.localScale = _startScale; 
        }

        public void Scale()
        {
            transform.localScale = minScale;

            _tween = transform
                .DOScale(maxScale, speed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        public void StopScale()
        {
            _tween?.Kill();
        }
    }
}