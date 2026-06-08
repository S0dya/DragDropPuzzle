using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PT.Backend.Interfaces;
using PT.Backend.Types;
using PT.Tools.Debugging;
using Zenject;

namespace PT.Tools.Leaderboard
{
    public class DatabaseLeaderboardService : ILeaderboardService
    {
        [Inject] private IDatabaseService _db;
        [Inject] private IAuthentificationService _identity;
        [Inject] private LeaderboardConfig _leaderboardConfig;

        public UniTask SetScore(long score)
        {
            return _db.SetDataAsync(
                $"{DatabasePathsKeys.LeaderboardsRoot}{_identity.PlayerId}",
                new LeaderboardRecord
                (
                    _identity.PlayerId, 
                    _identity.DisplayName, 
                    score,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                ));
        }
        
        public async UniTask<LeaderboardSnapshot> GetTop(int count)
        {
            var entries =  await _db.Query<LeaderboardEntry>(new DatabaseQuery
            {
                Path = DatabasePathsKeys.LeaderboardsRoot,
                OrderBy = "score",
                Descending = true,
                Limit = count
            });
            
            var playerEntry = await GetPlayer();
            
            DebugManager.Log(DebugCategory.Leaderboards, $"DB Top fetched | Entries: {entries.Count} | Player rank: {playerEntry.Rank}");
            
            return new LeaderboardSnapshot(playerEntry, entries);
        }

        public async UniTask<LeaderboardSnapshot> GetAroundPlayer(int range)
        {
            var playerEntry = await GetPlayer();
            return new LeaderboardSnapshot(playerEntry, Array.Empty<LeaderboardEntry>());
            
            // return _db.Query<LeaderboardEntry>(new DatabaseQuery
            // {
            //     Path = DatabasePathsKeys.LeaderboardsRoot,
            //     OrderBy = "rank",
            //     StartAt = _playerRank - range,
            //     EndAt = _playerRank + range
            // });
        }
        
        private async UniTask<LeaderboardEntry> GetPlayer()
        {
            var record = await _db.GetData<LeaderboardRecord>(
                $"{DatabasePathsKeys.LeaderboardsRoot}{_identity.PlayerId}" //change later, might not work
            );
            
            DebugManager.Log(DebugCategory.Leaderboards, $"DB player loaded | Score: {record.Score}");

            return new LeaderboardEntry(
                record.PlayerId,
                record.DisplayName,
                record.Score,
                10 //change later, rank needs to be recorded 
            );
        }
    }
}