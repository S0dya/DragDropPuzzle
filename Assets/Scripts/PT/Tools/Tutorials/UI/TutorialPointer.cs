using DG.Tweening;
using UnityEngine;

namespace PT.Tools.Tutorials.UI
{
     public class TutorialPointer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform rect;

        [Header("Durations")]
        [SerializeField] private float moveDuration = 0.5f;

        [Header("Idle Settings")]
        [SerializeField] private float idleDistance = 20f;
        [SerializeField] private bool idleVertical = true;

        private Tween _idleTween;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            KillTweens();
        }

        public void ShowAt(Vector2 screenPosition, bool withIdle = true)
        {
            KillTweens();
            rect.position = screenPosition;
            gameObject.SetActive(true);

            if (withIdle) StartIdleMove();
        }

        public void Hide()
        {
            KillTweens();
            rect.localScale = Vector3.one;
            gameObject.SetActive(false);
        }
        private void StartIdleMove()
        {
            KillTweens();

            var offset = idleVertical
                ? Vector2.up * idleDistance
                : Vector2.right * idleDistance;

            _idleTween = DOTween.Sequence()
                .Append(rect.DOAnchorPos(rect.anchoredPosition + offset, moveDuration)
                    .SetEase(Ease.InOutSine))
                .Append(rect.DOAnchorPos(rect.anchoredPosition, moveDuration)
                    .SetEase(Ease.InOutSine))
                .Join(rect.DOScale(1.05f, moveDuration).SetLoops(2, LoopType.Yoyo))
                .Join(rect.DORotate(new Vector3(0, 0, 5f), moveDuration * 0.5f)
                    .SetLoops(4, LoopType.Yoyo))
                .SetLoops(-1); 
        }
        
        private void KillTweens()
        {
            _idleTween?.Kill();
            rect.DOKill();
        }
    }
}