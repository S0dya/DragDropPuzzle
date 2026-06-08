using System;
using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Datas;
using PT.Tools.Helper;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.BallDrop.UI
{
    public class PushableView : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        [Inject] private PushableBallManager _pushableBallManager;
        
        private CompositeDisposable _disposable = new();

        private void Start()
        {
            _pushableBallManager.OnPushableBallChanged += OnPushableBallChanged;
            
            _pushableBallManager.PushProgress
                .Subscribe(OnPushProgressChanged)
                .AddTo(_disposable);

            HideUI();
        }

        private void OnDestroy()
        {
            if (_pushableBallManager != null)
            {
                _pushableBallManager.OnPushableBallChanged -= OnPushableBallChanged;
            }
            
            _disposable?.Dispose();
        }

        private void OnPushableBallChanged(PushableBall pushableBall)
        {
            if (pushableBall == null || _pushableBallManager.PushProgress.Value < 1)
            {
                HideUI();
            }
            else
            {
                ShowUI();
                fillImage.fillAmount = 1f;
            }
        }

        private void OnPushProgressChanged(float progress)
        {
            fillImage.fillAmount = progress;
        }

        private void ShowUI()
        {
            // if (fillImage != null)
                // fillImage.SetActive(true);
        }

        private void HideUI()
        {
            // if (fillImage != null)
                // fillImage.SetActive(false);
        }
    }
}
