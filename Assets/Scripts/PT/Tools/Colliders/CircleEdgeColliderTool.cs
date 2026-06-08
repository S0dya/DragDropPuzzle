using UnityEngine;

namespace PT.Tools.Colliders
{
    [ExecuteAlways]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class CircleEdgeColliderTool : MonoBehaviour
    {
        [Min(0.1f)] [SerializeField] private float radius = 5f;
        [Range(3, 256)] [SerializeField] private int segments = 64;
        [SerializeField] private bool autoCloseLoop = true;

        public void Generate()
        {
            var edge = GetComponent<EdgeCollider2D>();

            int pointCount = autoCloseLoop ? segments + 1 : segments;
            var points = new Vector2[pointCount];

            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;

                points[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            }

            if (autoCloseLoop) points[segments] = points[0];

            edge.points = points;
        }

        private void Start()
        {
            if (Application.isPlaying) Destroy(this);
        }
    }
}