using PT.GameplayAdditional.Trigger;

namespace Gameplay.BallDrop.Balls.BallCollisionStrategy
{
    public interface IBallCollisionHandler
    {
        BallCollisionType Type { get; }
        void Handle(Ball ball, CollisionInfo info);
    }
}