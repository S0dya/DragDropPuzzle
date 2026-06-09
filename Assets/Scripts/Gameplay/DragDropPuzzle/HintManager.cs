using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.DragDropPuzzle
{
    public class HintManager : MonoBehaviour
    {
        [SerializeField] private Button hintButton;
        
        public event Action OnHintRequested;
        
        private void Awake()
        {
            hintButton.onClick.AddListener(ShowHint);
        }
        
        private void ShowHint()
        {
            OnHintRequested?.Invoke();
        }
    }
}
