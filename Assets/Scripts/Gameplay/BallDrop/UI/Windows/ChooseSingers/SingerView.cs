using System;
using DG.Tweening;
using Gameplay.BallDrop.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BallDrop.UI.Windows.ChooseSingers
{
    public class SingerView : MonoBehaviour
    {
        [SerializeField] private Image coverImage;
        [SerializeField] private Button selectButton;
        [SerializeField] private RectTransform contentRect;
        [Space]
        [SerializeField] private GameObject selectedObject;
        [Space]
        [SerializeField] private RectTransform animatedRect;

        public CharacterData CharacterData { get; private set; }

        public event Action<SingerView> OnPressed;

        private Vector3 _startScale;
        private Tween _rejectTween;
        private Tween _selectTween;

        private void Awake()
        {
            _startScale = animatedRect.localScale;

            selectedObject.SetActive(false);
            
            selectButton.onClick.AddListener(() => OnPressed?.Invoke(this));
        }
        
        public void SetInfo(CharacterData characterData)
        {
            CharacterData = characterData;
            
            coverImage.sprite = characterData.BallSprite;

            Deselect();
            ResetVisual();
            
            animatedRect.localScale = _startScale;
        }

        public void Select()
        {
            selectedObject.SetActive(true);
            PlaySelectAnimation();
        }

        public void Deselect()
        {
            selectedObject.SetActive(false);
            PlayDeselectAnimation();
        }

        public void PlayRejectAnimation()
        {
            _rejectTween?.Kill();

            var returnPos = contentRect.anchoredPosition;
            
            _rejectTween = contentRect
                .DOShakeAnchorPos(0.18f, new Vector2(14f, 0f), 10, 90f, false, true)
                .OnKill(() => contentRect.anchoredPosition = returnPos)
                .OnComplete(() => contentRect.anchoredPosition = returnPos);
        }

        private void PlaySelectAnimation()
        {
            _selectTween?.Kill();

            contentRect.localScale = Vector3.one;

            _selectTween = contentRect
                .DOScale(1.08f, 0.12f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    contentRect.DOScale(1f, 0.12f).SetEase(Ease.OutBack);
                });
        }

        private void PlayDeselectAnimation()
        {
            _selectTween?.Kill();

            contentRect.localScale = Vector3.one;

            _selectTween = contentRect
                .DOScale(0.94f, 0.08f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    contentRect.DOScale(1f, 0.12f).SetEase(Ease.OutBack);
                });
        }

        private void ResetVisual()
        {
            contentRect.localScale = Vector3.one;
        }

        private void OnDestroy()
        {
            _rejectTween?.Kill();
            _selectTween?.Kill();
        }
    }
}