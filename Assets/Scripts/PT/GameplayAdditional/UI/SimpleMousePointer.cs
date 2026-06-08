using DG.Tweening;
using UnityEngine;

namespace PT.GameplayAdditional.UI
{
    [DisallowMultipleComponent]
    public class SimpleMousePointer : MonoBehaviour
    {
        [SerializeField] private RectTransform pointerRect;
        [SerializeField] private Canvas canvas;
        [Space]
        [SerializeField] private float clickDownScale = 0.82f;
        [SerializeField] private float clickDownDuration = 0.08f;
        [SerializeField] private float clickUpDuration = 0.12f;
        [SerializeField] private Ease downEase = Ease.OutQuad;
        [SerializeField] private Ease upEase = Ease.OutBack;
        [Space]
        [SerializeField] private float clickRotateZ = -12f;
        [SerializeField] private Ease rotateEase = Ease.OutQuad;

        private Tween _clickTween;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            UpdatePointerPosition();

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                PlayClickAnimation();
            }
        }

        private void UpdatePointerPosition()
        {
            var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                UnityEngine.Input.mousePosition,
                cam,
                out var localPos
            );

            pointerRect.anchoredPosition = localPos;
        }

        private void PlayClickAnimation()
        {
            if (pointerRect == null) return;

            _clickTween?.Kill();
            _clickTween = null;

            pointerRect.localScale = Vector3.one;
            pointerRect.localRotation = Quaternion.identity;

            var seq = DOTween.Sequence();

            seq.Append(pointerRect.DOScale(clickDownScale, clickDownDuration).SetEase(downEase));
            seq.Join(pointerRect.DOLocalRotate(new Vector3(0f, 0f, clickRotateZ), clickDownDuration).SetEase(rotateEase));

            seq.Append(pointerRect.DOScale(1f, clickUpDuration).SetEase(upEase));
            seq.Join(pointerRect.DOLocalRotate(Vector3.zero, clickUpDuration).SetEase(rotateEase));

            _clickTween = seq;
        }

        private void OnDisable()
        {
            _clickTween?.Kill();
            _clickTween = null;
        }

        private void OnDestroy()
        {
            _clickTween?.Kill();
            _clickTween = null;
        }
    }
}