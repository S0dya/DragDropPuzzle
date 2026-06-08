using PT.Logic.PersistentScene;
using PT.Tools.Windows;
using Zenject;

namespace PT.Logic.Dependency.PersistentScene
{
    public class PersistentSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<WindowsManager>().WithId("Persistent").FromComponentInHierarchy().AsSingle();
            
            Container.Bind<LoadingWindowManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}