using Cysharp.Threading.Tasks;
using PT.Logic.Save;
using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class MenuSettingsWindow : WindowBase
    {
        [SerializeField] private Button closeButton;
        
        [Inject(Id = "Menu")] private WindowsManager _windowsManager;
        [Inject] private SaveManager _saveManager;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseSettings);
        }

        private void OnCloseSettings()
        {
            // GlobalEventBus.On(GlobalEventEnum.GameSettingsClosed);

            _saveManager.Save();
            
            _windowsManager.Close<MenuSettingsWindow>().Forget();
        }
    }
}