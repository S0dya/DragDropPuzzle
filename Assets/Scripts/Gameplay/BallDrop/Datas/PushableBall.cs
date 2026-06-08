using Gameplay.BallDrop.Balls;

namespace Gameplay.BallDrop.Datas
{
    public class PushableBall
    {
        public Ball Ball { get; private set; }

        public PushableBall(Ball ball)
        {
            Ball = ball;
        }

    }
}
