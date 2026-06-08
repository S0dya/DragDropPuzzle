using System;
using UnityEditor;
using UnityEngine;

namespace PT.Tools.Debugging
{
    #region enums

    [Flags]
    public enum DebugCategory
    {
        None = 0,
        Points = 1 << 0,
        Errors = 1 << 1,
        AI = 1 << 2,
        UI = 1 << 3,
        Gameplay = 1 << 4,
        Misc = 1 << 5,
        Observer = 1 << 6,

        Backend = 1 << 7,
        Input = 1 << 8,

        Save = 1 << 9,
        Time = 1 << 10,
        Effects = 1 << 11,
    
        Sequence = 1 << 12,
        Tutorial = 1 << 13,
    
        Language = 1 << 14,
        Settings = 1 << 15,
    
        Loading = 1 << 16,
    
        Ads = 1 << 17,
        Audio = 1 << 18,
        Vibration = 1 << 19,
    
        Bonuses = 1 << 20,
    
        Shockwave = 1 << 21,
        Skills = 1 << 22,
        
        Leaderboards = 1 << 23,
        Addressables = 1 << 24,
    
        All = ~0
    }

    #endregion

    public static class DebugManager
    {
        private static DebugCategory _activeCategories;

        private const string _debugPrefsKey = "DebugManagerCategories";

        static DebugManager()
        {
#if UNITY_EDITOR
            LoadDebugCategories();
#else 
    _activeCategories = DebugCategory.All; 
    // Debug.unityLogger.logEnabled = false;
#endif
        }

        public static void EnableDebug(DebugCategory category)
        {
            _activeCategories |= category;
        }

        public static void DisableDebug(DebugCategory category)
        {
            _activeCategories &= ~category;
        }

        public static bool IsDebugEnabled(DebugCategory category) => _activeCategories.HasFlag(category);

        public static void Log(DebugCategory category, string message, LogType logType = LogType.Log)
        {
            if (category == DebugCategory.None) return;
            if (!IsDebugEnabled(category)) return;

            string color = GetCategoryColorHex(category);
            string prefix = $"<color=#{color}>[{category}]</color> ";

            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(prefix + message);
                    break;

                case LogType.Assert:
                    Debug.Log(prefix + "<b>ASSERT:</b> " + message);
                    break;

                case LogType.Warning:
                    Debug.LogWarning(prefix + "<b>WARNING:</b> " + message);
                    break;

                case LogType.Error:
                    Debug.LogError(prefix + "<b>ERROR:</b> " + message);
                    break;
            }
        }
        
        private static string GetCategoryColorHex(DebugCategory category)
        {
            var hash = ((int)category * 2654435761) & 0xFFFFFF;

            float hue = (hash % 360) / 360f;
            float saturation = 0.65f;
            float value = 0.9f;

            Color color = Color.HSVToRGB(hue, saturation, value);
            return ColorUtility.ToHtmlStringRGB(color);
        }

        #region save, load, editor
#if UNITY_EDITOR
        public static void SaveDebugCategories()
        {
            EditorPrefs.SetInt(_debugPrefsKey, (int)_activeCategories);
        }
        private static void LoadDebugCategories()
        {
            _activeCategories = EditorPrefs.HasKey(_debugPrefsKey) ?
                (DebugCategory)EditorPrefs.GetInt(_debugPrefsKey) :
                DebugCategory.None;
        }
#endif
        #endregion
    }
}