using UnityEngine;

namespace PT.Logic.Configs
{
    [CreateAssetMenu(menuName = "Configs/AdConfig", fileName = "AdConfig")]
    public class AdConfig : ScriptableObject
    {
        [SerializeField] private float breakFromAdsDuration = 60;
        [SerializeField] private float bannerHeightPx = 100;
        [SerializeField] private bool bannerEnabled = true;
        
        [Header("Google Play")]
        [SerializeField] private string gameIdAndroid;
        [SerializeField] private string interstitialVideoAndroid = "Interstitial_Android";
        [SerializeField] private string rewardedVideoAndroid = "Rewarded_Android";
        [SerializeField] private string bannerAndroid = "Banner_Android";
        
        [Header("iOS")]
        [SerializeField] private string gameIdIOS;
        [SerializeField] private string interstitialVideoIOS = "Interstitial_iOS";
        [SerializeField] private string rewardedVideoIOS = "Rewarded_iOS";
        [SerializeField] private string bannerIOS = "Banner_iOS";
        
        public float BreakFromAdsDuration => breakFromAdsDuration;
        public float BannerHeightPx => bannerHeightPx;
        public bool BannerEnabled => bannerEnabled;
        
#if UNITY_IOS
        public string GameId => gameIdIOS;
        public string InterstitialVideo => interstitialVideoIOS;
        public string RewardedVideo => rewardedVideoIOS;
        public string Banner => bannerIOS;
#else
        public string GameId => gameIdAndroid;
        public string InterstitialVideo => interstitialVideoAndroid;
        public string RewardedVideo => rewardedVideoAndroid;
        public string Banner => bannerAndroid;
#endif
    }
}