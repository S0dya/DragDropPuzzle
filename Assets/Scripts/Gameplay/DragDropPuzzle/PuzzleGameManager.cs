using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.DragDropPuzzle.Configs;
using PT.Logic.Configs;
using PT.Logic.PersistentScene;
using PT.Logic.ProjectContext;
using PT.Tools.Factories;
using PT.Tools.Windows;
using PT.UI.Windows;
using UnityEngine;
using Zenject;

namespace Gameplay.DragDropPuzzle
{
    public class PuzzleGameManager : MonoBehaviour
    {
        [SerializeField] private Transform levelParent;
        
        [Inject] private PuzzleLevelConfig _levelConfig;
        [Inject] private HandItemsView _handItemsView;
        [Inject] private ItemsManager _itemsManager;
        [Inject] private ParticleEffectPlayer _particleEffectPlayer;
        [Inject] private HintManager _hintManager;
        [Inject] private ProgressIndicator _progressIndicator;
        [Inject] private AudioManager _audioManager;
        [Inject] private GameSessionData _gameSessionData;
        [Inject] private ItemsConfig _itemsConfig;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private IFactoryZenject _factoryZenject;
        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        
        private PuzzleLevelData _currentLevel;

        private void Start()
        {
            LoadLevel();
        }
        private void LoadLevel()
        {
            _currentLevel = _levelConfig.GetLevel(_gameSessionData.CurrentLevel);
            
            if (_currentLevel == null)
            {
                Debug.LogError($"Level {_gameSessionData.CurrentLevel} not found!");
                return;
            }

            SetupLevel();
        }
        private void SetupLevel()
        {
            var level = _factoryZenject.InstantiateObject(_currentLevel.Level.gameObject, levelParent).GetComponent<PuzzleLevel>();

            var levelItemDatas = new List<ItemData>();
            foreach (var settableItem in level.SettableItems)
            {
                if (_itemsConfig.TryGetItemByType(settableItem.ItemType, out var itemData))
                {
                    levelItemDatas.Add(itemData);
                }
            }
            
            _handItemsView.SetItems(levelItemDatas);

            _progressIndicator.Initialize(levelItemDatas.Count);

            _itemsManager.OnAllItemsPlaced += OnLevelComplete;

            _itemsManager.RegisterSettableItems(level.SettableItems);
        }

        private void OnLevelComplete()
        {
            _itemsManager.OnAllItemsPlaced -= OnLevelComplete;
            
            _gameSessionData.CurrentLevel = (_gameSessionData.CurrentLevel + 1) % _levelConfig.Levels.Count;

            _windowsManager.Open<GameEndWindow>().Forget();
        }
    }
}
