using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using PT.Logic.Save;

namespace PT.Tools.Settings.Handlers
{
    public class LanguageSettingHandler : SettingHandler
    {
        protected override GameDataKey _key => GameDataKey.Language;
        
        private readonly LanguageManager _languageManager;

        public LanguageSettingHandler(LanguageManager languageManager, SettingsManager settingsManager) : base(settingsManager)
        {
            _languageManager = languageManager;
        }

        protected override void OnSettingChanged(object value)
        {
            if (value is LanguageEnum lang) _languageManager.ChangeLanguageIfPossibleAsync(lang);
            else if (value is int i) _languageManager.ChangeLanguageIfPossibleAsync((LanguageEnum)i);
        }
    }
}