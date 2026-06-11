using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.DragDropPuzzle.Components;
using Gameplay.DragDropPuzzle.Data;
using Gameplay.DragDropPuzzle.Views;
using PT.GameplayAdditional.Input;
using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using PT.Tools.Debugging;
using UnityEngine;
using Zenject;

namespace Gameplay.DragDropPuzzle.Managers
{
    public class ItemsManager : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        
        [Inject] private GameConfig _gameConfig;
        [Inject] private InputManager _inputManager;
        [Inject] private DraggableItem _draggableItem;
        [Inject] private HandItemsView _handItemsView;
        [Inject] private AudioManager _audioManager;
        [Inject] private ParticleEffectPlayer _particleEffectPlayer;
        [Inject] private ProgressIndicator _progressIndicator;
        [Inject] private HintManager _hintManager;
        
        public event Action OnAllItemsPlaced;
        
        private List<SettableItem> _settableItems = new();
        private ItemData _currentDraggedItem;
        private bool _isDragging = false;
        private bool _isProcessing = false;
        private SettableItem _currentTargetSlot;
        private Vector2 _startWorldPosition;

        private void Awake()
        {
            _inputManager.OnDrag += OnDrag;
            _inputManager.OnRelease += OnRelease;
            
            _handItemsView.OnItemSelected += OnItemSelected;
            
            _draggableItem.OnReachedTarget += OnItemReachedTarget;
            _draggableItem.OnReturned += OnItemReturned;
            
            _hintManager.OnHintRequested += HintSettableItem;
        }
        private void OnDestroy()
        {
            _inputManager.OnDrag -= OnDrag;
            _inputManager.OnRelease -= OnRelease;
            
            _handItemsView.OnItemSelected -= OnItemSelected;
            
            _draggableItem.OnReachedTarget -= OnItemReachedTarget;
            _draggableItem.OnReturned -= OnItemReturned;
            
            _hintManager.OnHintRequested -= HintSettableItem;
        }

        public void RegisterSettableItems(SettableItem[] settableItems)
        {
            _settableItems.Clear();
            _settableItems = settableItems.ToList();
        }

        private void OnItemSelected(ItemData itemData, Vector2 pointerScreenPosition)
        {
            if (_isDragging || _isProcessing) return;
            
            DebugManager.Log(DebugCategory.Gameplay, $"OnItemSelected: {itemData}");
            
            _currentDraggedItem = itemData;
            _isDragging = true;
            _isProcessing = true;
            
            foreach (var slot in _settableItems)
            {
                if (slot.ItemType == itemData.ItemType && !slot.IsFilled)
                    _currentTargetSlot = slot;
            }
            
            _draggableItem.SetItem(itemData);
            
            _startWorldPosition = cam.ScreenToWorldPoint(pointerScreenPosition);
            
            _draggableItem.transform.position = _startWorldPosition;
            _draggableItem.StartDrag(pointerScreenPosition);
            
            _handItemsView.SetHandClickable(false);
        }

        private void OnDrag(Vector2 position)
        {
            if (!_isDragging) return;
            
            _draggableItem.Move(position);
            
            if (_draggableItem.CheckSnapDistance(position, _currentTargetSlot.transform.position))
            {
                if (!_draggableItem.IsSnapped)
                {
                    _draggableItem.SnapTo(_currentTargetSlot.transform.position);
                }
            }
            else
            {
                if (_draggableItem.IsSnapped)
                {
                    _draggableItem.Unsnap();
                }
            }
        }

        private void OnRelease(Vector2 position)
        {
            if (!_isDragging) return;
            
            if (_draggableItem.IsSnapped)
            {
                _currentTargetSlot.SetItem(_currentDraggedItem);
                PlaySuccessEffect(_currentTargetSlot.transform.position);
                OnItemReachedTarget(_draggableItem);
            }
            else
            {
                if (_draggableItem.CheckSnapDistance(position, _currentTargetSlot.transform.position))
                {
                    _draggableItem.AnimateTo(_currentTargetSlot.transform.position, _gameConfig.AnimationDuration, () =>
                    {
                        _currentTargetSlot.SetItem(_currentDraggedItem);
                        PlaySuccessEffect(_currentTargetSlot.transform.position);
                    });
                }
                else
                {
                    _draggableItem.ReturnToOriginal(_startWorldPosition);
                }
            }
            
            _isDragging = false;
        }

        private void OnItemReachedTarget(DraggableItem draggableItem)
        {
            _isProcessing = false;
            _isDragging = false;
            
            _handItemsView.RemoveItem(_currentDraggedItem);
            _handItemsView.SetHandClickable(true);
            _progressIndicator.OnPiecePlaced();
            _currentDraggedItem = null;

            if (_handItemsView.IsHandEmpty())
            {
                OnAllItemsPlaced?.Invoke();
            }
        }

        private void OnItemReturned(DraggableItem draggableItem)
        {
            _isProcessing = false;
            _isDragging = false;
            
            _handItemsView.AddItem(_currentDraggedItem);
            _handItemsView.SetHandClickable(true);
            _currentDraggedItem = null;
            _draggableItem.HideItem();
        }

        private void PlaySuccessEffect(Vector2 position)
        {
            _audioManager.PlayOneShot(SoundEventEnum.ConfettiSmall, position);
            _particleEffectPlayer.PlaySuccessEffect(position);
        }
        
        
        private void HintSettableItem()
        {
            var settableItemToHighlight = _settableItems.FirstOrDefault(item => item.ItemType == _handItemsView.GetFirstItem().ItemType);

            if (settableItemToHighlight != null)
            {
                settableItemToHighlight.Highlight(true);
            }
        }
    }
}