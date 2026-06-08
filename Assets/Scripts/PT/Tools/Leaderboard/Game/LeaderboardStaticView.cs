using System.Collections.Generic;
using System.Linq;
using PT.Backend.Types;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PT.Tools.Leaderboard.Game
{
    public class LeaderboardStaticView : MonoBehaviour
    {
        [SerializeField] private LeaderboardViewInitializer initializer;

        [Inject] private LeaderboardManager _leaderboardManager;

        private async void Awake()
        {
            var result = await _leaderboardManager.GetTopEntries();

            if (!result.success) return;

            Init(result.snapshot.Entries, result.snapshot.PlayerEntry);   
        }
        
        private void Init(IReadOnlyList<LeaderboardEntry> topEntries, LeaderboardEntry playerEntry)
        {
            bool playerInside = topEntries.Any(e => e.PlayerId == playerEntry.PlayerId);

            initializer.SpawnEntries(topEntries);
            if (!playerInside) initializer.SpawnPlayer(playerEntry, false);
        }
    }
}
