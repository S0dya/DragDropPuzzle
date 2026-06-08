using Cysharp.Threading.Tasks;
using Gameplay;
using PT.Logic.Dependency.Signals;
using PT.Tools.Windows;
using PT.UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.GameplayAdditional.UI
{
    public class GameplayView : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;

        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private GameplayController _gameplayController;
        [Inject] private SignalBus _signalBus;
        
        private void Awake()
        {
            pauseButton.onClick.AddListener(OpenPause);
        }

        private void OpenPause()
        {
            _signalBus.Fire(new GameMenuOpenedSignal());
            _windowsManager.Open<PauseWindow>().Forget();
        }
    }
}