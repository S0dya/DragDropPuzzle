#if UNITY_WEBGL
using YG;
#endif
using PT.Logic.Configs;
using UnityEngine;
using Zenject;

namespace PT.Logic.PlatformRelated.Web
{
    public class AdditionalWebSDKLogicMenu
    {
        [Inject] private GameConfig _gameConfig;
        
        public void Init()
        {
#if UNITY_WEBGL
            Debug.Log("Initializing Web SDKs");
            
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex)
                YG2.GameReadyAPI();
#endif
        }
    }
}