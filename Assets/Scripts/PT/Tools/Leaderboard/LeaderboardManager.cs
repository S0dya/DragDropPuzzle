using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PT.Backend.Interfaces;
using PT.Backend.Types;
using PT.Logic.Dependency.Signals;
using PT.Logic.Save;
using PT.Tools.Debugging;
using UnityEngine;
using Zenject;

namespace PT.Tools.Leaderboard
{
    public class LeaderboardManager : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private LeaderboardConfig _leaderboardConfig;
        [Inject (Optional = true)] private ILeaderboardService _leaderboard;
        [Inject (Optional = true)] private IAuthentificationService _authentificationService;
        [Inject (Optional = true)] private IAnalyticsService _analyticsService;

		private int _currentRank = int.MaxValue; 

        public void Initialize()
        {
            _signalBus.Subscribe<NewHighestScoreReachedSignal>(OnGameNewRecordReached);
        }
        public void Dispose()
        {
            _signalBus.Unsubscribe<NewHighestScoreReachedSignal>(OnGameNewRecordReached);
        }

        public void SetNewScore(long value)
        {
            DebugManager.Log(DebugCategory.Leaderboards, $"Setting new score: {value}");
            
            _leaderboard.SetScore(value);
        }
        
        public async UniTask<(bool success, LeaderboardSnapshot snapshot)> GetTopEntries()
        {
            if (_authentificationService == null)
            {
                DebugManager.Log(DebugCategory.Errors, "No IAuthentificationService injected. Cannot fetch leaderboard.", LogType.Error);
                return (false, null);
            }
            
            if (!_authentificationService.IsSignedIn)
            {
                DebugManager.Log(DebugCategory.Leaderboards, "Player isn't signed in, trying to sign in...");
                
                await _authentificationService.SignIn();

                if (!_authentificationService.IsSignedIn) return (false, null);
            }

            DebugManager.Log(DebugCategory.Leaderboards, "Fetching TOP leaderboard entries");

            var snapshot = await _leaderboard.GetTop(_leaderboardConfig.TopAmount);
            CacheRank(snapshot.PlayerEntry.Rank);
            
            DebugManager.Log(DebugCategory.Leaderboards, $"Top fetched | Player rank: {snapshot.PlayerEntry.Rank} | Entries: {snapshot.Entries.Count}");
            return (true, snapshot);
        }
        public async UniTask<(bool success, LeaderboardSnapshot snapshot)> TryGetAroundPlayerEntries()
        {
            if (_authentificationService == null)
            {
                DebugManager.Log(DebugCategory.Errors, "No IAuthentificationService injected. Cannot fetch leaderboard.", LogType.Error);
                return (false, null);
            }
            
            DebugManager.Log(DebugCategory.Leaderboards, "Fetching AROUND PLAYER leaderboard");
            
            var snapshot = await _leaderboard.GetAroundPlayer(_leaderboardConfig.AroundPlayerAmount);
            bool newRankIsReached = snapshot.PlayerEntry.Rank < _currentRank;

            DebugManager.Log(DebugCategory.Leaderboards, $"Around fetched | New rank is Reached : {newRankIsReached}, | Entries: {snapshot.Entries.Count}");

            if (newRankIsReached)
            {
                _analyticsService?.Log(AnalyticsLogKeys.NewRankReached, new Dictionary<string, object>()
                {
                    { AnalyticsLogKeys.Rank, snapshot.PlayerEntry.Rank }
                });
            }
            
            CacheRank(snapshot.PlayerEntry.Rank);
            
            return (newRankIsReached, snapshot);
        }

        private void OnGameNewRecordReached() => SetNewScore((int)GameDataRegistry.Get(GameDataKey.HighestScore));
        
        private void CacheRank(int rank)
        {
            _currentRank = rank;
            GameDataRegistry.Set(GameDataKey.LeaderboardPlayerRank, rank);
        }
    }
}
