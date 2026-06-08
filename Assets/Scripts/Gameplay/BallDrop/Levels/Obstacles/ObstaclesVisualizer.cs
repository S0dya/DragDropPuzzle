using UnityEngine;

namespace Gameplay.BallDrop.Levels.Obstacles
{
    public class ObstaclesVisualizer : MonoBehaviour //ADAPT TO EDITOR SCRIPT
    {
        [SerializeField] private GameObject dotPrefab;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform parentTransform;

        private void Awake()
        {
            var obstacles = GetComponentsInChildren<MovingObstacle>(true);
            foreach (var obstacle in obstacles) Visualize(obstacle);
        }

        private void Visualize(MovingObstacle obstacle)
        {
            Vector2 startPos = obstacle.transform.position;

            var direction = Vector2.zero;
            switch (obstacle.MovementType)
            {
                case MovementTypeEnum.LeftRight:
                    direction = Vector2.right; break;
                case MovementTypeEnum.UpDown:
                    direction = Vector2.up; break;
                case MovementTypeEnum.DiagonalLeftDown:
                    direction = new Vector2(-1f, -1f).normalized; break;
                case MovementTypeEnum.DiagonalRightUp:
                    direction = new Vector2(1f, -1f).normalized; break;
                default: return;
            }

            if (obstacle.StartInInvertedDirection) direction = -direction;

            if (obstacle.MoveDistance > 0)
            {
                Vector2 endPos = startPos + direction * obstacle.MoveDistance;

                var dot1 = Instantiate(dotPrefab, startPos, Quaternion.identity, parentTransform);
                var dot2 = Instantiate(dotPrefab, endPos, Quaternion.identity, parentTransform);

                var line = Instantiate(linePrefab, parentTransform);
                var middle = (startPos + endPos) / 2f;
                line.transform.position = middle;

                var dir = endPos - startPos;
                float distance = dir.magnitude;
                line.transform.right = dir.normalized;
                line.transform.localScale = new Vector2(distance * 2 + distance / 4, line.transform.localScale.y);
            }
        }
    }
}