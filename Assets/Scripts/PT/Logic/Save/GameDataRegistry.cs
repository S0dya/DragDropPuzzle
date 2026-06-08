using System;
using System.Collections.Generic;
using PT.Logic.Configs;
using PT.Logic.ProjectContext;
using PT.Tools.Debugging;

namespace PT.Logic.Save
{
    public static class GameDataRegistry
    {
        private static readonly Dictionary<GameDataKey, Func<object>> _getters = new();
        private static readonly Dictionary<GameDataKey, Action<object>> _setters = new();

        static GameDataRegistry()
        {
            Register(GameDataKey.HighestScore, () => GameData.HighestScore, v => GameData.HighestScore = (int)v);
            
            Register(GameDataKey.SoundOn, () => GameData.SoundOn, v => GameData.SoundOn = (bool)v);
            Register(GameDataKey.VibroOn, () => GameData.VibroOn, v => GameData.VibroOn = (bool)v);
            
            Register(GameDataKey.LevelIndex, () => GameData.LevelIndex, v => GameData.LevelIndex = (int)v);
            
            Register(GameDataKey.Gold, () => GameData.Gold, v => GameData.Gold = (int)v);
            
            Register(GameDataKey.Language, () => GameData.Language, 
                v => { if (v is string s) GameData.Language = Enum.Parse<LanguageEnum>(s); else GameData.Language = (LanguageEnum)v; });
            
            Register(GameDataKey.SkillSwap, () => GameData.SkillSwap, v => GameData.SkillSwap = (int)v);
            Register(GameDataKey.SkillDestroy, () => GameData.SkillDestroy, v => GameData.SkillDestroy = (int)v);
            
            Register(GameDataKey.LeaderboardPlayerName, () => GameData.LeaderboardPlayerName, v => GameData.LeaderboardPlayerName = (string)v);
            Register(GameDataKey.LeaderboardPlayerRank, () => GameData.LeaderboardPlayerRank, v => GameData.LeaderboardPlayerRank = (int)v);

            Register(GameDataKey.UnlockedSongs, () => GameData.UnlockedSongs, v => GameData.UnlockedSongs = (int)v);
            Register(GameDataKey.UnlockedSingers, () => GameData.UnlockedSingers, v => GameData.UnlockedSingers = (int)v);
        }

        private static void Register(GameDataKey key, Func<object> getter, Action<object> setter)
        {
            _getters[key] = getter;
            _setters[key] = setter;
        }

        public static object Get(GameDataKey key)
        {
            DebugManager.Log(DebugCategory.Save, $"Getting {key} : {_getters[key]()}");
            
            return _getters[key]();
        }

        public static void Set(GameDataKey key, object value)
        {
            _setters[key](value);
            
            DebugManager.Log(DebugCategory.Save, $"Setting {key} : {_getters[key]()}");
        }
        
        public static Type GetValueType(GameDataKey key)
        {
            if (_getters.TryGetValue(key, out var getter))
            {
                var val = getter();
                return val?.GetType();
            }
            return null;
        }
    }
}