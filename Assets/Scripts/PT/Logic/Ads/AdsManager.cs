using System;
using Cysharp.Threading.Tasks;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Logic.Ads
{
    public class AdsManager : MonoBehaviour
    {
        [Inject] private AdConfig _adConfig;
        [Inject] private IAdsService _adsService;
        [Inject (Optional = true)] private IBannerService _bannerService;
        [Inject] private SignalBus _signalBus;

        private Action _rewardAction;

        private bool _takesBreakFromAd = true;

        private void Awake()
        {
            if (_adsService == null)
            {
                DebugManager.Log(DebugCategory.Ads, "No IAdsService injected. Ads disabled.", LogType.Warning); return;
            }

            _adsService.OnRewardAdCompleted += OnRewardAdCompleted;
            _adsService.OnAdCompleted += OnAdCompleted;
            
            _signalBus.Subscribe<ShowAdSignal>(ShowAd);
            _signalBus.Subscribe<AdOpenedSignal>(OnAdOpened);
        }

        private void Start()
        {
            TakeBreakFromAdTask().Forget();
        }

        public void ShowAd()
        {
            if (_takesBreakFromAd)
            {
                DebugManager.Log(DebugCategory.Ads, "ShowAd() called but still in cooldown. Ad not shown.");
                return;
            }

            DebugManager.Log(DebugCategory.Ads, "Showing fullscreen ad...");
            _signalBus.Fire(new AdOpenedSignal());

            _adsService?.ShowAd();
        }

        public void ShowRewardAd(Action rewardAction)
        {
            _signalBus.Fire(new AdOpenedSignal());

            _rewardAction = rewardAction;

            DebugManager.Log(DebugCategory.Ads, "Showing rewarded ad...");

            _adsService?.ShowRewardedAd();
        }

        public void ShowBanner()
        {
            _bannerService?.ShowBanner();
        }
        public void HideBanner()
        {
            _bannerService?.HideBanner();
        }
        public bool TryGetBannerReactive(out ReactiveProperty<bool> property)
        {
            if (_bannerService != null)
            {
                property = _bannerService.IsBannerActive;
                return true;
            }

            property = null;
            return false;
        } 
        
        private void OnRewardAdCompleted()
        {
            DebugManager.Log(DebugCategory.Ads, "Rewarded ad completed. Reward granted.");

            _rewardAction?.Invoke();
            OnAdCompleted();
        }

        private void OnAdOpened()
        {
            DebugManager.Log(DebugCategory.Ads, "Ad opened. Pausing ad availability.");
            _takesBreakFromAd = true;
        }

        private void OnAdCompleted()
        {
            _signalBus.Fire(new AdClosedSignal());

            TakeBreakFromAdTask().Forget();
        }

        private async UniTask TakeBreakFromAdTask()
        {
            _takesBreakFromAd = true;

            DebugManager.Log(DebugCategory.Ads, $"Cooldown started for {_adConfig.BreakFromAdsDuration}s.");
            await UniTask.WaitForSeconds(_adConfig.BreakFromAdsDuration);

            _takesBreakFromAd = false;
            DebugManager.Log(DebugCategory.Ads, "Cooldown finished. Ads available again.");
        }
    }
}
