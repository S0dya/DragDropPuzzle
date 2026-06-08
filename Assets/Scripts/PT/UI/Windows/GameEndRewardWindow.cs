using Cysharp.Threading.Tasks;
using PT.Logic.Ads;
using PT.Tools.CurrencyRelated;
using PT.Tools.RewWheel;
using PT.Tools.Sequences;
using PT.Tools.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class GameEndRewardWindow : WindowBase
    {
        [SerializeField] private Button rewardWheelButton;
        [SerializeField] private CanvasGroup rewardWheelButtonCg;
        [SerializeField] private Button skipButton;
        [Space]
        [SerializeField] private TextMeshProUGUI collectedCoinsText;
        [Space]
        [SerializeField] private Sequencer[] sequencers;
        [Space]
        [SerializeField] private RewardWheel rewardWheel; 
        
        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private AdsManager _adsManager;
        [Inject] private CurrencyManager _currencyManager;
        
        private int _collected;
        
        private void Awake()
        {
            skipButton.onClick.AddListener(OnSkip);
            rewardWheelButton.onClick.AddListener(OnRewardWheelButton);
        }

        public void SetCollected(int collected)
        {
            _collected = collected;
        }

        private void OnSkip()
        {
            rewardWheel.StopSpinning();
            
            _windowsManager.CloseAll().Forget();
            _windowsManager.Open<GameEndWindow>().Forget();
        }
        
        private void OnRewardWheelButton()
        {
            _adsManager.ShowRewardAd(OnRewardAdWatched);
        }
        
        private void OnRewardAdWatched()
        {
            var reward = rewardWheel.StopSpinningGetReward();

            _currencyManager.Add(CurrencyType.Gold, reward - _collected);
            
            rewardWheelButtonCg.interactable = false;
            rewardWheelButtonCg.blocksRaycasts = false;
        }
        
        protected override async UniTask OnOpen()
        {
            foreach (var sequencer in sequencers)
            {
                sequencer.Play().Forget();
            }
            
            collectedCoinsText.text = $"{_collected}";
            rewardWheel.StartSpinning(_collected);
        }
    }
}