using System;
using PT.Tools.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace PT.UI.Menu
{
    public class MenuNavigationButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [Space] 
        [SerializeField] private GameObject[] highlightObjects;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            button ??= GetComponent<Button>();
        }
        
        public void SetAction(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
        }
        
        public void SetHighlighted(bool highlighted)
        {
            highlightObjects.SetActive(highlighted);
            canvasGroup.alpha = highlighted ? 0.8f : 1;
            
            button.interactable = !highlighted;
        }
    }
}