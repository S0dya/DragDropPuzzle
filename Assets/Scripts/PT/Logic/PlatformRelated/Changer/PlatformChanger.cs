using PT.Logic.Configs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.Logic.PlatformRelated.Changer
{
    public class PlatformChanger : MonoBehaviour
    {
        [System.Serializable]
        class PlatformImageSpritesToggle
        {
            [SerializeField] private Image image;
            [SerializeField] private Sprite mobileSprite;
            [SerializeField] private Sprite webSprite;
            
            public Image Image => image;
            public Sprite MobileSprite => mobileSprite;
            public Sprite WebSprite => webSprite;
        }

        [SerializeField] private GameObject[] mobilePlatformObjects;
        [SerializeField] private GameObject[] webPlatformObjects;
        [Space]
        [SerializeField] private PlatformImageSpritesToggle[] platformImageSpritesToggle;

        [Inject] private GameConfig _gameConfig;
        
        private void Awake()
        {
            bool isMobile = _gameConfig.PlatformType == PlatformTypeEnum.Mobile || 
                            (_gameConfig.PlatformType == PlatformTypeEnum.Yandex);
                            // (_gameConfig.PlatformType == PlatformTypeEnum.Yandex && Settings.IsMobileDevice);

            foreach (var obj in mobilePlatformObjects) obj.SetActive(isMobile);
            foreach (var obj in webPlatformObjects) obj.SetActive(!isMobile);
            foreach (var iST in platformImageSpritesToggle) iST.Image.sprite = isMobile ? iST.MobileSprite : iST.WebSprite;
            
            enabled = false;
        }
    }
}