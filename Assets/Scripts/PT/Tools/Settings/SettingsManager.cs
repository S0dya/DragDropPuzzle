using System.Collections.Generic;
using System.Linq;
using PT.Logic.Configs;
using PT.Logic.Save;
using PT.Tools.Debugging;
using PT.Tools.Settings.Configs;
using PT.Tools.Settings.Types;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private readonly Dictionary<GameDataKey, ReactiveProperty<object>> _settings = new();

        public bool IsInitialized { get; private set; } = false;

        [Inject] private SettingsInfosConfig _settingsInfosConfigs;
        [Inject] private GameConfig _gameConfig;

        public void Init()
        {
            foreach (var settingInfo in _settingsInfosConfigs.SettingsInfos)
            {
                if (settingInfo.SkipOnPlatformTypes != null && 
                    settingInfo.SkipOnPlatformTypes.Any(x => x == _gameConfig.PlatformType)) 
                    continue;
                
                object currentValue = GameDataRegistry.Get(settingInfo.Key);
                var prop = new ReactiveProperty<object>(GameDataRegistry.Get(settingInfo.Key));
                prop.Subscribe(v => GameDataRegistry.Set(settingInfo.Key, v));
                _settings[settingInfo.Key] = prop;
                
                // _settings[settingInfo.Key] = new ReactiveProperty<object>(currentValue);
            }

            IsInitialized = true;
        }

        public IReadOnlyReactiveProperty<object> Get(GameDataKey key)
        {
            if (_settings.TryGetValue(key, out var property)) return property;
            else 
            {
                DebugManager.Log(DebugCategory.Settings, $"Couldn't get {key} setting", LogType.Warning);
                
                return null;
            }
        } 

        public void Set(GameDataKey key, object value)
        {
            DebugManager.Log(DebugCategory.Settings, $"Set Setting : {key}");
            
            _settings[key].Value = value;
            GameDataRegistry.Set(key, value);
        }
    }
}