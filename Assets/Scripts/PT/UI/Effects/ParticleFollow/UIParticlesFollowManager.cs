using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace PT.UI.Effects.ParticleFollow
{
    #region Classes

    [Serializable]
    public class ParticleFollowInfoEditor : ParticleFollowInfo 
    {
        [Header("Editor Settings")]   
        [SerializeField] private int poolSize = 20;
        
        public int PoolSize => poolSize;
    } 
    
    [Serializable]
    public class ParticleFollowInfo 
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private ParticleFollowEnum particleFollowEnum;
        [Space]        
        [SerializeField, MinMaxSlider(0.1f, 100)] private Vector2 moveSpeed = new(1, 2);
        [Space]        
        [SerializeField] private bool slowlyMovesNearSpawn;
        [SerializeField, MinMaxSlider(0.1f, 100)] private Vector2 slowMoveSpeed = new(0.5f, 1);
        [SerializeField] private float slowMoveRadius;
        
        public GameObject Prefab => prefab;
        public ParticleFollowEnum ParticleFollowEnum => particleFollowEnum;
        public Vector2 MoveSpeed => moveSpeed;
        public bool SlowlyMovesNearSpawn => slowlyMovesNearSpawn;
        public Vector2 SlowMoveSpeed => slowMoveSpeed;
        public float SlowMoveRadius => slowMoveRadius;
    }
    
    class ParticleFollowPool
    {
        public ParticleFollowInfo ParticleFollowInfo { get; private set; }
        public ObjectPool<GameObject> Pool { get; private set; }

        public ParticleFollowPool(ParticleFollowInfo particleFollowInfo, ObjectPool<GameObject> pool)
        {
            ParticleFollowInfo = particleFollowInfo;
            Pool = pool;
        }
    }

    class ParticleFollow
    {
        public ParticleFollowEnum ParticleFollowEnum;
        public Transform Transf;
        public List<ParticleFollowPath> Paths = new();
        public Action TargetReachedAction;

        public ParticleFollow() { }
    }
    class ParticleFollowPath
    {
        public float Speed;
        public Vector3 Position;

        public ParticleFollowPath(float speed, Vector3 position)
        {
            Speed = speed;
            Position = position;
        }
    }

    #endregion

    public class UIParticlesFollowManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float reachedTargetDistance = 0.1f;
        // [SerializeField] private bool lerpMovement;
        [Space(20)]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas uiCanvas;
        [Space]
        [SerializeField] private Transform particleFollowParent;
        [Space(20)]
        [SerializeField] private SerializableKeyValue<FollowToTargetEnum, Transform> followTargetTransforms;
        [SerializeField] private ParticleFollowInfoEditor[] particleFollowInfos;
        
        [Inject] private SignalBus _signalBus;
        
        private Dictionary<ParticleFollowEnum, ParticleFollowPool> _particleFollowPoolsInfos = new();
        private List<ParticleFollow> _curParticleFollows = new();
        
        private RectTransform _uiCanvasRectTransform;
        
        private CancellationTokenSource _cts;

        private void Awake()
        {
            if (_uiCanvasRectTransform == null)
            {
                _uiCanvasRectTransform = uiCanvas.GetComponent<RectTransform>();
            }
            
            foreach (var particleFollowInfo in particleFollowInfos)
            {
                var particleFollowEnum = particleFollowInfo.ParticleFollowEnum;
                
                if (particleFollowInfo.Prefab == null)
                {
                    DebugManager.Log(DebugCategory.Effects, $"Prefab not assigned {particleFollowEnum}", LogType.Warning);
                    continue;
                }
                if (_particleFollowPoolsInfos.ContainsKey(particleFollowEnum))
                {
                    DebugManager.Log(DebugCategory.Effects, $"Already has pool for {particleFollowEnum}", LogType.Warning);
                    continue;
                }

                var pool = new ObjectPool<GameObject>(
                    () => Instantiate(particleFollowInfo.Prefab, particleFollowParent), 
                    defaultCapacity: particleFollowInfo.PoolSize);
                
                _particleFollowPoolsInfos.Add(particleFollowEnum, new(particleFollowInfo, pool));
            }

            _signalBus.Subscribe<GameEndedSignal>(FinishParticles);
        }

        private void Update()
        {
            HandleParticles();
        }
        private void HandleParticles()
        {
            for (int i = _curParticleFollows.Count - 1; i >= 0; i--)
            {
                var particleFollow = _curParticleFollows[i];
                particleFollow.Transf.position = Vector3.Lerp(particleFollow.Transf.position,
                    particleFollow.Paths[0].Position, particleFollow.Paths[0].Speed * Time.deltaTime);

                if (Vector3.Distance(particleFollow.Transf.position, particleFollow.Paths[0].Position) < reachedTargetDistance)
                {
                    particleFollow.Paths.RemoveAt(0);

                    if (particleFollow.Paths.Count == 0)
                    {
                        particleFollow.TargetReachedAction?.Invoke();

                        RemoveParticle(_curParticleFollows[i]);
                    }
                }
            }
        }

        public void InstantiateParticlesFollowTo(
            Vector2 spawnPosition,
            bool isWorldPosition,
            ParticleFollowEnum particleFollowEnum,
            FollowToTargetEnum followToTarget,
            Action targetReachedAction,
            int amount = 1)
        {
            _cts?.Cancel();
            _cts = new ();
            
            DebugManager.Log(DebugCategory.Effects, $"Instantiate particles: {particleFollowEnum}, amount: {amount}, target: {followToTarget}");

            InstantiateParticlesAsync(spawnPosition, isWorldPosition, particleFollowEnum, amount, followToTarget, targetReachedAction, _cts.Token).Forget();
        }

        private async UniTaskVoid InstantiateParticlesAsync(
            Vector2 spawnPosition,
            bool isWorldPosition,
            ParticleFollowEnum particleFollowEnum,
            int amount,
            FollowToTargetEnum followToTarget,
            Action targetReachedAction,
            CancellationToken ct)
        {
            var particleFollowInfo = _particleFollowPoolsInfos[particleFollowEnum].ParticleFollowInfo;
            Vector2 targetPosition = followTargetTransforms.Dictionary[followToTarget].position;
                
            for (int i = 0; i < amount; i++)
            {
                if (ct.IsCancellationRequested) return;

                CreateParticleFollow(particleFollowEnum, spawnPosition, isWorldPosition,
                    out ParticleFollow particleFollow, out Vector2 uiSpawnPosition);

                if (particleFollow.Transf == null) return;

                if (particleFollowInfo.SlowlyMovesNearSpawn)
                {
                    particleFollow.Paths.Add(new ParticleFollowPath(
                        Utils.GetRandomValue(particleFollowInfo.SlowMoveSpeed),
                        uiSpawnPosition + Utils.GetRandomVector(particleFollowInfo.SlowMoveRadius)));
                }

                particleFollow.Paths.Add(new ParticleFollowPath(
                    Utils.GetRandomValue(particleFollowInfo.MoveSpeed),
                    targetPosition));

                particleFollow.TargetReachedAction = targetReachedAction;
                _curParticleFollows.Add(particleFollow);

                await UniTask.Delay(TimeSpan.FromSeconds(Utils.GetRandomValue(0.01f, 0.05f)), cancellationToken: ct);
            }
        }

        public void FinishParticles()
        {
            _cts?.Cancel();

            while (_curParticleFollows.Count > 0)
            {
                _curParticleFollows[0].TargetReachedAction?.Invoke();
                RemoveParticle(_curParticleFollows[0]);
            }
        }

        private void CreateParticleFollow(
            ParticleFollowEnum particleFollowEnum,
            Vector2 spawnPosition,
            bool isWorldPosition,
            out ParticleFollow particleFollow,
            out Vector2 uiSpawnPosition)
        {
            particleFollow = new();
            uiSpawnPosition = Vector2.zero;

            Vector2 localUiPos;

            if (isWorldPosition)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(spawnPosition);
                if (screenPos.z <= 0) return;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _uiCanvasRectTransform,
                    screenPos,
                    uiCanvas.worldCamera,
                    out localUiPos
                );
            }
            else
            {
                // IMPORTANT: UI LOCAL SPACE, NO CONVERSION
                localUiPos = spawnPosition;
            }

            var particleObject = _particleFollowPoolsInfos[particleFollowEnum].Pool.Get();
            var rect = particleObject.GetComponent<RectTransform>();

            rect.SetParent(particleFollowParent, false);
            rect.anchoredPosition = localUiPos;

            particleFollow.Transf = rect;
            particleFollow.ParticleFollowEnum = particleFollowEnum;

            uiSpawnPosition = rect.position;
        }

        private void RemoveParticle(ParticleFollow particleFollow)
        {
            _particleFollowPoolsInfos[particleFollow.ParticleFollowEnum].Pool.Release(particleFollow.Transf.gameObject);

            _curParticleFollows.Remove(particleFollow);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
        }
    }
}