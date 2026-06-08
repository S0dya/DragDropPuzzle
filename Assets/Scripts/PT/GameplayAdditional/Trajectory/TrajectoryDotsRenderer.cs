using System.Collections.Generic;
using PT.Tools.Helper;
using UnityEngine;

namespace PT.GameplayAdditional.Trajectory
{
    public class TrajectoryDotsRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject dotPrefab;
        [SerializeField] private int poolSize = 30;
        [SerializeField, Min(0.1f)] private Vector2 dotMinSize;

        private readonly List<Transform> _dots = new();
        private Vector2 _dotMaxSize;

        private void Awake()
        {
            var prefabScale = dotPrefab.transform.localScale;
            _dotMaxSize = prefabScale;

            for (int i = 0; i < poolSize; i++)
            {
                var dot = Instantiate(dotPrefab, transform);
                dot.SetActive(false);
                _dots.Add(dot.transform);
            }
        }

        public void Draw(List<Vector2> points)
        {
            int count = Mathf.Min(points.Count, _dots.Count);

            for (int i = 0; i < count; i++)
            {
                _dots[i].position = points[i];
                _dots[i].localScale = Vector2.Lerp(dotMinSize, _dotMaxSize, (float)(count - i) / poolSize);

                if (!_dots[i].gameObject.activeSelf) _dots[i].SetActive(true);
            }

            for (int i = count; i < _dots.Count; i++)
            {
                if (_dots[i].gameObject.activeSelf) _dots[i].SetActive(false);
            }
        }

        public void Clear()
        {
            foreach (var dot in _dots) if (dot.gameObject.activeSelf) dot.SetActive(false);
        }
    }
}