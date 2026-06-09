using Gameplay.DragDropPuzzle.Views;
using UnityEngine;

namespace Gameplay.DragDropPuzzle.Data
{
    public class PuzzleLevel : MonoBehaviour
    {
        [SerializeField] private SettableItem[] settableItems;
        
        public SettableItem[] SettableItems => settableItems;
    }
}