using UnityEngine;

namespace Gameplay.DragDropPuzzle
{
    public class SettableItem : MonoBehaviour
    {
        [SerializeField] private ItemTypeEnum itemType;
        [Space]
        [SerializeField] private SpriteRenderer blankSr;
        [SerializeField] private SpriteRenderer filledSr;
        
        private void Start()
        {
            //set the item based on the item data, access the config of all the items and get the proper itemData then set the in game object for sprite renderer and for blank renderer    
        }
    }
}