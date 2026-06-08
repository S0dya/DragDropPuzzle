using Gameplay.BallDrop.Balls;
using PT.GameplayAdditional.Cameras;
using PT.Logic.Dependency.Signals;
using Zenject;

namespace Gameplay.BallDrop.Camera
{
    public class CameraMoveController : CameraController
    {
        [Inject] private SignalBus _signalBus;

        private void Awake()
        {
            _signalBus.Subscribe<NewLowestBallSignal>(OnNewLowestBall);
        }
        
        private void OnNewLowestBall(NewLowestBallSignal signal)
        {
            SetTarget(signal.Ball.transform);
        }
    }
}