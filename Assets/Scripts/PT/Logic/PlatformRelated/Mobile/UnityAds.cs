using System;
using PT.Logic.Ads;
using PT.Logic.Configs;
using PT.Tools.Debugging;
using UniRx;
using UnityEngine.Advertisements;
using Zenject;

namespace PT.Logic.PlatformRelated.Mobile
{
    public class UnityAds : IAdsService, IBannerService, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public event Action OnRewardAdCompleted;
        public event Action OnAdCompleted;
        public event Action OnAdStarted;

        public ReactiveProperty<bool> IsBannerActive { get; private set; } = new(false);

        private AdConfig _adConfig;
     
        private bool _bannerLoaded;

        [Inject]
        public void Construct(AdConfig adConfig)
        {
            _adConfig = adConfig;

            Advertisement.Initialize(_adConfig.GameId, true, this);
        }

        public void ShowAd()
        {
            Advertisement.Show(_adConfig.InterstitialVideo, this);
        }
        public void ShowRewardedAd()
        {
            Advertisement.Show(_adConfig.RewardedVideo, this);
        }


        #region Unity Ads Interface Implementations
        public void OnInitializationComplete()
        {
            Advertisement.Load(_adConfig.GameId, this);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            DebugManager.Log(DebugCategory.Ads, $"Init Failed: [{error}]: {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            DebugManager.Log(DebugCategory.Ads, $"Load Failed: [{error}:{placementId}] {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            DebugManager.Log(DebugCategory.Ads, $"OnUnityAdsShowFailure Failed: [{error}:{placementId}] {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            OnAdStarted?.Invoke();
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED && placementId == _adConfig.GameId)
                OnRewardAdCompleted?.Invoke();
            else if (placementId == _adConfig.InterstitialVideo) OnAdCompleted?.Invoke();
        }
        
        public void ShowBanner()
        {
            if (!_bannerLoaded)
            {
                LoadBanner();
                return;
            }

            Advertisement.Banner.Show(_adConfig.Banner);
            IsBannerActive.Value = true;
        }
        public void HideBanner()
        {
            Advertisement.Banner.Hide(false);
            IsBannerActive.Value = false;
        }
        
        #endregion
        
        #region Banner

        private void LoadBanner()
        {
            var options = new BannerLoadOptions
            {
                loadCallback = () =>
                {
                    _bannerLoaded = true;
                    Advertisement.Banner.Show(_adConfig.Banner);
                    IsBannerActive.Value = true;
                },
                errorCallback = error =>
                {
                    DebugManager.Log(DebugCategory.Ads, $"Banner load error: {error}");
                }
            };

            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Load(_adConfig.Banner, options);
        }

        #endregion
    }
}