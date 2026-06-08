using Cysharp.Threading.Tasks;
using DG.Tweening;
using PT.Logic.Configs;
using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class LoadingWindow : WindowBase 
    {
        [Inject] private GameConfig _gameConfig;
        
        [SerializeField] private Image sliderImage;

        private Tween _progressTween;

        public void SetProgress(float value) => SetProgress(value, _gameConfig.LoadingAnimationDuration);
        public void SetProgress(float value, float duration)
        {
            _progressTween?.Kill();
            _progressTween = sliderImage.DOFillAmount(value, duration).SetEase(Ease.OutQuad);
        }
        public void ResetProgress()
        {
            sliderImage.fillAmount = 0f;
        }

        protected override async UniTask OnOpen()
        {
            ResetProgress();
            
            await base.OnClose();
        }
        protected override async UniTask OnClose()
        {
            _progressTween?.Kill();
            _progressTween = null;
            
            await base.OnClose();
        }
    }
}