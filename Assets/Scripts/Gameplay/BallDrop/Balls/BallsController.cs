using System.Collections.Generic;
using Gameplay.BallDrop.Balls.BallCollisionStrategy;
using Gameplay.BallDrop.UI;
using Gameplay.Session;
using NaughtyAttributes;
using PT.GameplayAdditional.Trigger;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Balls
{
    public class BallsController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Ball ballPrefab;
        [SerializeField][MinMaxSlider(-10, 10)] private Vector2 randomSpawnDistanceRange;
        
        [Inject] private GameConfig _gameConfig;
        [Inject] private GameSessionData _gameSessionData;
        [Inject] private SignalBus _signalBus;
        [Inject] private List<IBallCollisionHandler> _ballCollisionHandlers;

        [Inject] private BallsNamesController _ballsNamesController;
        
        private bool _ballsAreOutOfIntroBox = false;
        
        private readonly List<Ball> _activeBalls = new();

        private Ball _lowestBall;

        private void Awake()
        {
            _signalBus.Subscribe<BallReachedFinishSignal>(OnBallReachedFinish);
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }
        
        public void Init()
        {
            SpawnBalls();
        }
        
        
        private void FixedUpdate()
        {
            if (!_ballsAreOutOfIntroBox) return;
            
            foreach (var ball in _activeBalls)
            {
                if (ball == null) continue;
                
                if (_lowestBall == null || ball.transform.position.y < _lowestBall.transform.position.y - Mathf.Abs(_gameConfig.LowestBallYOffset))
                {
                    _lowestBall = ball;
                    
                    _signalBus.Fire(new NewLowestBallSignal(_lowestBall));
                }
            }
        }
        
        private void SpawnBalls()
        {
            foreach (var characterData in _gameSessionData.CharacterDatas)
            {
                var spawnPosition = spawnPoint.position + (Vector3)Utils.GetRandomVector(randomSpawnDistanceRange.GetRandomValue());
                var ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity, transform);
                ball.Init(characterData);
                _activeBalls.Add(ball);
                
                ball.OnCollision += OnBallCollision;
            }
            
            _ballsNamesController.Init(_activeBalls.ToArray());
        }

        private void OnBallReachedFinish(BallReachedFinishSignal signal)
        {
            if (signal.Ball != null && _activeBalls.Contains(signal.Ball))
            {
                if (_lowestBall == signal.Ball && !_lowestBall.IsPlayer) _lowestBall = null;
                _activeBalls.Remove(signal.Ball);
            }
        }

        private void OnGameStarted()
        {
            _ballsAreOutOfIntroBox = true;
        }
        
        private void OnBallCollision(Ball ball, CollisionInfo collisionInfo, BallCollisionType collisionType)
        {
            // DebugManager.Log(DebugCategory.Gameplay, $"Ball collided with {collisionType}");
            
            foreach (var handler in _ballCollisionHandlers)
            {
                if (handler.Type == collisionType)
                {
                    handler.Handle(ball, collisionInfo);
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var ball in _activeBalls)
            {
                if (ball == null) continue;
                UnsubFromBall(ball);
            }
            
            _signalBus.Unsubscribe<BallReachedFinishSignal>(OnBallReachedFinish);
        }

        private void UnsubFromBall(Ball ball) => ball.OnCollision -= OnBallCollision;
    }
}