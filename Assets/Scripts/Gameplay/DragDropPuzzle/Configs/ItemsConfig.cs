using Gameplay.DragDropPuzzle.Data;
using UnityEngine;

namespace Gameplay.DragDropPuzzle.Configs
{
    [CreateAssetMenu(menuName = "Configs/ItemsConfig", fileName = "ItemsConfig")]
    public class ItemsConfig : ScriptableObject
    {
        [SerializeField] private ItemData[] itemDatas;
        
        public ItemData[] ItemDatas => itemDatas;

        public bool TryGetItemByType(ItemTypeEnum itemType, out ItemData foundItemData)
        {
            foundItemData = null;
            
            foreach (var itemData in itemDatas)
            {
                if (itemData.ItemType == itemType)
                {
                    foundItemData = itemData;
                    
                    return true;
                }
            }
            
            return false;
        }
    }
}