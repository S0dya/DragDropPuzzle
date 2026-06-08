using System;
using PT.Logic.Save;
using PT.Tools.Debugging;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Settings.Handlers
{
    public abstract class SettingHandler : IInitializable, IDisposable
    {
        protected abstract GameDataKey _key { get; }
        protected IDisposable _subscription;

        private SettingsManager _settingsManager;
        
        protected SettingHandler(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }
        
        public virtual void Initialize()
        {
            if (!_settingsManager.IsInitialized)
            {
                DebugManager.Log(DebugCategory.Settings, $"SettingsManager not ready for {_key}, retrying…", LogType.Warning);
                
                Observable.EveryUpdate()
                    .First(_ => _settingsManager.IsInitialized)
                    .Subscribe(_ => SubscribeNow());
            }
            else
            {
                SubscribeNow();
            }
        }

        private void SubscribeNow()
        {
            var prop = _settingsManager.Get(_key);
            if (prop == null)
            {
                DebugManager.Log(DebugCategory.Settings, $"No property for {_key}", LogType.Warning);
                return;
            }

            _subscription = prop
                .Skip(1)
                .Subscribe(OnSettingChanged);
            
        }

        protected abstract void OnSettingChanged(object value);

        public virtual void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}