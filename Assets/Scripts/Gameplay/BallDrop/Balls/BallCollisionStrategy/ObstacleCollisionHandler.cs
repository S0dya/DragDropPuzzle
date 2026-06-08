using PT.GameplayAdditional.Trigger;
using PT.Logic.Dependency.Signals;

namespace Gameplay.BallDrop.Balls.BallCollisionStrategy
{
    public class ObstacleCollisionHandler : IBallCollisionHandler
    {
        public BallCollisionType Type { get; private set; } = BallCollisionType.Obstacle;

        public void Handle(Ball ball, CollisionInfo info)
        {
            
        }
    }
}