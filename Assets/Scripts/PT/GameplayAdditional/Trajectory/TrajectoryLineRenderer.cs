using System.Collections.Generic;
using UnityEngine;

namespace PT.GameplayAdditional.Trajectory
{
    public class TrajectoryLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        public void Draw(List<Vector2> points)
        {
            if (points == null || points.Count == 0)
            {
                Clear(); return;
            }

            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }

        public void Clear()
        {
            lineRenderer.positionCount = 0;
        }
    }
}