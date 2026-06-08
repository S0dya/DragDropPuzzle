using PT.Logic.ProjectContext;
using PT.Logic.Save;

namespace PT.Tools.Settings.Handlers
{
    public class VibroSettingHandler : SettingHandler
    {
        protected override GameDataKey _key => GameDataKey.VibroOn;
        
        // private readonly AudioManager _audioManager;

        public VibroSettingHandler(SettingsManager settingsManager) : base(settingsManager)
        {
        }

        protected override void OnSettingChanged(object value)
        {
            // var isEnabled = (bool)value;
            // _audioManager.ToggleSound(isEnabled);
        }
    }
}