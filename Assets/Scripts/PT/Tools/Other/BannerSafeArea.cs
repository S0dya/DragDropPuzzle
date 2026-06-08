using PT.Logic.Ads;
using PT.Logic.Configs;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Other
{
    public class BannerSafeArea : MonoBehaviour
    {
        [SerializeField] private RectTransform root;

        [Inject] private AdsManager _adsManager;
        [Inject] private AdConfig _adConfig;

        private Vector2 _originalOffsetMin;

        private void Awake()
        {
            if (!root) root = GetComponent<RectTransform>();

            _originalOffsetMin = root.offsetMin;
            
            if (_adsManager.TryGetBannerReactive(out var property)) 
                property.Subscribe(OnBannerToggle).AddTo(this);
        }

        void OnBannerToggle(bool active)
        {
            root.offsetMin = new Vector2(
                _originalOffsetMin.x,
                active ? _originalOffsetMin.y + _adConfig.BannerHeightPx : _originalOffsetMin.y
            );
        }
    }
}