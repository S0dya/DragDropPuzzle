#if UNITY_WEBGL
using PT.Logic.Ads;
using System;
using YG;
#endif

namespace PT.Logic.PlatformRelated.YG
{
#if UNITY_WEBGL
    public class YandexAds : IAdsService
    {
        public event Action OnRewardAdCompleted;
        public event Action OnAdCompleted;

        public YandexAds()
        {
            YG2.onRewardAdv += OnRewardAdClosed;
            YG2.onCloseInterAdv += OnAdCompleted;
            YG2.onCloseRewardedAdv += OnAdCompleted;
            YG2.onErrorInterAdv += OnAdCompleted;
            YG2.onErrorRewardedAdv += OnAdCompleted;
        }

        ~YandexAds()
        {
            YG2.onRewardAdv -= OnRewardAdClosed;
            YG2.onCloseInterAdv -= OnAdCompleted;
            YG2.onCloseRewardedAdv -= OnAdCompleted;
            YG2.onErrorInterAdv -= OnAdCompleted;
            YG2.onErrorRewardedAdv -= OnAdCompleted;
        }

        public void ShowAd()
        {
            YG2.InterstitialAdvShow();
        }

        public void ShowRewardedAd()
        {
            YG2.RewardedAdvShow("reward", OnRewardAdClosed);
        }

        private void OnRewardAdClosed(string id)
        {
            OnRewardAdCompleted?.Invoke();
        }
        private void OnRewardAdClosed()
        {
            OnRewardAdCompleted?.Invoke();
        }
    }
#endif
}
