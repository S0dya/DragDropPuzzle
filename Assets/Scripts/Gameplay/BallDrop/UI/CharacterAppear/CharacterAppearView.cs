using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BallDrop.UI.CharacterAppear
{
    public class CharacterAppearView : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [Space]
        [SerializeField] private float appearDuration = 0.22f;
        [SerializeField] private float disappearDuration = 0.18f;
        [SerializeField] private float appearYOffset = 18f;
        [Space]
        [SerializeField] private float breathScale = 1.03f;
        [SerializeField] private float breathDuration = 1.8f;

        private RectTransform _rectTransform;

        private Tween _appearTween;
        private Tween _disappearTween;
        private Tween _breathTween;
        private Sequence _appearSequence;
        private Vector2 _anchoredBasePos;

        private bool _isVisible;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        public void Init(Sprite sprite)
        {
            characterImage.sprite = sprite;

            transform.localScale = Vector3.one;
            canvasGroup.alpha = 0f;
            _isVisible = false;
        }

        public void SetToSlot(RectTransform slot)
        {
            KillTweens();

            _rectTransform.anchorMin = slot.anchorMin;
            _rectTransform.anchorMax = slot.anchorMax;
            _rectTransform.pivot = slot.pivot;
            _rectTransform.anchoredPosition = slot.anchoredPosition;
            _rectTransform.sizeDelta = slot.sizeDelta;
            _rectTransform.localRotation = slot.localRotation;
            _rectTransform.localScale = Vector3.one;

            _anchoredBasePos = _rectTransform.anchoredPosition;
        }

        public void Appear()
        {
            KillTweens();

            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            _isVisible = true;

            _rectTransform.localScale = Vector3.one * 0.98f;
            _rectTransform.anchoredPosition = _anchoredBasePos + Vector2.down * appearYOffset;
            canvasGroup.alpha = 0f;

            _appearSequence = DOTween.Sequence();

            _appearSequence.Join(canvasGroup.DOFade(1f, appearDuration));
            _appearSequence.Join(_rectTransform.DOAnchorPos(_anchoredBasePos, appearDuration).SetEase(Ease.OutQuad));
            _appearSequence.Join(_rectTransform.DOScale(1f, appearDuration).SetEase(Ease.OutQuad));
            _appearSequence.OnComplete(StartBreathLoop);
        }

        public void Disappear()
        {
            if (!_isVisible) return;

            KillTweens();

            _isVisible = false;

            _disappearTween = DOTween.Sequence()
                .Join(canvasGroup.DOFade(0f, disappearDuration))
                .Join(_rectTransform.DOScale(0.97f, disappearDuration).SetEase(Ease.OutQuad))
                .Join(_rectTransform.DOAnchorPos(_anchoredBasePos + Vector2.down * (appearYOffset * 0.5f), disappearDuration).SetEase(Ease.OutQuad));
        }

        private void StartBreathLoop()
        {
            if (!_isVisible) return;

            _breathTween = _rectTransform
                .DOScale(breathScale, breathDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void KillTweens()
        {
            _appearTween?.Kill();
            _disappearTween?.Kill();
            _breathTween?.Kill();
            _appearSequence?.Kill();
        }

        private void OnDestroy()
        {
            KillTweens();
        }
    }
}