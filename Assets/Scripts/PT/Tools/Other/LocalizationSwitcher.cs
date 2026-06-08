using System;
using PT.Tools.Debugging;
using PT.Tools.Helper;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace PT.Tools.Other
{
    public class LocalizationSwitcher : MonoBehaviour
    {
        [Tooltip("ru, en, etc.")]
        [SerializeField] private SerializableKeyValue<string, GameObject> localizationObjects;
        
        protected virtual void Awake() 
        {
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }
        protected virtual void Start()
        {
            LocalizationSwitch();
        }
        
        private void OnSelectedLocaleChanged(Locale obj) => LocalizationSwitch();
        protected virtual void LocalizationSwitch()
        {
            if (LocalizationSettings.SelectedLocale == null)
            {
                DebugManager.Log(DebugCategory.Errors, "LocalizationSwitcher: SelectedLocale is null."); return;
            }

            var currentLocaleCode = LocalizationSettings.SelectedLocale.Identifier.Code;

            foreach (var localizationObject in localizationObjects.Dictionary)
            {
                bool shouldEnable = localizationObject.Key.Equals(currentLocaleCode, StringComparison.OrdinalIgnoreCase);
                localizationObject.Value.SetActive(shouldEnable);
            }
        }
        
        protected virtual void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }
    }
}