using System;
using PT.Logic.Dependency.Signals;
using PT.Logic.Save;
using PT.Tools.Debugging;
using PT.Tools.Other;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Score
{
    public class ScoreManager : IInitializable, IDisposable
    {
        public ReactiveProperty<int> CurrentScoreReactive { get; private set; } = new(0);
        public Subject<ValueChange> CurrentScoreChange { get; private set; } = new();
        public ReactiveProperty<int> HighestScoreReactive { get; private set; } = new(0); 

        [Inject] protected SignalBus _signalBus;
        [Inject] protected ScoreMultiplier _multiplier;

        public virtual void Initialize()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
            _signalBus.Subscribe<GameEndedSignal>(OnGameFinished);
        }
        
        public virtual void Init()
        {
            HighestScoreReactive.Value = (int)GameDataRegistry.Get(GameDataKey.HighestScore);
            HighestScoreReactive.Subscribe(val => GameDataRegistry.Set(GameDataKey.HighestScore, val) );
        }
        public void Dispose()
        {
            GameDataRegistry.Set(GameDataKey.HighestScore, HighestScoreReactive.Value);
        }
        
        private void OnGameStarted()
        {
            ResetScore();
        }

        private void OnGameFinished()
        {
            if (CurrentScoreReactive.Value > HighestScoreReactive.Value)
            {
                HighestScoreReactive.Value = CurrentScoreReactive.Value;
                
                _signalBus.Fire(new NewHighestScoreReachedSignal());
            }
        }
        
        public virtual void UpdateScore(int value)
        {
            var add = Mathf.RoundToInt(value * _multiplier.Current);
            
            DebugManager.Log(DebugCategory.Misc, $"Add Score : {value} (multip : {add} to : {CurrentScoreReactive.Value}");
            
            CurrentScoreChange.OnNext(new (add, ValueChangeType.Add));
            CurrentScoreReactive.Value += add;
        }
        
        protected virtual void ResetScore()
        {
            CurrentScoreReactive.Value = 0;
        }
    }
}