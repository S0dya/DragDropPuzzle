using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Datas;
using Gameplay.BallDrop.UI;
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
        [SerializeField] private Transform statsContainer;
        [SerializeField] private StatisticsView statItemPrefab;

        [Inject (Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private AdsManager _adsManager;
        [Inject] private SaveManager _saveManager;
        [Inject] private SignalBus _signalBus;
        [Inject] private BallStatisticsManager _ballStatisticsManager;
        [Inject] private CurrencyManager _currencyManager;
        [Inject] private GameConfig _gameConfig;

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

            DisplayStatistics();
            
            _currencyManager.Add(CurrencyType.Gold, _gameConfig.GoldAmountForLevelFinish);
            
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

        private void DisplayStatistics()
        {
            foreach (Transform child in statsContainer) Destroy(child.gameObject);

            Dictionary<CharacterData, float> stats = _ballStatisticsManager.GetStatistics();

            float totalTime = 0f;
            foreach (var kvp in stats) totalTime += kvp.Value;

            var sortedStats = stats.OrderByDescending(kvp => kvp.Value).ToList();

            foreach (var kvp in sortedStats)
            {
                var statItem = Instantiate(statItemPrefab, statsContainer);

                float percent = totalTime > 0 ? kvp.Value / totalTime : 0f;

                statItem.SetData(kvp.Key, percent);
            }
        }
    }
}