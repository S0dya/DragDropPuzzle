using System.Collections.Generic;
using UnityEngine;

namespace PT.GameplayAdditional.Trajectory
{
    public class TrajectoryCalculator
    {
        public void Calculate(Vector2 startPos, Vector2 startVelocity, Vector2 gravity, 
            int steps, float distance, float skipDistance, float maxDistance, 
            List<Vector2> outputPoints)
        {
            outputPoints.Clear();
            var pos = startPos;
            var vel = startVelocity;

            float traveled = 0f;
            for (int i = 0; i < steps + 5; i++)
            {
                vel += gravity * distance;
                var newPos = pos + vel * distance;

                traveled += Vector2.Distance(pos, newPos);
                pos = newPos;

                outputPoints.Add(pos);
            }
        }
    }
}