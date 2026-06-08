using System;
using System.Linq;
using PT.Logic.Configs;
using PT.Tools.Helper;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace PT.Logic.PlatformRelated.Changer
{
    public class PlatformToggler : MonoBehaviour
    {
        [Serializable]
        private class SerializablePlatformEnums
        {
            [SerializeField] private PlatformTypeEnum[] platformTypeEnums;
            public PlatformTypeEnum[] PlatformTypeEnums => platformTypeEnums;
        }
        
        [SerializeField] private SerializableKeyValue<SerializablePlatformEnums, UnityEvent> platformActions;
        
        [Inject] private GameConfig _gameConfig;
        
        private void Awake()
        {
            foreach (var platformAction in platformActions.Dictionary)
                if (platformAction.Key.PlatformTypeEnums.Contains(_gameConfig.PlatformType))
                    platformAction.Value?.Invoke();
            
            enabled = false;
        }
    }
}
