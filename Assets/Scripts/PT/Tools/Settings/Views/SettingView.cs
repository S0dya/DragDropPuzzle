using System;
using PT.Logic.Save;
using UniRx;
using UnityEngine;
using Zenject;

namespace PT.Tools.Settings.Views
{
    public abstract class SettingView : MonoBehaviour
    {
        [SerializeField] protected GameDataKey key;
        [Space(20)]
        
        [Inject] private SettingsManager _settingsManager;
        
        protected IDisposable _subscription;
        
        public virtual void Init()
        {
            var reactive = _settingsManager.Get(key);
            _subscription = reactive.Subscribe(OnSettingChanged);
        }

        protected abstract void OnSettingChanged(object value);
        protected abstract void OnUserChangedValue(object value);

        protected void SetValue(object value) => _settingsManager.Set(key, value);

        protected virtual void OnDestroy() => _subscription?.Dispose();
    }
}