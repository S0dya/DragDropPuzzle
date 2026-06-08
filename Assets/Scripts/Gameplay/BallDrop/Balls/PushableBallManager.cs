using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BallDrop.Datas;
using PT.GameplayAdditional.Input;
using PT.GameplayAdditional.Trajectory;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using PT.Tools.TimeRelated;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Balls
{
    public class PushableBallManager : MonoBehaviour
    {
        [Inject] private InputManager _inputManager;
        [Inject] private TimeManager _timeManager;
        [Inject] private GameConfig _gameConfig;
        [Inject] private SignalBus _signalBus;

        [SerializeField] private TrajectoryDotsRenderer trajectoryLineRenderer;
        [SerializeField] private UnityEngine.Camera cam;

        private PushableBall _currentPushableBall;
        private bool _isPushing;
        private bool _isSlowMoActive;
        private Vector3 _pushDirection;
        private float _pushStartTime;
        private float _rechargeStartTime;

        private const string SlowMoKey = "PushableBallSlowMo";

        public event Action<PushableBall> OnPushableBallChanged;

        public ReactiveProperty<float> PushProgress { get; private set; } = new(1);

        private void Awake()
        {
            _signalBus.Subscribe<NewLowestBallSignal>(OnNewLowestBall);
        }

        private void Start()
        {
            _inputManager.OnClick += OnPointerDown;
            _inputManager.OnDrag += OnPointerDrag;
            _inputManager.OnRelease += OnPointerUp;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<NewLowestBallSignal>(OnNewLowestBall);
            
            _inputManager.OnClick -= OnPointerDown;
            _inputManager.OnDrag -= OnPointerDrag;
            _inputManager.OnRelease -= OnPointerUp;
        }

        private void OnNewLowestBall(NewLowestBallSignal signal)
        {
            if (_isPushing)
            {
                // Ball switched while being pushed - cancel the push
                CancelPush();
            }

            SetPushableBall(signal.Ball);
        }

        private void SetPushableBall(Ball ball)
        {
            ClearTrajectory();

            if (ball == null)
            {
                DebugManager.Log(DebugCategory.Gameplay, "Pushable ball set to null");
                _currentPushableBall = null;
                OnPushableBallChanged?.Invoke(null);
                return;
            }

            _currentPushableBall = new PushableBall(ball);
            DebugManager.Log(DebugCategory.Gameplay, $"Pushable ball set to {ball.Info.CharacterName}");
            OnPushableBallChanged?.Invoke(_currentPushableBall);
        }

        private void OnPointerDown(Vector2 screenPosition)
        {
            if (_currentPushableBall == null || _isPushing || PushProgress.Value < 1) return;
            
            DebugManager.Log(DebugCategory.Gameplay, $"Starting Push at {screenPosition} " +
                                                     $"Ball null: {_currentPushableBall == null} " +
                                                     $"IsPushing: {_isPushing} " +
                                                     $"Progress: {PushProgress.Value} ");

            StartSlowMo();
            _isPushing = true;
            _pushStartTime = Time.time;
        }

        private void OnPointerDrag(Vector2 screenPosition)
        {
            if (!_isPushing || _currentPushableBall == null) return;

            Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane));
            _pushDirection = (worldPosition - _currentPushableBall.Ball.transform.position).normalized;

            DebugManager.Log(DebugCategory.Gameplay, $"Push direction updated: {_pushDirection}");
            UpdateTrajectory();
        }

        private void OnPointerUp(Vector2 screenPosition)
        {
            if (!_isPushing) return;

            DebugManager.Log(DebugCategory.Gameplay, $"Push released at {screenPosition}");
            ExecutePush();
        }

        private void StartSlowMo()
        {
            _isSlowMoActive = true;
            _timeManager.RequestTimeScale(SlowMoKey, _gameConfig.PushSlowMoScale, _gameConfig.PushSlowMoInDuration, 0);
            DebugManager.Log(DebugCategory.Gameplay, $"Slow-mo started: scale={_gameConfig.PushSlowMoScale}");
        }

        private void StopSlowMo()
        {
            _isSlowMoActive = false;
            _timeManager.RemoveRequest(SlowMoKey);
            DebugManager.Log(DebugCategory.Gameplay, "Slow-mo stopped");
        }

        private void ExecutePush()
        {
            if (_currentPushableBall == null)
            {
                CancelPush();
                return;
            }

            StopSlowMo();

            // Apply push force to the ball using its Push method
            _currentPushableBall.Ball.Push(_pushDirection, _gameConfig.PushPower);

            DebugManager.Log(DebugCategory.Gameplay, $"Push executed with direction {_pushDirection} and power {_gameConfig.PushPower}");
            ClearTrajectory();

            _isPushing = false;
            PushProgress.Value = 0;
            _rechargeStartTime = Time.time;
        }

        private void CancelPush()
        {
            DebugManager.Log(DebugCategory.Gameplay, "Push cancelled");
            StopSlowMo();
            ClearTrajectory();
            _isPushing = false;
            PushProgress.Value = 0;
            _rechargeStartTime = Time.time;
        }

        private void UpdateTrajectory()
        {
            if (_currentPushableBall == null) return;

            Rigidbody2D ballRigidbody = _currentPushableBall.Ball.GetComponent<Rigidbody2D>();
            List<Vector2> trajectoryPoints = SimulateTrajectory(
                _currentPushableBall.Ball.transform.position,
                _pushDirection * _gameConfig.PushPower,
                ballRigidbody.mass,
                ballRigidbody.linearDamping
            );

            trajectoryLineRenderer.Draw(trajectoryPoints);
        }

        private List<Vector2> SimulateTrajectory(Vector3 startPosition, Vector3 initialForce, float mass, float drag)
        {
            List<Vector2> points = new List<Vector2>();

            Vector3 position = startPosition;
            Vector3 velocity = initialForce / mass;

            for (int i = 0; i < _gameConfig.TrajectoryPoints; i++)
            {
                points.Add(position);

                // Apply physics simulation
                velocity += Physics.gravity * _gameConfig.TrajectoryTimeStep;
                velocity *= (1f - drag * _gameConfig.TrajectoryTimeStep);
                position += velocity * _gameConfig.TrajectoryTimeStep;
            }

            return points;
        }

        private void ClearTrajectory()
        {
            if (trajectoryLineRenderer != null)
            {
                trajectoryLineRenderer.Clear();
            }
        }

        private void Update()
        {
            if (PushProgress.Value < 1)
            {
                // Update recharge progress after push
                float rechargeProgress = (Time.time - _rechargeStartTime) / _gameConfig.PushCooldownDuration;
                rechargeProgress = Mathf.Clamp01(rechargeProgress);
                PushProgress.Value = rechargeProgress;
            }
        }
    }
}
