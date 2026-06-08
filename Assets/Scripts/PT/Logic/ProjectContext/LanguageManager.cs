using Cysharp.Threading.Tasks;
using PT.Logic.Save;
using System.Linq;
using PT.Logic.Configs;
using PT.Logic.PlatformRelated;
using PT.Tools.Debugging;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;

#if UNITY_WEBGL
using YG;
#endif

namespace PT.Logic.ProjectContext
{
    public class LanguageManager : MonoBehaviour
    {
        [Inject] private GameConfig _gameConfig;
        [Inject] private LanguageConfig _languageConfig;

        private bool _canChangeLanguage = true;

        public async UniTask InitAsync()
        {
#if UNITY_WEBGL
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex)
            {
                YG2.onSwitchLang += OnYandexLanguageChanged;
                YG2.onCorrectLang += OnYandexLanguageChanged;
                
                DebugManager.Log(DebugCategory.Language, "[LanguageManager] Subscribed to Yandex SwitchLangEvent");
            }
#endif
            
            Init();
            
            DebugManager.Log(DebugCategory.Language, $"InitAsync → Current Language: {GameData.Language}");

            await ChangeLanguageAsync(GameData.Language);
        }

        private void Init()
        {
#if UNITY_WEBGL
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex)
            {
                string currentLang = YG2.lang;

                GameData.Language = currentLang switch
                {
                    "ru" => LanguageEnum.Ru,
                    "en" => LanguageEnum.En,
                    _ => LanguageEnum.En
                };

                DebugManager.Log(DebugCategory.Language,$"Init (WebGL-Yandex) → Overriding with YG2.lang = {currentLang} → GameData.Language = {GameData.Language}");
            }
            else
            {
                // ✅ Non-Yandex WebGL fallback
                if (GameData.Language == LanguageEnum.none)
                {
                    GameData.Language = LanguageEnum.En;
                    DebugManager.Log(DebugCategory.Language, "Init (WebGL-Other) → Defaulting to English");
                }
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (GameData.Language == LanguageEnum.none)
            {
                string languageCode = Application.systemLanguage == SystemLanguage.Russian ? "ru" : "en";
                GameData.Language = languageCode == "ru" ? LanguageEnum.Ru : LanguageEnum.En;
                DebugManager.Log(DebugCategory.Language, 
                    $"Init (Mobile) → System language: {Application.systemLanguage}, Set to {GameData.Language}");
            }
#else
            if (GameData.Language == LanguageEnum.none)
            {
                GameData.Language = LanguageEnum.En;
                DebugManager.Log(DebugCategory.Language, "Init (Editor/Other) → Defaulting to English");
            }
#endif
        }

        public bool ChangeLanguageIfPossibleAsync(LanguageEnum language)
        {
            bool temp = _canChangeLanguage;

            DebugManager.Log(DebugCategory.Language, $"ChangeLanguageIfPossibleAsync → Requested: {language}, Allowed: {temp}");

            if (temp) ChangeLanguageAsync(GameData.Language = language).Forget();

            return temp;
        }

        public string GetLocalizedString(LocalizationKeyEnum key)
        {
            string result = _languageConfig.KVStringsLocalizations.First(x => x.LocalizationKey == key).LocalizationValues[(int)GameData.Language];
            DebugManager.Log(DebugCategory.Language, $"GetLocalizedString → Key: {key}, Value: {result}, CurrentLang: {GameData.Language}");
            return result;
        }

        private async UniTask ChangeLanguageAsync(LanguageEnum locale)
        {
            DebugManager.Log(DebugCategory.Language, $"ChangeLanguageAsync → Changing to {locale}...");

            _canChangeLanguage = false;

            await LocalizationSettings.InitializationOperation.Task;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)locale];

            DebugManager.Log(DebugCategory.Language, $"ChangeLanguageAsync → Changed to {locale}");

            _canChangeLanguage = true;
        }
        
        private async void OnYandexLanguageChanged(string newLang)
        {
            DebugManager.Log(DebugCategory.Language, $"OnYandexLanguageChanged → {newLang}");

            var newLanguageEnum = newLang == "ru" ? LanguageEnum.Ru : LanguageEnum.En;

            if (newLanguageEnum != GameData.Language)
            {
                GameData.Language = newLanguageEnum;
                await ChangeLanguageAsync(GameData.Language);
            }
        }
        
#if UNITY_WEBGL
        private void OnDestroy()
        {
            if (_gameConfig.PlatformType == PlatformTypeEnum.Yandex)
            {
                YG2.onSwitchLang -= OnYandexLanguageChanged;
                YG2.onCorrectLang -= OnYandexLanguageChanged;
            }
        }
#endif
    }
}