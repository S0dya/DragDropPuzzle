using PT.Logic.PlatformRelated.Web;
using PT.Tools.Windows;
using Zenject;

namespace PT.Logic.Dependency.MenuScene
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<WindowsManager>().WithId("Menu").FromComponentInHierarchy().AsSingle();
            
#if UNITY_WEBGL
            Container.Bind<AdditionalWebSDKLogicMenu>().AsSingle();
#endif
        }
    }
}