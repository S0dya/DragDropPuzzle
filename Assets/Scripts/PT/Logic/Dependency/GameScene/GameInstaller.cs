using Gameplay;
using Gameplay.Analytics;
using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Balls.BallCollisionStrategy;
using Gameplay.BallDrop.Configs;
using Gameplay.BallDrop.Levels;
using Gameplay.BallDrop.Levels.Generation;
using Gameplay.BallDrop.Music;
using Gameplay.BallDrop.UI;
using PT.Backend.Interfaces;
using PT.GameplayAdditional.Cameras;
using PT.GameplayAdditional.Vibrations;
using PT.Logic.Configs;
using PT.Tools.Tutorials;
using PT.UI.Windows;
using UnityEngine;
using Zenject;

namespace PT.Logic.Dependency.GameScene
{
    public class GameInstaller : BaseGameInstaller
    {
        [Inject(Optional = true)] private IBackendService _backendService;
        
        public override void InstallBindings()
        {
            base.InstallBindings();
            
            Container.Bind<TutorialManager>().FromComponentInHierarchy().AsSingle();

            Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
            
            Container.Bind<GameplayController>().FromComponentInHierarchy().AsSingle();
            
            Container.Bind<GameEndRewardWindow>().FromComponentInHierarchy().AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameSoundsController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameVibrationsController>().AsSingle();
            
            Container.Bind<IBallCollisionHandler>().To<LevelTriggerCollisionHandler>().AsSingle();
            Container.Bind<IBallCollisionHandler>().To<ObstacleCollisionHandler>().AsSingle();
            Container.Bind<IBallCollisionHandler>().To<FinishCollisionHandler>().AsSingle();

            Container.Bind<ILevelGenerationService>().To<LengthLevelGenerationService>().AsSingle();
            
            Container.Bind<LevelSessionData>().AsSingle();
            
            Container.Bind<LevelGenerator>().FromComponentInHierarchy().AsSingle();
            Container.Bind<BallsController>().FromComponentInHierarchy().AsSingle();
            
            Container.Bind<GameAudioController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<BallsNamesController>().FromComponentInHierarchy().AsSingle();

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            if (_backendService != null && _backendService.IsReady)
            {
                Container.BindInterfacesAndSelfTo<AnalyticsEventsListener>().AsSingle();
                Container.Bind<RunState>().AsSingle();
            }
#elif UNITY_WEBGL
            // Container.Bind<AdditionalWebSDKLogic>().AsSingle();
#endif
        }
    }
}