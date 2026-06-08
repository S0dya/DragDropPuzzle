using System;
using System.Linq;
using PT.Backend.Interfaces;
using PT.Backend.Types;
using PT.Logic.Dependency.Signals;
using Zenject;

namespace Gameplay.Analytics
{
    public class AnalyticsEventsListener : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private IAnalyticsService _analytics;
        [Inject] private RunState _run;

        public void Initialize()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameEndedSignal>(OnGameEnded);
            _signalBus.Subscribe<GameVictorySignal>(OnGameVictory);

            _signalBus.Subscribe<ShowAdSignal>(OnShowAd);
            
            _signalBus.Subscribe<GameAddCoinsSignal>(OnMoneyEarned);
            _signalBus.Subscribe<GameSpendCoinsSignal>(OnMoneySpent);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Unsubscribe<GameEndedSignal>(OnGameEnded);
            _signalBus.Unsubscribe<GameVictorySignal>(OnGameVictory);
            _signalBus.Unsubscribe<ShowAdSignal>(OnShowAd);
            
            _signalBus.Unsubscribe<GameAddCoinsSignal>(OnMoneyEarned);
            _signalBus.Unsubscribe<GameSpendCoinsSignal>(OnMoneySpent);
        }

        private void OnGameStarted()
        {
            _run.Reset();

        }

        private void OnMoneyEarned(GameAddCoinsSignal s)
        {
            _run.TotalMoneyEarned += s.Amount;
        }

        private void OnMoneySpent(GameSpendCoinsSignal s)
        {
            _run.TotalMoneySpent += s.Amount;
        }

        private void OnShowAd()
        {
            _run.AdsShownCount++;
        }

        private void OnGameVictory()
        {
            _run.IsVictory = true;
        }

        private void OnGameEnded()
        {
            _analytics.Log(AnalyticsLogKeys.RunEnded, new()
            {
                { AnalyticsLogKeys.AverageDecisionTime, _run.DecisionTimes.Sum() / Math.Max(1, _run.DecisionTimes.Count) },
                { AnalyticsLogKeys.AdsShown, _run.AdsShownCount },
                { AnalyticsLogKeys.Victory, _run.IsVictory },

                { AnalyticsLogKeys.MoneyEarned, _run.TotalMoneyEarned },
                { AnalyticsLogKeys.MoneySpent, _run.TotalMoneySpent },
                { AnalyticsLogKeys.MoneyDelta, _run.TotalMoneyEarned - _run.TotalMoneySpent }
            });
        }
    }
}
