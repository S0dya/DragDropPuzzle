using Cysharp.Threading.Tasks;
using PT.Logic.Dependency.Signals;
using PT.Logic.PersistentScene;
using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class PauseWindow : WindowBase
    {
        [SerializeField] private Button[] resumeButtons;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button menuButton;
        
        [Inject] private SignalBus _signalBus;
        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private LoadingManager _loadingManager;
        
        private void Awake()
        {
            foreach (var resumeButton in resumeButtons) resumeButton.onClick.AddListener(OnResume);
            replayButton?.onClick.AddListener(OnReplay);
            menuButton.onClick.AddListener(OnMenu);            
        }

        private void OnResume()
        {
            _signalBus.Fire(new GameMenuClosedSignal());
            
            _windowsManager.Close<PauseWindow>().Forget();
        }
        
        private void OnReplay()
        {
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Game),
            }).Forget();
            
            _windowsManager.Close<PauseWindow>().Forget();
        }
        
        private void OnMenu()
        {
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Menu),
            }).Forget();

            _windowsManager.Close<PauseWindow>().Forget();
        }
    }
}