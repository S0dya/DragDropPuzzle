using System;
using System.Collections;
using Gameplay.BallDrop.Datas;
using PT.GameplayAdditional.Trigger;
using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Balls
{
    public enum BallCollisionType
    {
        Obstacle,
        LevelBoard,
        Finish,
    }
    
    public class Ball : MonoBehaviour
    {
        [SerializeField] private BallView ballView;
        [SerializeField] private SerializableKeyValue<BallCollisionType, TargetTrigger> typeTriggers;
        [Space]
        [SerializeField] private float stuckOffset = 0.01f;
        [SerializeField] private float stuckTimer = 3;
        [SerializeField] private Vector2 stuckScale = new(0.1f, 0.1f);
        
        public event Action<Ball, CollisionInfo, BallCollisionType> OnCollision;
        
        public CharacterData Info { get; private set; }
        public bool IsPlayer { get; private set; }

        private Vector2 _lastPosition;
        private Coroutine _stuckCoroutine; 

        private void Start()
        {
            typeTriggers.Dictionary[BallCollisionType.LevelBoard].OnTriggered += OnLevelBoardEnter;
            typeTriggers.Dictionary[BallCollisionType.Obstacle].OnTriggered += OnObstacleEnter;
            typeTriggers.Dictionary[BallCollisionType.Finish].OnTriggered += OnFinishEnter;
        }
        private void OnDestroy()
        {
            typeTriggers.Dictionary[BallCollisionType.LevelBoard].OnTriggered -= OnLevelBoardEnter;
            typeTriggers.Dictionary[BallCollisionType.Obstacle].OnTriggered -= OnObstacleEnter;
            typeTriggers.Dictionary[BallCollisionType.Finish].OnTriggered -= OnFinishEnter;
        }

        public void Init(CharacterData characterData)
        {
            Info = characterData;
            
            ballView.SetInfo(characterData);
        }

        private void FixedUpdate()
        {
            if (Vector2.Distance(transform.position, _lastPosition) < stuckOffset)
            {
                if (_stuckCoroutine == null) 
                    _stuckCoroutine = StartCoroutine(StuckTimerCoroutine());
            }
            else
            {
                if (_stuckCoroutine != null)
                {
                    StopCoroutine(_stuckCoroutine);
                    _stuckCoroutine = null;
                }
            }

            _lastPosition = transform.position;
        }
        
        public void Push(Vector3 direction, float power)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(direction * power, ForceMode2D.Impulse);
        }

        public void ChangeSize(Vector2 size)
        {
            transform.localScale = size;
        }
        
        private void OnObstacleEnter(CollisionInfo collisionInfo)
        {
            InvokeCollided(collisionInfo, BallCollisionType.Obstacle);
        }
        private void OnFinishEnter(CollisionInfo collisionInfo)
        {
            InvokeCollided(collisionInfo, BallCollisionType.Finish);
        }
        private void OnLevelBoardEnter(CollisionInfo collisionInfo)
        {
            InvokeCollided(collisionInfo, BallCollisionType.LevelBoard);
        }

        private void InvokeCollided(CollisionInfo collisionInfo, BallCollisionType type) => OnCollision?.Invoke(this, collisionInfo, type);

        private IEnumerator StuckTimerCoroutine()
        {
            yield return new WaitForSeconds(stuckTimer);

            transform.localScale = stuckScale;
        }
    }
}