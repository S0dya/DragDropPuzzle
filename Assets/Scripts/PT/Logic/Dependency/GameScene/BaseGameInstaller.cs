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

            // Container.DeclareSignal<GameEndedSignal>();
        }
    }
}