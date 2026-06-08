using System;
using PT.UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.DragDropPuzzle
{
    public class HandItemView : MonoBehaviour
    {
        [SerializeField] private BasicButton mainButton;
        [SerializeField] private Image itemImage;
        
        public ItemData ItemData { get; private set; }
        
        public event Action<HandItemView> OnPressed;

        private void Start()
        {
            mainButton.SetOnClick(() => OnPressed?.Invoke(this));
        }
        
        public void SetItem(ItemData itemData)
        {
            ItemData = itemData;
            itemImage.sprite = itemData.HandSprite;
        }
    }
}