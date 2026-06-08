using NaughtyAttributes;
using PT.Tools.Helper;
using UnityEngine;

namespace PT.Tools.Leaderboard
{
    [CreateAssetMenu(menuName = "Configs/LeaderboardConfig", fileName = "LeaderboardConfig")]
    public class LeaderboardConfig : ScriptableObject
    {
        [SerializeField] private LeaderboardType leaderboardType;
        [Space]
        [SerializeField] private int topAmount = 10;
        [SerializeField] private int aroundPlayerAmount = 15;
        
        [Header("Google Play")]
        [SerializeField] private string leaderboardId;
        
        [Header("Firebase")]
        [SerializeField] private int firebaseRankWindowFetch = 200;
        
        [Header("Fake")]
        [SerializeField] private SerializableKeyValue<string, long> entries;
        [Space]
        [SerializeField] private string initialPlayerName = "You";
        [SerializeField] private string playerId = "Player";
        [SerializeField] private int initialPlayerRank = 9000;
        [SerializeField][MinMaxSlider(1, 100)] private Vector2Int scoreAddition = new (1, 40);
        [SerializeField][Min(1)] private int rankAdditionDelta = 100;
        
        public LeaderboardType LeaderboardType => leaderboardType;
        
        public int TopAmount => topAmount;
        public int AroundPlayerAmount => aroundPlayerAmount;
        
        public string LeaderboardId => leaderboardId;
        
        public int FirebaseRankWindowFetch => firebaseRankWindowFetch;
        
        public SerializableKeyValue<string, long> Entries => entries;
        public string InitialPlayerName => initialPlayerName;
        public string PlayerId => playerId;
        public int InitialPlayerRank => initialPlayerRank;
        public Vector2Int ScoreAddition => scoreAddition;
        public int RankAdditionDelta => rankAdditionDelta;
    }
}