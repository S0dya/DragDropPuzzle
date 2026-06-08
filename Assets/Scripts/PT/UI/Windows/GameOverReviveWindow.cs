using Cysharp.Threading.Tasks;
using PT.Logic.Ads;
using PT.Logic.Dependency.Signals;
using PT.Logic.PersistentScene;
using PT.Tools.Sequences;
using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class GameOverReviveWindow : WindowBase
    {
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button skipButton;
        [Space]
        [SerializeField] private Sequencer[] sequencers;
        
        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private AdsManager _adsManager;
        [Inject] private SignalBus _signalBus;
        
        private void Awake()
        {
            reviveButton.onClick.AddListener(OnRevive);
            skipButton.onClick.AddListener(OnSkip);
        }

        private void OnRevive()
        {
            _adsManager.ShowRewardAd(RewardedAdCompleted);
        }
        private void OnSkip()
        {
            _windowsManager.CloseAll().Forget();
            _windowsManager.Open<GameEndWindow>().Forget();
        }
        
        private async void RewardedAdCompleted()
        {
            _signalBus.Fire(new GameReplaySignal());
            
            await _windowsManager.CloseAllFrom<GameEndWindow>();
        }
        
        protected override async UniTask OnOpen()
        {
            foreach (var sequencer in sequencers)
            {
                sequencer.Play().Forget();
            }
        }
    }
}