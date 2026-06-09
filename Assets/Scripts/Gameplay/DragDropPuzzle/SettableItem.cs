using Gameplay.DragDropPuzzle.Configs;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace Gameplay.DragDropPuzzle
{
    public class SettableItem : MonoBehaviour
    {
        [SerializeField] private ItemTypeEnum itemType;
        [Space]
        [SerializeField] private SpriteRenderer blankSr;
        [SerializeField] private SpriteRenderer filledSr;
        [Space]
        [SerializeField] private Color highlightColor = Color.yellow;
        
        [Inject] private ItemsConfig _itemsConfig;
        
        private ItemData _itemData;
        private bool _isFilled;
        
        public ItemTypeEnum ItemType => itemType;
        public bool IsFilled => _isFilled;

        private void Start()
        {
            if (!_itemsConfig.TryGetItemByType(itemType, out _itemData))
            {
                DebugManager.Log(DebugCategory.Errors, $"SettableItem: Item type {itemType} not found in items config");
                
                gameObject.SetActive(false);
                return;
            }
            
            blankSr.sprite = _itemData.BlankSprite;
            filledSr.sprite = _itemData.InGameSprite;
            filledSr.SetActive(false);
            Highlight(false);
        }

        public void SetItem(ItemData itemData)
        {
            _isFilled = true;
            blankSr.SetActive(false);
            filledSr.SetActive(true);
            
            Highlight(false);
        }
        
        public void Highlight(bool highlight)
        {
            blankSr.color = highlight ? highlightColor : Color.white;
        }
    }
}