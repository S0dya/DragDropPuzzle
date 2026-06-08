using PT.Backend.Interfaces;
using PT.Logic.PlatformRelated;
using UnityEngine;
using Zenject;

namespace PT.Logic.Configs
{
    [CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig")]
    public class BaseGameConfig : ScriptableObject
    {
        [Inject(Optional = true)] protected IRemoteConfigService _remoteConfig;
        
        [SerializeField] private PlatformTypeEnum platformType;
        [Space]
        [SerializeField] private float loadingAnimationDuration = 0.01f;
        [Space] 
        [SerializeField][Min(1)] private float addressablesDiagnosticsInterval = 60;
        [Space]
        [SerializeField] private float cloudSaveCooldownSeconds = 120f;
        
        protected int RCInt(string key, int local) => _remoteConfig?.GetInt(key, local) ?? local;
        protected float RCFloat(string key, float local) => _remoteConfig?.GetFloat(key, local) ?? local;
        protected bool RCBool(string key, bool local) => _remoteConfig?.GetBool(key, local) ?? local;
        protected string RCString(string key, string local) => _remoteConfig != null ? _remoteConfig.GetString(key, local) : local;
        
        public PlatformTypeEnum PlatformType => platformType;
        
        public float LoadingAnimationDuration => loadingAnimationDuration;
        
        public float AddressablesDiagnosticsInterval => addressablesDiagnosticsInterval;
        
        public float CloudSaveCooldownSeconds => cloudSaveCooldownSeconds;
    }
}