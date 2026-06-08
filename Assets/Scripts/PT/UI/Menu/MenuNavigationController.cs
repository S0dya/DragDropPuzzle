using System;
using Cysharp.Threading.Tasks;
using PT.Tools.Windows;
using UnityEngine;
using Zenject;

namespace PT.UI.Menu
{
    internal enum MenuWindowType
    {
        MainMenu,
        Settings,
        Leaderboard,
        Tutorial,
        Shop,
    }
        
    [Serializable]
    class WindowInfo
    {
        [SerializeField] private MenuNavigationButtonView buttonView;
        [SerializeField] private MenuWindowType windowType;
        [SerializeField] private GameObject windowObject;
        
        public MenuNavigationButtonView ButtonView => buttonView;
        public MenuWindowType WindowType => windowType;
        public GameObject WindowObject => windowObject;
    }
    
    public class MenuNavigationController : MonoBehaviour
    {
        [SerializeField] private WindowInfo[] windowInfos;

        [Inject(Id = "Menu")] private WindowsManager _windowsManager;
        
        private void Awake()
        {
            foreach (var windowInfo in windowInfos)
            {
                windowInfo.ButtonView.SetAction(() =>
                {
                    OpenWindow(windowInfo.WindowType);
                });
            }
            
            OpenWindow(MenuWindowType.MainMenu);
        }

        private void OpenWindow(MenuWindowType typeToOpen)
        {
            _windowsManager.CloseAll().Forget();
            
            foreach (var windowInfo in windowInfos)
            {
                var open = windowInfo.WindowType == typeToOpen;
                
                windowInfo.WindowObject.SetActive(open);
                windowInfo.ButtonView.SetHighlighted(open);
            }   
        }
    }
}