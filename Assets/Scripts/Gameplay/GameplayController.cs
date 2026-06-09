using System;
using Cysharp.Threading.Tasks;
using PT.GameplayAdditional.Input;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using PT.Tools.Windows;
using PT.UI.Windows;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class GameplayController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private RectTransform[] inputZoneUIs;
        [SerializeField] private Canvas canvas;
        
        [Inject] private GameConfig _gameConfig;
        [Inject] private SignalBus _signalBus;
        [Inject] private GameEndRewardWindow _gameEndRewardView;
        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private InputManager _inputManager;

        private bool _gameEnded;
        private int _otherBallsReachedFinishAmount;

        private void Awake()
        {
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }

        private void OnGameStarted()
        {
            _gameEnded = false;
            _otherBallsReachedFinishAmount = 0;
            
            _inputManager.OnClick += TryStartSlowMo;
            _inputManager.OnRelease += TryPerformPush;
        }
        
        private async void Victory()
        {
            if (_gameEnded) return;
            _gameEnded = true;
            
            DebugManager.Log(DebugCategory.Gameplay, "Victory");
            
            // _gameEndRewardView.SetCollected(_coinsEarnedThisGame);

            await UniTask.Delay(TimeSpan.FromSeconds(_gameConfig.GameEndDelay));
            
            _signalBus.Fire(new GameVictorySignal());
            await _windowsManager.Open<GameEndWindow>();
        }
        private async void GameOver()
        {
            DebugManager.Log(DebugCategory.Gameplay, "GameOver");
            
            // _gameEndRewardView.SetCollected(_coinsEarnedThisGame);
            
            _signalBus.Fire(new GameEndedSignal());
            await _windowsManager.Open<GameOverReviveWindow>();
            // await _windowsManager.Open<GameEndRewardWindow>();
        }
        
        private void TryStartSlowMo(Vector2 screenPosition)
        {
            if (IsInputInZone(screenPosition))
            {
                // ballsManager.StartSlowMo();
            }
        }
        private void TryPerformPush(Vector2 screenPosition)
        {
            if (IsInputInZone(screenPosition))
            {
                // ballsManager.DropBall();
            }
        }
        
        private bool IsInputInZone(Vector2 screenPosition)
        {
            foreach (var zone in inputZoneUIs)
            {
                var usedCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(zone, screenPosition, usedCam, out var localPoint);
                bool inside = zone.rect.Contains(localPoint);

                if (inside) return true;
            }

            return false;
        }
        
        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartedSignal>(OnGameStarted);
        }
    }
}