using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Button playLevelButton;

        [Inject(Id = "Menu")] private WindowsManager _windowsManager;
        
        private void Awake()
        {
            playLevelButton.onClick.AddListener(OnPlayLevel);
        }

        private void OnPlayLevel()
        {
            // _windowsManager.Open<ChooseSongWindow>().Forget();
        }
    }
}