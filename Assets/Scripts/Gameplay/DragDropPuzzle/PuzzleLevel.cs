using UnityEngine;

namespace Gameplay.DragDropPuzzle
{
    public class PuzzleLevel : MonoBehaviour
    {
        [SerializeField] private SettableItem[] settableItems;
        
        public SettableItem[] SettableItems => settableItems;
    }
}