using PT.Logic.PlatformRelated;
using PT.Logic.Save;
using PT.Tools.Settings.Types;
using UnityEngine;

namespace PT.Tools.Settings.Configs
{
    [CreateAssetMenu(menuName = "Configs/SettingInfoConfig", fileName = "SettingInfoConfig")]
    public class SettingInfoConfig : ScriptableObject
    {
        [SerializeField] private GameDataKey key;
        // [SerializeField] private string displayName;
        [SerializeField] private SettingType type;
        [Space]
        [SerializeField] private PlatformTypeEnum[] skipOnPlatformTypes;
        [Space]
        [SerializeField] private bool defaultBool;
        [SerializeField] private float defaultFloat;
        [SerializeField] private int defaultInt;
        [Space]
        // [SerializeField] private Sprite[] icons;
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;
        [Space]
        [SerializeField] private string[] dropdownOptions;

        public GameDataKey Key => key;
        // public string DisplayName => displayName;
        public SettingType Type => type;

        public PlatformTypeEnum[] SkipOnPlatformTypes => skipOnPlatformTypes;

        public bool DefaultBool => defaultBool;
        public float DefaultFloat => defaultFloat;
        public int DefaultInt => defaultInt;

        // public Sprite[] Icons => icons;
        
        public float MinValue => minValue;
        public float MaxValue => maxValue;

        public string[] DropdownOptions => dropdownOptions;
    }
}