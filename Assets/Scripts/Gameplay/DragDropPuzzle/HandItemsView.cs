using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.DragDropPuzzle
{
    public class HandItemsView : MonoBehaviour
    {
        [SerializeField] private HandItemView handItemView;
        [SerializeField] private Transform handItemViewsContainer;
        [SerializeField] private CanvasGroup handCg;

        public event Action<ItemData> OnItemSelected;
        
        private List<HandItemView> _activeHandItemViews = new(); 
        
        private ObjectPool<HandItemView> _handItemViewPool;
        
        private void Start()
        {
            _handItemViewPool = new ObjectPool<HandItemView>();//init, set active, set false, default amount : 10
            
            //sign up to all the buttons clicking too to OnHandItemPressed
        }

        public void SetItems(List<ItemData> itemDatas)
        {
            //add all the items using pool
        }
        public void AddItem(ItemData itemData)
        {
            //check where the item was, put it back there
        }
        public void RemoveItem(ItemData itemData)
        {
            // remove the item, but remember the position. on return place it back where it was
        }

        public void SetHandClickable(bool toggle)
        {
            handCg.interactable = toggle;
        }

        private void OnHandItemPressed(HandItemView handItemView)
        {
            OnItemSelected?.Invoke(handItemView.ItemData);
            
            RemoveItem(handItemView.ItemData);
        } 
    }
}