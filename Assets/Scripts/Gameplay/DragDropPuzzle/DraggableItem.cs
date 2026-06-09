using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.DragDropPuzzle
{
    public class DraggableItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float snapDistance = 1f;
        [SerializeField] private float snapSpeed = 15f;
        [SerializeField] private float unsnapDistance = 1.5f;
        [SerializeField] private Camera cam;
        
        private ItemData _itemData;
        private Vector2 _originalPosition;
        private Vector2 _snapTargetPosition;
        private bool _isDragging;
        private bool _isLocked;
        private bool _isSnapped;
        
        public event Action<DraggableItem> OnReachedTarget;
        public event Action<DraggableItem> OnReturned;
        public ItemData ItemData => _itemData;
        public bool IsLocked => _isLocked;
        public bool IsDragging => _isDragging;
        public bool IsSnapped => _isSnapped;

        public void SetItem(ItemData itemData)
        {
            _itemData = itemData;
            spriteRenderer.sprite = itemData.InGameSprite;
            _originalPosition = transform.position;
            _isSnapped = false;
            gameObject.SetActive(true);
        }
        
        public void StartDrag(Vector2 screenPosition)
        {
            if (_isLocked) return;
            
            _isDragging = true;
            _isSnapped = false;
            _originalPosition = transform.position;
            MoveToPointer(screenPosition);
        }

        public void Move(Vector2 screenPosition)
        {
            if (!_isDragging || _isLocked) return;
            
            if (_isSnapped)
            {
                float distance = Vector2.Distance(transform.position, _snapTargetPosition);
                if (distance > unsnapDistance)
                {
                    Unsnap();
                }
                else
                {
                    return;
                }
            }
            
            MoveToPointer(screenPosition);
        }

        private void MoveToPointer(Vector2 screenPosition)
        {
            var worldPosition = cam.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
            transform.position = worldPosition;
        }

        public bool CheckSnapDistance(Vector2 screenPosition, Vector2 targetPosition)
        {
            if (_isLocked) return false;
            Vector2 worldPosition = cam.ScreenToWorldPoint(screenPosition);
            
            float distance = Vector2.Distance(worldPosition, targetPosition);
            return distance <= snapDistance;
        }

        public void SnapTo(Vector2 position)
        {
            _isSnapped = true;
            _snapTargetPosition = position;
        }

        public void UpdateSnap()
        {
            if (!_isSnapped || _isLocked) return;
            
            transform.position = Vector2.Lerp(transform.position, _snapTargetPosition, snapSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, _snapTargetPosition) < 0.01f)
            {
                transform.position = _snapTargetPosition;
            }
        }

        public void Unsnap()
        {
            _isSnapped = false;
        }

        public void AnimateTo(Vector2 position, float duration = 0.3f, Action onComplete = null)
        {
            if (_isLocked) return;
            
            _isDragging = false;
            
            transform.DOMove(position, duration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _isLocked = true;
                    _isSnapped = true;
                    OnReachedTarget?.Invoke(this);
                    onComplete?.Invoke();
                    HideItem();
                });
        }

        public async void ReturnToOriginal(Vector2 returnPosition)
        {
            if (_isLocked) return;
            
            _isDragging = false;
            
            transform.DOMove(returnPosition, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    OnReturned?.Invoke(this);
                    HideItem();
                });
        }

        public void HideItem()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateSnap();
        }
    }
}