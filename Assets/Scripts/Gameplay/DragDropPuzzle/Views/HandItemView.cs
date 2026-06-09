using System;
using Gameplay.DragDropPuzzle.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.DragDropPuzzle.Views
{
    public class HandItemView : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image itemImage;
        
        public ItemData ItemData { get; private set; }
        
        public event Action<HandItemView, Vector2> OnPressedDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressedDown?.Invoke(this, eventData.position);
        }
        
        public void SetItem(ItemData itemData)
        {
            ItemData = itemData;
            itemImage.sprite = itemData.HandSprite != null ? itemData.HandSprite : itemData.InGameSprite;
        }
    }
}