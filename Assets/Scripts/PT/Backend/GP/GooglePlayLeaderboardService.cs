#if UNITY_IOS || UNITY_ANDROID
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PT.Backend.Types;
using PT.Tools.Debugging;
using PT.Tools.Leaderboard;
using Zenject;

namespace PT.Backend.GP
{
    public class GooglePlayLeaderboardService : ILeaderboardService
    {
        [Inject] private LeaderboardConfig _leaderboardConfig;
        
        public async UniTask SetScore(long score)
        {
            PlayGamesPlatform.Instance.ReportScore(
                score, 
                _leaderboardConfig.LeaderboardId,
                success =>
                {
                    DebugManager.Log(DebugCategory.Leaderboards, $"Leaderboard score submit: {success}");
                });    
        }
        
        public async UniTask<LeaderboardSnapshot> GetTop(int count)
        {
            var entries = await LoadScores(LeaderboardStart.TopScores, count);
            var playerEntry = await GetPlayer();
            return new LeaderboardSnapshot(playerEntry, entries);
        }

        public async UniTask<LeaderboardSnapshot> GetAroundPlayer(int range)
        {
            var entries = await LoadScores(LeaderboardStart.PlayerCentered, range * 2 + 1);
            var playerEntry = entries.FirstOrDefault(e =>
                e.PlayerId == PlayGamesPlatform.Instance.localUser.id);
            return new LeaderboardSnapshot(playerEntry, entries);
        }
        
        private async UniTask<LeaderboardEntry> GetPlayer()
        {
            var tcs = new UniTaskCompletionSource<LeaderboardEntry>();

            PlayGamesPlatform.Instance.LoadScores(
                _leaderboardConfig.LeaderboardId,
                LeaderboardStart.PlayerCentered,
                1,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                data =>
                {
                    if (!data.Valid || data.Scores.Length == 0)
                    {
                        DebugManager.Log(DebugCategory.Leaderboards, "GP player score NOT FOUND");
                        tcs.TrySetResult(new());
                        return;
                    }
                    
                    var score = data.Scores[0];
                    
                    DebugManager.Log(DebugCategory.Leaderboards, $"GP player loaded | Rank: {score.rank} | Score: {score.value}");

                    tcs.TrySetResult(new LeaderboardEntry(
                        score.userID,
                        PlayGamesPlatform.Instance.localUser.userName,
                        score.value,
                        score.rank
                    ));
                });

            return await tcs.Task;
        }

        private UniTask<IReadOnlyList<LeaderboardEntry>> LoadScores(LeaderboardStart start, int count)
        {
            var tcs = new UniTaskCompletionSource<IReadOnlyList<LeaderboardEntry>>();
            
            PlayGamesPlatform.Instance.LoadScores(
                _leaderboardConfig.LeaderboardId, start, count, 
                LeaderboardCollection.Public, 
                LeaderboardTimeSpan.AllTime, 
                data =>
            {
                if (!data.Valid)
                {
                    DebugManager.Log(DebugCategory.Leaderboards, "GP LoadScores failed");
                    tcs.TrySetResult(new List<LeaderboardEntry>());
                    return;
                }
                
                DebugManager.Log(DebugCategory.Leaderboards, $"GP LoadScores success | Count: {data.Scores.Length}");
                
                var list = data.Scores.Select(score => new LeaderboardEntry(
                    score.userID,
                    score.userID == PlayGamesPlatform.Instance.localUser.id
                        ? PlayGamesPlatform.Instance.localUser.userName
                        : score.formattedValue, 
                    score.value,
                    score.rank))
                    .ToList();
                
                tcs.TrySetResult(list);
            });

            return tcs.Task;
        }
    }
}
#endif