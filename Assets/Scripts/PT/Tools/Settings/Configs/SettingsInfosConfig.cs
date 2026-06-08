using UnityEngine;

namespace PT.Tools.Settings.Configs
{
    [CreateAssetMenu(menuName = "Configs/SettingInfosConfig", fileName = "SettingInfosConfig")]
    public class SettingsInfosConfig : ScriptableObject
    {
        [SerializeField] private SettingInfoConfig[] settingsInfos;
        
        public SettingInfoConfig[] SettingsInfos => settingsInfos;
    }
}