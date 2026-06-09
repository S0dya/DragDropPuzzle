using System;
using UnityEngine;

namespace Gameplay.DragDropPuzzle.Data
{
    [Serializable]
    public class ItemData
    {
        [SerializeField] private string name;
        [SerializeField] private ItemTypeEnum itemType;
        [Space]
        [SerializeField] private Sprite inGameSprite;
        [SerializeField] private Sprite handSprite;
        [SerializeField] private Sprite blankSprite;
        
        public string Name => name;
        public ItemTypeEnum ItemType => itemType;

        public Sprite InGameSprite => inGameSprite;
        public Sprite HandSprite => handSprite;
        public Sprite BlankSprite => blankSprite;
    }
}