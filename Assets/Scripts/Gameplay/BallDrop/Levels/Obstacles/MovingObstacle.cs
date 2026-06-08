using DG.Tweening;
using UnityEngine;

namespace Gameplay.BallDrop.Levels.Obstacles
{
    public enum MovementTypeEnum 
    {
        None, LeftRight, DiagonalLeftDown, UpDown, DiagonalRightUp
    }
    
    public class MovingObstacle : MonoBehaviour
    {
        [SerializeField] private MovementTypeEnum movementType = MovementTypeEnum.None;
        [SerializeField] private float moveDistance = 2f;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private bool startInInvertedDirection = false;

        public MovementTypeEnum MovementType => movementType;
        public float MoveDistance => moveDistance;
        public bool StartInInvertedDirection => startInInvertedDirection;
        
        
        private Vector2 _startPos;
        private Tween _moveTween;

        private void OnEnable()
        {
            _startPos = transform.position;
            StartMovement();
        }

        private void OnDisable()
        {
            _moveTween?.Kill();
            transform.position = _startPos;
        }

        private void StartMovement()
        {
            Vector2 direction = Vector2.zero;

            switch (movementType)
            {
                case MovementTypeEnum.None: return;
                case MovementTypeEnum.LeftRight:
                    direction = Vector2.right;
                    break;
                case MovementTypeEnum.UpDown:
                    direction = Vector2.up;
                    break;
                case MovementTypeEnum.DiagonalLeftDown:
                    direction = new Vector2(-1f, -1f).normalized;
                    break;
                case MovementTypeEnum.DiagonalRightUp:
                    direction = new Vector2(1f, -1f).normalized;
                    break;
            }

            if (startInInvertedDirection)
                direction *= -1f;

            var endPos = _startPos + (direction * moveDistance);

            _moveTween = transform.DOMove(endPos, moveDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}