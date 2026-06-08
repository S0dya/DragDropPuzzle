using DG.Tweening;
using UnityEngine;

namespace PT.GameplayAdditional.Animations
{
    public class SimpleBounceLoop : MonoBehaviour
    {
        [SerializeField] private float distance = 30f;
        [SerializeField] private float speed = 1f;

        private Tween _tween;
        private RectTransform _rectTransform;
        private Vector2 _startPos;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _startPos = _rectTransform.anchoredPosition;
            Bounce();
        }
        private void OnDisable()
        {
            StopBounce();
            _rectTransform.anchoredPosition = _startPos; 
        }

        public void Bounce()
        {
            _tween = _rectTransform
                .DOAnchorPosY(_startPos.y + distance, speed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }
        public void StopBounce()
        {
            _tween?.Kill();
        }
    }
}