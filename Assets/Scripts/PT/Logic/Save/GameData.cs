using PT.Logic.Configs;

namespace PT.Logic.Save
{
    public enum GameDataKey
    {
        SoundOn,
        Language,
        HighestScore,
        
        VibroOn,
        
        LevelIndex,
        
        Gold,
        
        SkillSwap,
        SkillDestroy,
        
        LeaderboardPlayerRank,
        LeaderboardPlayerName,
        
        UnlockedSongs,
        UnlockedSingers
    }
    
    public static class GameData
    {
        public static bool SoundOn { get; set; } = true;
        public static LanguageEnum Language { get; internal set; } = LanguageEnum.Ru;
        public static int HighestScore { get; internal set; }
        
        public static bool VibroOn { get; internal set; } = true;
        
        public static int LevelIndex { get; internal set; }
        
        public static int Gold { get; internal set; }
        
        public static int SkillSwap { get; internal set; } = 3;
        public static int SkillDestroy { get; internal set; } = 3;
        
        public static int LeaderboardPlayerRank { get; internal set; }
        public static string LeaderboardPlayerName { get; internal set; }

        public static int UnlockedSongs { get; internal set; } = 1000;
        public static int UnlockedSingers { get; internal set; } = 1000;
    }
}
