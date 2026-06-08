using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PT.UI.Buttons
{
	public class BasicButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
	{
		[SerializeField] private Button button;
		[SerializeField] private ParticleSystem effect;
        [SerializeField] private CanvasGroup canvasGroup;
        [Space(10)]
		[SerializeField] private bool needAnimateOnClick = true;
		[SerializeField] private bool needAnimateOnUnPress = true;
		[Space(10)]
		[SerializeField] private float scaleX = 0.95f;
		[SerializeField] private float scaleY = 0.95f;

        public bool Locked { get; private set; } = false;
        public bool IsVisible { get; private set; }
        private bool touchable
        {
            get => _touchable;
            set
            {
                _touchable = value;

                if (button) button.interactable = _touchable;
            }
        }

        private Button.ButtonClickedEvent _onClick = new();
        private Button.ButtonClickedEvent onClick => button ? button.onClick : _onClick;

        private bool _touchable = true;
        private bool _firstLockPass;

        private bool _wasPointerDown = false;
        private bool _wasPointerExit = false;

		private RectTransform _rectTransform;
        private Vector3 _baseScale;

        private Action _resetAction;

        private Tween _appearTween;
        private Sequence _selectTween;
        private Sequence _pressTween;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _baseScale = _rectTransform.localScale;
            if (effect) effect.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            SetEffect(false);
        }

        protected virtual void OnDestroy()
        {
            _selectTween?.Kill();
            _selectTween = null;
        }

        // public methods
        public void SetOnClick(Action onClick)
        {
            this.onClick.RemoveAllListeners();
            this.onClick.AddListener(() => onClick?.Invoke());
        }


        public void SetLock(bool val, bool _touchable  = false)
        {
            if (Locked == val && _firstLockPass) 
				return;
			_firstLockPass = true;
            Locked = val;

            touchable = val ? _touchable  : true;
        }

        public void ShowEffect()
        {
            IsVisible = true;
                                //SetVisible(true);
            SetEffect(true);
        }

        public void SetVisible(bool visible)
        {
            IsVisible = visible;
            AppearDisappear(visible);
        }

        public void StopAnim()
        {
            _selectTween?.Kill(); _selectTween = null;
        }

        // animation
        protected virtual void OnSelectAnimation()
		{
			if (!gameObject || !_rectTransform) return;

			_selectTween?.Kill();
			_selectTween = DOTween.Sequence()
				.SetLink(gameObject)
				.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 0.95f, _baseScale.y * 1.01f, _baseScale.z), 0.05f))
				.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 1.05f, _baseScale.y * 0.9f, _baseScale.z), 0.05f))
				.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 0.95f, _baseScale.y * 1.01f, _baseScale.z), 0.05f))
				.Append(_rectTransform.DOScale(_baseScale, 0.05f))
				.OnComplete(() => _selectTween = null)
				.SetUpdate(true); ;
			_selectTween.Play();
		}

        protected virtual void OnPressAnimationStart()
        {
            if (!gameObject) return;

            if (_pressTween != null)
            {
                _pressTween.Rewind(); _pressTween.Play();

                return;
            }

            _pressTween = DOTween.Sequence()
                .Append(_rectTransform.DOScale(new Vector3(_baseScale.x * scaleX, _baseScale.y * scaleY, _baseScale.z), 0.05f))
                .SetUpdate(true); ;

            _pressTween.SetLink(gameObject);
            _pressTween.SetAutoKill(false);

            _pressTween.Play();
        }

		protected virtual void OnPressAnimationFinished()
        {
            if (!gameObject) return;

            if (_pressTween != null)
            {
                _pressTween.Rewind();

                return;
            }
        }

        // pointer interface
        public void OnPointerClick(PointerEventData eventData) //REMOVE IF NOT USED
        {
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (!touchable)
                return;

            _wasPointerDown = true;
            _wasPointerExit = false;

            if (needAnimateOnClick) OnPressAnimationStart();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
			_wasPointerDown = false;

			if (_wasPointerExit)
			{
				_wasPointerExit = false;

				if (needAnimateOnClick) OnPressAnimationFinished();

				return;
			}

            if (canvasGroup != null && !canvasGroup.interactable) return;

			if (!touchable)
			{
				if (needAnimateOnClick) OnPressAnimationFinished();

				return;
			}

            if (eventData.dragging) // remove if doesnt work or not needed
            {
                if (needAnimateOnClick) OnPressAnimationFinished();
                
                return;
            }

			if (needAnimateOnClick)
			{
				if (needAnimateOnUnPress) OnSelectAnimation();
				else OnPressAnimationFinished();
			}

            if (!button) _onClick?.Invoke();

            PlaySoundClick();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!_wasPointerDown) return;

			_wasPointerExit = false;
			
			if (needAnimateOnClick) OnPressAnimationStart();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!_wasPointerDown) return;

			_wasPointerExit = true;
			
			if (needAnimateOnClick) OnPressAnimationFinished();
		}

        // other methods
        private void AppearDisappear(bool appear, bool needEffect = false, float duration = 0.25f)
        {
            if (!gameObject) return;

            _resetAction?.Invoke();
            _appearTween?.Kill();

            touchable = appear;

            gameObject.SetActive(appear);
            SetEffect(needEffect && appear);

            if (canvasGroup)
            {
                var alpha = appear ? 1f : 0f;
                _appearTween = canvasGroup
                    .DOFade(alpha, duration)
                    .SetEase(Ease.InSine)
                    .SetLink(gameObject);

                if (!appear) _appearTween.OnComplete(() => gameObject.SetActive(false));

                _resetAction = () =>
                {
                    _appearTween?.Kill();

                    if (!appear) gameObject.SetActive(false);
                    if (canvasGroup) canvasGroup.alpha = alpha;
                };
            }
        }

        internal void PlaySoundClick() // myb change later
        {
            //if (_soundClickCallback != null)
            //    _soundClickCallback?.Invoke();
            //else
            //	Game.Sound.PlayBasicButtonClick();
        }

        private void SetEffect(bool appear)
        {
            if (!effect) return;

            effect.gameObject.SetActive(appear);

            if (appear) effect.Play();
        }
    }
}


