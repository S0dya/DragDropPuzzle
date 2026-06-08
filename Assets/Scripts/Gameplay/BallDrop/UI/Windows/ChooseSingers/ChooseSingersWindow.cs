using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallDrop.Configs;
using Gameplay.BallDrop.Datas;
using Gameplay.BallDrop.UI.Windows.ChooseSong;
using Gameplay.Session;
using MoreMountains.NiceVibrations;
using PT.Logic.Configs;
using PT.Logic.PersistentScene;
using PT.Logic.Save;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using PT.Tools.Vibrations;
using PT.Tools.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.BallDrop.UI.Windows.ChooseSingers
{
    public class ChooseSingersWindow : WindowBase
    {
        [SerializeField] private SongView songView;
        [Space]
        [SerializeField] private SingerView singerPrefab;
        [SerializeField] private Transform unlockedSingersParent;
        [SerializeField] private Transform lockedSingersParent;
        [Space]
        [SerializeField] private SingerUnlockable singerUnlockable;
        [Space]
        [SerializeField] private RectTransform selectedSingersAmountRect;
        [SerializeField] private TextMeshProUGUI selectedSingersAmountText;
        [Space]
        [SerializeField] private Button playButton;
        [SerializeField] private Button closeButton;

        [Inject] private GameConfig _gameConfig;
        [Inject] private CharacterConfig _characterConfig;
        [Inject] private GameSessionData _gameSessionData;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private VibrationManager _vibrationManager;
        [Inject(Id = "Menu")] private WindowsManager _windowsManager;

        private SongData _currentSongData;
        
        private readonly List<SingerView> _singerViews = new();
        private List<SingerView> _currentSingersViews = new();
        
        private Tween _selectedAmountShakeTween;
        private Tween _selectedAmountPunchTween;
        
        private void Awake()
        {
            for (int i = 0; i < _gameConfig.MaxTotalSingersAmount; i++) 
            {
                var singerView = Instantiate(singerPrefab, unlockedSingersParent);
                singerView.SetActive(false);
                
                _singerViews.Add(singerView);
                singerView.OnPressed += OnSingerChosen;
            }
            
            playButton.onClick.AddListener(OnPlay);
            closeButton.onClick.AddListener(OnClosePressed);
            
            RefreshPlayButtonState();
        }

        protected override async UniTask OnOpen()
        {
            if (Payload is not SongData songData)
            {
                DebugManager.Log(DebugCategory.Gameplay, "Payload is null", LogType.Error);
                await UniTask.CompletedTask;
                return;
            }
            
            _currentSongData = songData;
            
            _currentSingersViews.Clear();
            UpdateSelectedSingersAmountText();
            songView.Init(_currentSongData);

            for (int i = 0; i < _currentSongData.Singers.Length; i++)
            {
                var characterData = _characterConfig.CharacterDatas.FirstOrDefault(cd => cd.CharacterName == _currentSongData.Singers[i]);

                if (characterData == null)
                {
                    DebugManager.Log(DebugCategory.Gameplay, "characterData is not found", LogType.Error); return;
                }
                
                bool isSecondHalf = i >= _currentSongData.Singers.Length / 2;
                
                _singerViews[i].transform.SetParent(isSecondHalf ? lockedSingersParent : unlockedSingersParent);
                _singerViews[i].SetActive(true);
                _singerViews[i].SetInfo(characterData);
            }
            
            RefreshPlayButtonState();
            
            singerUnlockable.SetSingerIndex(_currentSongData.SongIndex);
            
            await base.OnOpen();
        }

        protected override async UniTask OnClose()
        {
            await base.OnClose();
            
            foreach (var singer in _singerViews)
            {
                singer.SetActive(false);
                singer.Deselect();
            }
            _currentSingersViews.Clear();
            UpdateSelectedSingersAmountText();
            RefreshPlayButtonState();
        }

        private void OnSingerChosen(SingerView singerView)
        {
            if (!_currentSingersViews.Contains(singerView))
            {
                if (_currentSingersViews.Count >= _gameConfig.MaxSingersAmount)
                {
                    PlayMaxSingersReachedFeedback(singerView); return;
                }                
                
                _currentSingersViews.Add(singerView);
                singerView.Select();

                UpdateSelectedSingersAmountText(true);
                RefreshPlayButtonState();
                PlayAmountChangedAnimation(true);
            }
            else
            {
                _currentSingersViews.Remove(singerView);
                singerView.Deselect();

                UpdateSelectedSingersAmountText(true);
                RefreshPlayButtonState();
                PlayAmountChangedAnimation(false);
            }
        }
        
        
        private void UpdateSelectedSingersAmountText(bool updateButtonState = true)
        {
            selectedSingersAmountText.text = $"{_currentSingersViews.Count}/{_gameConfig.MaxSingersAmount}";

            if (updateButtonState)
            {
                RefreshPlayButtonState();
            }
        }

        private void RefreshPlayButtonState()
        {
            playButton.interactable = _currentSingersViews.Count >= _gameConfig.MinSingersAmount;
        }
        
        private void PlayMaxSingersReachedFeedback(SingerView singerView)
        {
            singerView.PlayRejectAnimation();

            _selectedAmountShakeTween?.Kill();
            
            var returnPos = selectedSingersAmountRect.anchoredPosition;
            
            _selectedAmountShakeTween = selectedSingersAmountRect
                .DOShakeAnchorPos(0.22f, new Vector2(18f, 0f), 12, 90f, false, true)
                .OnKill(() => selectedSingersAmountRect.anchoredPosition = returnPos)
                .OnComplete(() => selectedSingersAmountRect.anchoredPosition = returnPos);
        }
        private void PlayAmountChangedAnimation(bool isAdded)
        {
            _selectedAmountPunchTween?.Kill();

            selectedSingersAmountRect.localScale = Vector3.one;

            var targetScale = isAdded ? 1.12f : 1.08f;
            var upDuration = isAdded ? 0.12f : 0.1f;
            var downDuration = isAdded ? 0.14f : 0.12f;

            _selectedAmountPunchTween = selectedSingersAmountRect
                .DOScale(targetScale, upDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    selectedSingersAmountRect
                        .DOScale(1f, downDuration)
                        .SetEase(Ease.OutBack);
                });
        }

        private void OnPlay()
        {
            if (_currentSingersViews.Count < _gameConfig.MinSingersAmount)
            {
                _vibrationManager.Vibrate(HapticTypes.Warning); return;
            }

            _vibrationManager.Vibrate(HapticTypes.Selection);
            StartGame();
        }

        private void StartGame()
        {
            var currentCharacterDatas = _currentSingersViews.Select(sv => sv.CharacterData).ToArray();
            
            _gameSessionData.SetInfo(0, _currentSongData, currentCharacterDatas);
            
            DebugManager.Log(DebugCategory.UI, $"level starting : {_gameSessionData.LevelLengthType} | index : {_gameSessionData.SelectedLevelIndex} | song : {_gameSessionData.SongData.Name} | length : {_gameSessionData.CharacterDatas.Length}");
            
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Menu),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Game),
            }).Forget();
        }
        private void OnClosePressed()
        {
            _windowsManager.Close<ChooseSingersWindow>().Forget();
        }

        
        private void OnDestroy()
        {
            foreach (var singer in _singerViews) singer.OnPressed -= OnSingerChosen;

            _selectedAmountShakeTween?.Kill();
            _selectedAmountPunchTween?.Kill();
        }
    }
}