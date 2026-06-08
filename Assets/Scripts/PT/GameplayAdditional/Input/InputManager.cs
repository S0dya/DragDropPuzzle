using System;
using PT.Logic.Dependency.Signals;
using PT.Logic.ProjectContext;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace PT.GameplayAdditional.Input 
{
    public class InputManager : InputManagerBase
    {
        public event Action<Vector2> OnClick;
        public event Action<Vector2> OnDrag;
        public event Action<Vector2> OnRelease;
        public event Action<DirectionEnum> OnSwipe;
        
        [Inject] private SignalBus _signalBus;

        private SwipeDetector _swipe;
        
        private InputActionMap _inGameInput;
        
        private bool _isPressing;
        private Vector2 _lastPointerPosition;       
        
        public void Init()
        {
            _signalBus.Subscribe<GameMenuClosedSignal>(signal =>
            {
                EnableActionMap(_inGameInput);
            });
        }

        protected override void OnStart()
        {
            _inGameInput = _input.InGameInput;
            _actionMapsList.Add(_inGameInput);

            _swipe = new(dir => OnSwipe?.Invoke(dir));

            EnableActionMap(_inGameInput);
        }

        protected override void OnSub()
        {
            var input = _input.InGameInput;

            Subscribe(input.Click, ctx =>
            {
                _isPressing = true;
                Vector2 pos = GetPointerPosition();

                DebugManager.Log(DebugCategory.Input, $"Click started at {pos}");
                OnClick?.Invoke(pos);
                _swipe.OnPress(pos);
            });

            Subscribe(input.Drag, ctx =>
            {
                if (!_isPressing) return;

                Vector2 pos = GetPointerPosition();
                OnDrag?.Invoke(pos);
            });

            Subscribe(input.Release, ctx =>
            {
                if (!_isPressing) return;

                _isPressing = false;
                DebugManager.Log(DebugCategory.Input, $"Released at {_lastPointerPosition}");
                OnRelease?.Invoke(_lastPointerPosition);
                _swipe.OnRelease(_lastPointerPosition);
            });
        }

        private Vector2 GetPointerPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                _lastPointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            else if (Mouse.current != null)
                _lastPointerPosition = Mouse.current.position.ReadValue();
            else if (Pointer.current != null)
                _lastPointerPosition = Pointer.current.position.ReadValue();

            return _lastPointerPosition;
        }

        protected override void OnUnSub()
        {
            DebugManager.Log(DebugCategory.Input, "Input actions unsubscribed or disabled.");
        }
    }
}