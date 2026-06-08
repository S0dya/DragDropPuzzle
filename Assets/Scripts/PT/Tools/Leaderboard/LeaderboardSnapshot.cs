using System.Collections.Generic;
using PT.Backend.Types;

namespace PT.Tools.Leaderboard
{
    public class LeaderboardSnapshot
    {
        public LeaderboardEntry PlayerEntry { get; }
        public IReadOnlyList<LeaderboardEntry> Entries { get; }
        
        public LeaderboardSnapshot(LeaderboardEntry playerEntry, IReadOnlyList<LeaderboardEntry> entries)
        {
            PlayerEntry = playerEntry;
            Entries = entries;
        }
    }
}