using System.Collections.Generic;
using Gameplay.BallDrop.Balls;
using Gameplay.Session;
using PT.Logic.Dependency.Signals;
using PT.Tools.Debugging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.BallDrop.UI.CharacterAppear
{
    public class CharacterAppearController : MonoBehaviour
    {
        [SerializeField] private CharacterAppearView characterViewPrefab;
        [SerializeField] private Transform characterImageParent;
        [Space]
        [SerializeField] private RectTransform[] appearTransforms;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private GameSessionData _gameSessionData;

        private Dictionary<CharacterNameEnum, CharacterAppearView> _characterViews = new();
        
        private CharacterNameEnum? _currentCharacter;
        private int _currentCharacterIndex = -1;
        
        private void Awake()
        {
            CreateViews();
            _signalBus.Subscribe<NewLowestBallSignal>(OnNewLowestBall);
        }
        
        private void CreateViews()
        {
            var spriteMap = new Dictionary<CharacterNameEnum, Sprite>();
            foreach (var data in _gameSessionData.CharacterDatas)
            {
                spriteMap.Add(data.CharacterName, data.CharacterSprite);
            }

            foreach (var kvp in spriteMap)
            {
                var view = Instantiate(characterViewPrefab, characterImageParent);
                view.Init(kvp.Value);
                view.gameObject.SetActive(true);

                _characterViews.Add(kvp.Key, view);
            }
        }

        private void OnNewLowestBall(NewLowestBallSignal signal)
        {
            if (appearTransforms == null || appearTransforms.Length == 0) return;

            var newCharacter = signal.Ball.Info.CharacterName;

            if (_currentCharacter.HasValue && _currentCharacter.Value == newCharacter)
            {
                return;
            }

            if (!_characterViews.TryGetValue(newCharacter, out var newView))
            {
                DebugManager.Log(DebugCategory.UI, $"CharacterAppearController: no view for {newCharacter}", LogType.Warning);
                return;
            }

            if (_currentCharacter.HasValue && _characterViews.TryGetValue(_currentCharacter.Value, out var oldView))
            {
                oldView.Disappear();
            }

            _currentCharacterIndex = (_currentCharacterIndex + 1) % appearTransforms.Length;

            var target = appearTransforms[_currentCharacterIndex];
            newView.SetToSlot(target);

            newView.Appear();
            _currentCharacter = newCharacter;
        }
        
        private void OnDestroy()
        {
            _signalBus.Unsubscribe<NewLowestBallSignal>(OnNewLowestBall);
        }
    }
}