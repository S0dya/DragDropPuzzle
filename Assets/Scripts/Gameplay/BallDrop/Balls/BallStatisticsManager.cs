using System;
using System.Collections.Generic;
using Gameplay.BallDrop.Datas;
using Gameplay.Session;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Balls
{
    public class BallStatisticsManager : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private GameSessionData _gameSessionData;

        private Dictionary<CharacterNameEnum, float> _ballFallTimes = new();
        private CharacterNameEnum? _currentLowestBall;
        private float _lowestBallStartTime;

        private void Awake()
        {
            _signalBus.Subscribe<NewLowestBallSignal>(OnNewLowestBall);
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<NewLowestBallSignal>(OnNewLowestBall);
            _signalBus.Unsubscribe<GameStartedSignal>(OnGameStarted);
        }

        private void OnGameStarted()
        {
            ResetStatistics();
        }
        
        private void OnNewLowestBall(NewLowestBallSignal signal)
        {
            if (_currentLowestBall != null)
            {
                float duration = Time.time - _lowestBallStartTime;
                _ballFallTimes[_currentLowestBall.Value] += duration;

                DebugManager.Log(DebugCategory.Gameplay, $"Ball {_currentLowestBall.Value} lowest for {duration:F2}s. Total: {_ballFallTimes[_currentLowestBall.Value]:F2}s");
            }

            _currentLowestBall = signal.Ball.Info.CharacterName;
            _lowestBallStartTime = Time.time;
        }

        private void ResetStatistics()
        {
            _currentLowestBall = null;
            _lowestBallStartTime = 0f;

            _ballFallTimes.Clear();
            foreach (var characterData in _gameSessionData.CharacterDatas)
                _ballFallTimes[characterData.CharacterName] = 0f;
            
            DebugManager.Log(DebugCategory.Gameplay, "Ball statistics reset");
        }

        public Dictionary<CharacterData, float> GetStatistics()
        {
            Dictionary<CharacterData, float> stats = new();

            foreach (var cd in _gameSessionData.CharacterDatas)
            {
                float totalTime = _ballFallTimes[cd.CharacterName];

                if (_currentLowestBall == cd.CharacterName) totalTime += Time.time - _lowestBallStartTime;

                stats[cd] = totalTime;
            }

            return stats;
        }
    }
}
