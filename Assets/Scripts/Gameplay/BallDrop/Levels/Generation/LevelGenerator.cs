using System.Collections.Generic;
using Gameplay.BallDrop.Configs;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using PT.Tools.Factories;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace Gameplay.BallDrop.Levels.Generation
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private Transform levelParent;
        [SerializeField] private LevelChunk finishLevelChunkPrefab;
        [SerializeField] private UnityEngine.Camera gameCamera;
        
        [Inject] private ILevelGenerationService _levelGenerationService;
        [Inject] private LevelSessionData _levelSessionData;
        [Inject] private IFactoryZenject _factoryZenject;
        [Inject] private SignalBus _signalBus;
        [Inject] private GameConfig _gameConfig;

        private int _currentChunkIndex;
        private float _currentYPosition;

        private LevelChunk[] _currentChunks;
        private LevelChunk _finishChunk;

        private bool _songIsEnding;
        
        private void Awake()
        {
            _signalBus.Subscribe<GameSongEndingSignal>(OnSongEnding);
        }
        
        public void Init()
        {
            GenerateLevel();

            SetChunkVisibility();
        }

        private void GenerateLevel()
        {
            var buildResult = _levelGenerationService.GenerateLevel(levelParent);

            var levelSizeTriggersYs = new Dictionary<float, LevelSizeChangingTrigger>();
            foreach (var trigger in buildResult.SizeChangingTriggers)
            {
                levelSizeTriggersYs.Add(trigger.transform.position.y, trigger);
            }
            _levelSessionData.InitLevelSizeTriggers(levelSizeTriggersYs);

            _currentChunks = buildResult.LevelChunks;
            
            var finishChunk = _factoryZenject.InstantiateObject(finishLevelChunkPrefab.gameObject, levelParent).GetComponent<LevelChunk>();
            finishChunk.transform.position = new Vector2(0, buildResult.LastYPosition - Mathf.Abs(finishChunk.Height*2));
            _finishChunk = finishChunk;
        }
        private void SetChunkVisibility()
        {
            _songIsEnding = false;
            
            foreach (var chunk in _currentChunks) chunk.Toggle(false);
            for (int i = 0; i < _gameConfig.LevelChunksInitialEnabled; i++) _currentChunks[i].Toggle(true);
            
            _currentChunkIndex = _gameConfig.LevelChunksInitialEnabled;
            _currentYPosition = _gameConfig.LevelChunksOpeningYOffset;
        }

        private void Update()
        {
            if (gameCamera.transform.position.y < _currentYPosition)
            {
                if (_songIsEnding || _currentChunks == null) return;
                if (_currentChunkIndex >= _currentChunks.Length) return;

                int chunkAboveIndexToDisable = _currentChunkIndex - _gameConfig.LevelChunksTopDisableStep;
                if (chunkAboveIndexToDisable > 0) _currentChunks[chunkAboveIndexToDisable].Toggle(false);
                
                _currentChunks[_currentChunkIndex].Toggle(true);
                _currentChunkIndex++;
                
                DebugManager.Log(DebugCategory.Gameplay, $"new chunk opened. current chunk index {_currentChunkIndex} | next position {_currentYPosition}");

                if (_currentChunkIndex < _currentChunks.Length)
                {
                    _currentYPosition = _currentChunks[_currentChunkIndex].transform.position.y + Mathf.Abs(_gameConfig.LevelChunksOpeningYOffset);
                }
            }
        }

        private void OnSongEnding()
        {
            if (_songIsEnding) return;
            _songIsEnding = true;
            
            _finishChunk.transform.position = new Vector2(0, 
                _currentChunks[_currentChunkIndex].transform.position.y);
        }
    }
}