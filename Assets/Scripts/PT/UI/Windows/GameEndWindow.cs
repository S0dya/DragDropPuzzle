using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using PT.Logic.Ads;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Logic.PersistentScene;
using PT.Logic.Save;
using PT.Tools.CurrencyRelated;
using PT.Tools.Helper;
using PT.Tools.Leaderboard.Game;
using PT.Tools.Sequences;
using PT.Tools.Windows;
using PT.UI.Windows.Payloads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class GameEndWindow : WindowBase
    {
        [SerializeField] private Button nextLvlButton;
        [SerializeField] private Button menuButton;
        [Space]
        [SerializeField] private Sequencer[] starsSequencers;

        [Inject (Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private AdsManager _adsManager;
        [Inject] private SaveManager _saveManager;
        [Inject] private SignalBus _signalBus;

        private CancellationTokenSource _openCts;
        
        private void Awake()
        {
            _signalBus.Subscribe<GameVictorySignal>(OnGameVictory);
            _signalBus.Subscribe<NewHighestScoreReachedSignal>(OnGameNewRecordReached);
            
            nextLvlButton.onClick.AddListener(OnNextLevel);
            menuButton.onClick.AddListener(OnMenu);
        }

        private void OnNextLevel()
        {
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Game),
            }).Forget();
            
            _adsManager.ShowAd();
        }
        private void OnMenu()
        {
            _saveManager.Save();
            _adsManager.ShowAd();
            
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Menu),
            }).Forget();
        }
        private async void OnDynamicLeaderboard(Action action)
        {
            await _windowsManager.CloseAllFrom<GameEndWindow>();

            var payload = new LeaderboardDynamicPayload()
            {
                OnNext = action,
            };
            _windowsManager.Open<LeaderboardDynamicView>(payload).Forget();
        }
        
        private void OnGameVictory()
        {
            this.SetActive(true);
        }
        
        private void OnGameNewRecordReached()
        {
            nextLvlButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveAllListeners();
            
            nextLvlButton.onClick.AddListener(() => OnDynamicLeaderboard(OnNextLevel));
            menuButton.onClick.AddListener(() => OnDynamicLeaderboard(OnMenu));
        }

        protected override async UniTask OnOpen()
        {
            _openCts?.Cancel();
            _openCts = new CancellationTokenSource();

            try
            {
                foreach (var star in starsSequencers)
                {
                    if (star == null) continue;
                    await star.Play().AttachExternalCancellation(_openCts.Token);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}