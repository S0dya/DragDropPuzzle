using System;
using UnityEngine;

namespace PT.Logic.Configs
{
    public enum LocalizationKeyEnum
    {
        Level,
        
        TutorialDropTitle,
        TutorialDropDesc,
        TutorialSlowMoTitle,
        TutorialSlowMoDesc,
        TutorialBonusRainbowTitle,
        TutorialBonusRainbowDesc,
    }
    
    public enum LanguageEnum
    {
        none = -1,
        
        En = 0,
        Ru = 1,
    }
    
    [CreateAssetMenu(menuName = "Configs/LanguageConfig", fileName = "LanguageConfig")]
    public class LanguageConfig : ScriptableObject
    {
        [Serializable]
        public class KVStringLocalization
        {
            [SerializeField] private LocalizationKeyEnum localizationKey;
            [Space]
            [SerializeField] private string[] localizationValues;

            public LocalizationKeyEnum LocalizationKey => localizationKey;
            public string[] LocalizationValues => localizationValues;
        }

        [SerializeField] private KVStringLocalization[] kvStringsLocalizations;
        public KVStringLocalization[] KVStringsLocalizations => kvStringsLocalizations;
    }
}