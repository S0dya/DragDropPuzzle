using System.Collections.Generic;
using Gameplay.BallDrop.Levels;
using PT.GameplayAdditional.Trigger;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Balls.BallCollisionStrategy
{
    public class LevelTriggerCollisionHandler : IBallCollisionHandler
    {
        public BallCollisionType Type { get; private set; } = BallCollisionType.LevelBoard;

        [Inject] private LevelSessionData _levelSessionData; 
        
        public void Handle(Ball ball, CollisionInfo info)
        {
            // var closestPair = _levelSessionData.LevelSizeTriggers
            //     .Select(kvp => new KeyValuePair<TargetTrigger, float>(kvp.Key, Vector2.Distance(info.CollisionPoint, kvp.Key.transform.position)))
            //     .OrderBy(kvp => kvp.Value)
            //     .FirstOrDefault();

            var closestPair = new KeyValuePair<float, LevelSizeChangingTrigger>(float.MaxValue, null);
            var ballY = ball.transform.position.y;
            float currentDistance;
            
            foreach (var kvp in _levelSessionData.LevelSizeTriggers)
            {
                currentDistance = Mathf.Abs(kvp.Key - ballY);
                
                if (currentDistance < closestPair.Key)
                {
                    closestPair = new (currentDistance, kvp.Value);
                }
            }

            var size = closestPair.Value.ScaleSize;
            ball.ChangeSize(new (size, size));
        }
    }
}