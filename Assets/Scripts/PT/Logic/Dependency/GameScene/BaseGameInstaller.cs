using Gameplay.BallDrop.Balls;
using PT.GameplayAdditional.Input;
using PT.Tools.Windows;
using Zenject;

namespace PT.Logic.Dependency.GameScene
{
    public class BaseGameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<WindowsManager>().WithId("Game").FromComponentInHierarchy().AsSingle();

            Container.Bind<InputManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PushableBallManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<BallStatisticsManager>().FromComponentInHierarchy().AsSingle();

            // Container.DeclareSignal<GameEndedSignal>();
        }
    }
}