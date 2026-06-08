using DG.Tweening;
using Libraries.RSG;
using NaughtyAttributes;
using PT.Tools.Helper;
using TMPro;
using UnityEngine;

namespace PT.UI.Effects
{
    public class TextPopUp : MonoBehaviour
    {
        [SerializeField] protected RectTransform rect;
        [SerializeField] protected TextMeshProUGUI text;
        [SerializeField] protected CanvasGroup canvasGroup;
        [Header("Spawn")]
        [SerializeField][MinMaxSlider(-100, 100)] protected Vector2 offsetXRandom;
        [SerializeField] protected float moveUpDistance = 80f;
        [SerializeField] protected float initialScale = 0.4f;
        [Header("Animation")]
        [SerializeField] protected float scaleUp = 1.2f;
        [SerializeField] protected float scaleUpTime = 0.12f;
        [SerializeField] protected float moveTime = 0.35f;
        [SerializeField] protected float holdTime = 0.08f;
        [SerializeField] protected float fadeTime = 0.18f;

        private Sequence _seq;

        public IPromise Play(string str = null, Vector2? startPos = null)
        {
            Kill();
            
            ApplyStyle();
            
            if (str != null) text.text = str;
            canvasGroup.alpha = 1f;

            if (startPos != null) rect.anchoredPosition = startPos.Value + Vector2.right * offsetXRandom.GetRandomValue();
            rect.localScale = Vector2.one * initialScale;

            var targetPos = rect.anchoredPosition + Vector2.up * moveUpDistance;

            gameObject.SetActive(true);
            
            _seq = DOTween.Sequence();

            _seq.Append(rect.DOScale(scaleUp, scaleUpTime).SetEase(Ease.OutBack));
            _seq.Append(rect.DOScale(1f, scaleUpTime * 0.6f).SetEase(Ease.OutQuad));
            _seq.Join(rect.DOAnchorPos(targetPos, moveTime).SetEase(Ease.OutCubic));
            _seq.AppendInterval(holdTime);
            _seq.Append(canvasGroup.DOFade(0f, fadeTime));

            _seq.OnComplete(() => { gameObject.SetActive(false); });
            _seq.SetTarget(this);
            
            return _seq.ToPromise();
        }

        public void Cancel()
        {
            Kill();
            
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Kill();
        }
        private void OnDestroy()
        {
            Kill();
        }
        
        private void Kill()
        {
            if (_seq != null && _seq.IsActive()) _seq.Kill(complete: true);
        }
        
        protected virtual void ApplyStyle() {}
    }
}