using PT.GameplayAdditional.Trigger;
using PT.Logic.Dependency.Signals;
using Zenject;

namespace Gameplay.BallDrop.Balls.BallCollisionStrategy
{
    public class FinishCollisionHandler : IBallCollisionHandler
    {
        public BallCollisionType Type { get; private set; } = BallCollisionType.Finish;

        [Inject] private SignalBus _signalBus; 
        
        public void Handle(Ball ball, CollisionInfo info)
        {
            _signalBus.Fire(new BallReachedFinishSignal(ball));
        }
    }
}