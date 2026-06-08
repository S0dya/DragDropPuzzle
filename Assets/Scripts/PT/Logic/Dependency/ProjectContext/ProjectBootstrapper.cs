using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PT.Backend.Interfaces;
using PT.Logic.Ads;
using PT.Logic.Configs;
using PT.Logic.PersistentScene;
using PT.Logic.PersistentScene.Loading.Steps;
using PT.Logic.PlatformRelated;
using PT.Logic.ProjectContext;
using PT.Logic.Save;
using PT.Tools.Addressables;
using PT.Tools.CurrencyRelated;
using PT.Tools.Score;
using PT.Tools.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

#if UNITY_WEBGL
using YG;
#endif

namespace PT.Logic.Dependency.ProjectContext
{
    public class ProjectBootstrapper : MonoBehaviour
    {
        [Inject] private GameConfig _gameConfig;
        [Inject(Optional = true)] private IBackendService _backendService;
        [Inject(Optional = true)] private IAuthentificationService _authentificationService;
        [Inject] private CurrencyManager _currencyManager;
        [Inject] private LoadingManager _loadingManager;
        [Inject] private SaveManager _saveManager;
        [Inject] private LanguageManager _languageManager;
        [Inject] private AudioManager _audioManager;
        [Inject] private SettingsManager _settingsManager;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private AssetsConfig _assetsConfig;
        [Inject] private AdsManager _adsManager;
        [Inject] private AdConfig _adConfig;
        
        [Inject(Optional = true)] private IAnalyticsService _analyticsService;

        private bool _needsFakeLoad = true;
        
        private async void Awake()
        {
#if UNITY_WEBGL
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex)
            {
                await UniTask.WaitUntil(() => YG2.isSDKEnabled);
            }
#endif
            await InitializeGame();
        }

        private void Start()
        {
            if (!_needsFakeLoad) return;
            
            _loadingManager.LoadSteps(new()
            {
                new FakeWaitingStep(10f)
            }).Forget();
        }
        
        private async UniTask InitializeGame()
        {
            if (_backendService != null) await _backendService.Init();
            if (_authentificationService != null) await _authentificationService.SignIn();

            await LoadGlobalAssets();
            
            _saveManager.Init();
            
            await UniTask.Yield();//myb remove

            await _languageManager.InitAsync();
            _audioManager.Init();
            _settingsManager.Init();
            _currencyManager.Init();
            _scoreManager.Init();
            
            _needsFakeLoad = false;
            if (SceneManager.sceneCount == 1)
            {
                _loadingManager.LoadSteps(new()
                {
                    _loadingManager.GetSceneLoadingStep(SceneNameEnum.Menu),
                    new FakeWaitingStep(0.1f)
                }).Forget();
            }

            if (_adConfig.BannerEnabled) _adsManager.ShowBanner();
        }

        private async UniTask LoadGlobalAssets() //myb move to a dedicated script
        {
            foreach (var key in _assetsConfig.GlobalAssets)
            {
                await _assetProvider.Load<Object>(key);
            }
        }
    }
}
