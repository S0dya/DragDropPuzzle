using Cysharp.Threading.Tasks;
using PT.Logic.Ads;
using PT.Logic.PersistentScene;
using PT.Logic.Save;
using PT.Tools.Sequences;
using PT.Tools.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.UI.Windows
{
    public class GameOverWindow : WindowBase
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [Space]
        [SerializeField] private Sequencer[] sequencers;
        
        [Inject(Id = "Game")] private WindowsManager _windowsManager;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private AdsManager _adsManager;
        [Inject] private SaveManager _saveManager;
        
        private void Awake()
        {
            restartButton.onClick.AddListener(OnRestart);
            menuButton.onClick.AddListener(OnMenu);
        }
        
        private void OnRestart()
        {
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Game),
            }).Forget();
        }
        private void OnMenu()
        {
            _saveManager.Save();
            _adsManager.ShowAd();
            
            _loadingManager.LoadSteps(new()
            {
                _loadingManager.GetSceneUnloadingStep(SceneNameEnum.Game),
                _loadingManager.GetSceneLoadingStep(SceneNameEnum.Menu),
            }).Forget();
        }
        
        protected override async UniTask OnOpen()
        {
            foreach (var sequencer in sequencers)
            {
                sequencer.Play().Forget();
            }
        }
    }
}