using PT.Backend.Fake;
using PT.Backend.Interfaces;
using PT.Logic.Ads;
using PT.Logic.Configs;
using PT.Logic.Dependency.Signals;
using PT.Logic.PersistentScene;
using PT.Logic.PlatformRelated.Mobile;
using PT.Logic.ProjectContext;
using PT.Logic.Save;
using PT.Tools;
using PT.Tools.Addressables;
using PT.Tools.CurrencyRelated;
using PT.Tools.Debugging;
using PT.Tools.Factories;
using PT.Tools.Leaderboard;
using PT.Tools.Other;
using PT.Tools.Score;
using PT.Tools.TimeRelated;
using PT.Tools.Vibrations;
using UnityEngine;
using Zenject;
using PT.Backend.FB;
using PT.Backend.Interfaces;
using PT.Tools.Leaderboard;
using PT.Backend.GP;
using PT.Backend.YG;
using PT.Logic.PlatformRelated;
using PT.Logic.PlatformRelated.Web;
using PT.Logic.PlatformRelated.YG;

namespace PT.Logic.Dependency.ProjectContext
{
    public class BaseProjectInstaller : MonoInstaller
    {
        [Inject] private LeaderboardConfig _leaderboardConfig;
        [Inject] private GameConfig _gameConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<SaveManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LanguageManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<AudioManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<VibrationManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<AdsManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LoadingManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<LeaderboardManager>().AsSingle();

            Container.Bind<IFactoryZenject>().To<FactoryZenject>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<AddressablesAssetsService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PrefabFactory>().AsSingle();
            Container.Bind<IAssetsSceneLoader>().To<AddressablesSceneLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<AddressablesDiagnostics>().AsSingle();
            
            Container.Bind<TimeManager>().FromComponentInHierarchy().AsSingle();
            
            Container.BindInterfacesAndSelfTo<CurrencyManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
            Container.Bind<ScoreMultiplier>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<AudioSignalsController>().AsSingle();
            Container.BindInterfacesAndSelfTo<VibrationSignalsController>().AsSingle();

            Container.BindInterfacesAndSelfTo<SignalsLogger>().AsSingle();
            
#if UNITY_WEBGL
            Container.BindInterfacesAndSelfTo<AdditionalWebSDKLogic>().AsSingle();
            
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex)
            {
                Container.Bind<ISaveService>().To<YandexSave>().AsSingle();
                Container.BindInterfacesAndSelfTo<YandexAds>().AsSingle();
            }
            else if (_gameConfig.PlatformType == PlatformTypeEnum.WebGL)
            {
            
            }
            
#elif UNITY_IOS || UNITY_ANDROID
            Container.Bind<ISaveService>().To<PrefsSave>().AsSingle();
            Container.BindInterfacesAndSelfTo<UnityAds>().AsSingle();
            
            #if !UNITY_EDITOR
            Container.Bind<FirebaseBackendService>().AsSingle();
            Container.Bind<IBackendService>().To<FirebaseBackendService>().FromResolve();
            
            Container.Bind<IAnalyticsService>().To<FirebaseAnalyticsService>().AsSingle();
            Container.Bind<IRemoteConfigService>().To<FirebaseRemoteConfigService>().AsSingle();
            Container.Bind<ICloudSaveService>().To<FirebaseCloudSaveService>().AsSingle();
            Container.Bind<IDatabaseService>().To<FirebaseDatabaseService>().AsSingle();
            
            Container.QueueForInject(_gameConfig);//myb remove
            #endif
#endif

            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<GameMenuOpenedSignal>();
            Container.DeclareSignal<GameMenuClosedSignal>();
            
            Container.DeclareSignal<ShowAdSignal>();
            Container.DeclareSignal<AdOpenedSignal>();
            Container.DeclareSignal<AdClosedSignal>();
            Container.DeclareSignal<AdCountdownStartSignal>();
            Container.DeclareSignal<AdCountdownStopSignal>();
            
            Container.DeclareSignal<SceneUnloadedSignal>();
            
            Container.DeclareSignal<GameIntroOpenedSignal>();
            Container.DeclareSignal<GameIntroClosedSignal>();
            
            Container.DeclareSignal<GameStartedSignal>();
            Container.DeclareSignal<GameEndedSignal>();
            Container.DeclareSignal<GameVictorySignal>();
            Container.DeclareSignal<GameReplaySignal>();
            Container.DeclareSignal<NewHighestScoreReachedSignal>();
            
            switch (_leaderboardConfig.LeaderboardType)
            {
                case LeaderboardType.Fake:
                    Container.Bind<IAuthentificationService>().To<FakeAuthentificationService>().AsSingle();
                    Container.BindInterfacesAndSelfTo<FakeLeaderboardService>().AsSingle();
                    break;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
                case LeaderboardType.Firebase:
                    Container.Bind<IAuthentificationService>().To<FirebaseAuthentificationService>().AsSingle();
                    Container.Bind<ILeaderboardService>().To<DatabaseLeaderboardService>().AsSingle();
                    break;
                case LeaderboardType.GooglePlayGames:
                    Container.Bind<IAuthentificationService>().To<GooglePlayAuthentificationService>().AsSingle();
                    Container.Bind<ILeaderboardService>().To<GooglePlayLeaderboardService>().AsSingle();
                    break;
#elif UNITY_WEBGL
                case LeaderboardType.YandexGames: 
                    // Container.Bind<IAuthentificationService>().To<YandexAuthentificationService>().AsSingle();
                    // Container.Bind<ILeaderboardService>().To<YandexLeaderboardService>().AsSingle();
                    break;
#endif         
            }
        }
    }
}