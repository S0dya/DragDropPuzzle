using System;
using System.Collections.Generic;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.DragDropPuzzle
{
    public class HandItemsView : MonoBehaviour
    {
        [SerializeField] private HandItemView handItemView;
        [SerializeField] private Transform handItemViewsContainer;
        [SerializeField] private CanvasGroup handCg;

        public event Action<ItemData, Vector2> OnItemSelected;
        
        private List<HandItemView> _activeHandItemViews = new(); 
        private Dictionary<ItemData, int> _itemPositions = new();
        
        private ObjectPool<HandItemView> _handItemViewPool;
        private int _totalItemCount;
        
        private void Start()
        {
            _handItemViewPool = new ObjectPool<HandItemView>(
                CreateHandItemView,
                obj => obj.SetActive(true),
                obj => obj.SetActive(false),
                defaultCapacity: 20
            );
        }

        private HandItemView CreateHandItemView()
        {
            HandItemView view = Instantiate(handItemView, handItemViewsContainer);
            view.OnPressedDown += OnHandItemPointerDown;
            return view;
        }

        public void SetItems(List<ItemData> itemDatas)
        {
            ClearItems();
            _totalItemCount = itemDatas.Count;
            
            for (int i = 0; i < itemDatas.Count; i++)
            {
                AddItem(itemDatas[i], i);
            }
        }

        public void AddItem(ItemData itemData)
        {
            if (_itemPositions.ContainsKey(itemData))
            {
                AddItem(itemData, _itemPositions[itemData]);
            }
            else
            {
                AddItem(itemData, _activeHandItemViews.Count);
            }
        }

        private void AddItem(ItemData itemData, int position)
        {
            HandItemView view = _handItemViewPool.Get();
            view.SetItem(itemData);
            
            if (position >= _activeHandItemViews.Count)
            {
                _activeHandItemViews.Add(view);
            }
            else
            {
                _activeHandItemViews.Insert(position, view);
            }
            
            _itemPositions[itemData] = position;
            ReorderItems();
        }

        public void RemoveItem(ItemData itemData)
        {
            HandItemView view = FindViewByItemData(itemData);
            if (view != null)
            {
                _activeHandItemViews.Remove(view);
                _handItemViewPool.Release(view);
                UpdatePositions();
            }
        }

        private HandItemView FindViewByItemData(ItemData itemData)
        {
            foreach (var view in _activeHandItemViews)
            {
                if (view.ItemData == itemData)
                    return view;
            }
            return null;
        }

        private void UpdatePositions()
        {
            for (int i = 0; i < _activeHandItemViews.Count; i++)
            {
                _itemPositions[_activeHandItemViews[i].ItemData] = i;
            }
        }

        private void ReorderItems()
        {
            for (int i = 0; i < _activeHandItemViews.Count; i++)
            {
                _activeHandItemViews[i].transform.SetSiblingIndex(i);
            }
        }

        private void ClearItems()
        {
            foreach (var view in _activeHandItemViews)
            {
                _handItemViewPool.Release(view);
            }
            _activeHandItemViews.Clear();
            _itemPositions.Clear();
        }

        public void SetHandClickable(bool toggle)
        {
            handCg.interactable = toggle;
            handCg.blocksRaycasts = toggle;

            if (toggle) UpdatePositions();
        }

        public ItemData GetFirstItem()
        {
            if (_activeHandItemViews.Count > 0)
            {
                return _activeHandItemViews[0].ItemData;
            }
            return null;
        }
        
        public bool IsHandEmpty() => _activeHandItemViews.Count == 0;

        private void OnHandItemPointerDown(HandItemView handItemView, Vector2 pointerPosition)
        {
            DebugManager.Log(DebugCategory.Gameplay, $"HandItemPointerDown: {handItemView.ItemData}");
            
            RemoveItem(handItemView.ItemData);
            
            OnItemSelected?.Invoke(handItemView.ItemData, pointerPosition);
        }
    }
}