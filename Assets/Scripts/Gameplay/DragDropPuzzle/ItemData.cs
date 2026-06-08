using System;
using UnityEngine;

namespace Gameplay.DragDropPuzzle
{
    [Serializable]
    public class ItemData
    {
        [SerializeField] private string name;
        [SerializeField] private ItemTypeEnum itemType;
        [SerializeField] private Sprite inGameSprite;
        [SerializeField] private Sprite handSprite;
        [Space]
        [SerializeField] private Sprite gameSprite;
        [SerializeField] private Sprite blankSprite;
        
        public string Name => name;
        public ItemTypeEnum ItemType => itemType;
        public Sprite InGameSprite => inGameSprite;
        public Sprite HandSprite => handSprite;
        
        public Sprite GameSprite => gameSprite;
        public Sprite BlankSprite => blankSprite;
    }
}