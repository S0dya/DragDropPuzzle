using Gameplay;
using Gameplay.Analytics;
using Gameplay.DragDropPuzzle;
using Gameplay.DragDropPuzzle.Components;
using Gameplay.DragDropPuzzle.Managers;
using Gameplay.DragDropPuzzle.Views;
using PT.Backend.Interfaces;
using PT.GameplayAdditional.Cameras;
using PT.GameplayAdditional.Vibrations;
using PT.Tools.Tutorials;
using PT.UI.Windows;
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
            
            Container.Bind<ItemsManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<HandItemsView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DraggableItem>().FromComponentInHierarchy().AsSingle();
            Container.Bind<HintManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ParticleEffectPlayer>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ProgressIndicator>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PuzzleGameManager>().FromComponentInHierarchy().AsSingle();

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