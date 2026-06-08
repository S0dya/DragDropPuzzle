using PT.Logic.ProjectContext;
using PT.Logic.Save;
using UnityEngine;

namespace PT.Tools.Settings.Handlers
{
    public class SoundSettingHandler : SettingHandler
    {
        protected override GameDataKey _key => GameDataKey.SoundOn;
        
        private readonly AudioManager _audioManager;

        public SoundSettingHandler(AudioManager audioManager, SettingsManager settingsManager) : base(settingsManager)
        {
            _audioManager = audioManager;
        }

        protected override void OnSettingChanged(object value)
        {
            _audioManager.ToggleSound((bool)value);
        }
    }
}