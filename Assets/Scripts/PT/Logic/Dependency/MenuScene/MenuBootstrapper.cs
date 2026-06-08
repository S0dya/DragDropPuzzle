using PT.Logic.PlatformRelated.Web;
using UnityEngine;
using Zenject;

namespace PT.Logic.Dependency.MenuScene
{
    public class MenuBootstrapper : MonoBehaviour
    {
#if UNITY_WEBGL
        [Inject] private AdditionalWebSDKLogicMenu _additionalWebSDKLogicMenu;
#endif

        private void Start()
        {
            
#if UNITY_WEBGL
            _additionalWebSDKLogicMenu.Init();
#endif
        }
    }
}