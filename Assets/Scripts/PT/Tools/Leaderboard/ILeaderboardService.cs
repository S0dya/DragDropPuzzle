using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PT.Backend.Types;

namespace PT.Tools.Leaderboard
{
    public interface ILeaderboardService
    {
        UniTask SetScore(long score);
        UniTask<LeaderboardSnapshot> GetTop(int count);
        UniTask<LeaderboardSnapshot> GetAroundPlayer(int range);
    }
}