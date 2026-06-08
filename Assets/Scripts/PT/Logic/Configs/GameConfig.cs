using UnityEngine;

namespace PT.Logic.Configs
{
    [CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig")]
    public class GameConfig : BaseGameConfig
    {
        [Space(20)]
        [Header("GAME settings:")]
        [SerializeField] private float lowestBallYOffset = 5;
        [SerializeField] private float gameEndDelay = 1;
        [Space]
        [SerializeField] private float vocalsRampSeconds = 0.25f;
        [SerializeField] private float songEndingBeforeFinishSeconds = 5f;
        [Space]
        [SerializeField] private int minSingersAmount = 2;
        [SerializeField] private int maxSingersAmount = 5;
        [SerializeField] private int maxTotalSingersAmount = 15;
        [Space]
        [SerializeField] private float levelChunksOpeningYOffset = 14;
        [SerializeField] private int levelChunksInitialEnabled = 3;
        [SerializeField] private int levelChunksTopDisableStep = 3;
        [Space]
        [SerializeField] private float pushSlowMoScale = 0.2f;
        [SerializeField] private float pushSlowMoInDuration = 0.5f;
        [SerializeField] private float pushPower = 10f;
        [SerializeField] private float pushMaxDuration = 2f;
        [SerializeField] private float pushCooldownDuration = 3f;
        [Space]
        [SerializeField] private int trajectoryPoints = 50;
        [SerializeField] private float trajectoryTimeStep = 0.05f;
        [Space]
        [SerializeField] private int goldAmountForLevelFinish = 150;


        public float LowestBallYOffset => lowestBallYOffset;
        public float GameEndDelay => gameEndDelay;
        
        public float VocalsRampSeconds => vocalsRampSeconds;
        public float SongEndingBeforeFinishSeconds => songEndingBeforeFinishSeconds;
        
        public int MinSingersAmount => minSingersAmount;
        public int MaxSingersAmount => maxSingersAmount;
        public int MaxTotalSingersAmount => maxTotalSingersAmount;
        
        public float LevelChunksOpeningYOffset => levelChunksOpeningYOffset;
        public int LevelChunksInitialEnabled => levelChunksInitialEnabled;
        public int LevelChunksTopDisableStep => levelChunksTopDisableStep;

        public float PushSlowMoScale => pushSlowMoScale;
        public float PushSlowMoInDuration => pushSlowMoInDuration;
        public float PushPower => pushPower;
        public float PushMaxDuration => pushMaxDuration;
        public float PushCooldownDuration => pushCooldownDuration;
        public int TrajectoryPoints => trajectoryPoints;
        public float TrajectoryTimeStep => trajectoryTimeStep;
        
        public int GoldAmountForLevelFinish => goldAmountForLevelFinish;
        
        // public Vector2Int InitialAddedElements =>
        //     new Vector2Int(
        //         RCInt(RemoteConfigKeys.InitialAddedElementsMin, initialAddedElements.x),
        //         RCInt(RemoteConfigKeys.InitialAddedElementsMax, initialAddedElements.y)
        //     );
        //
        // public bool StrictPushDirections =>
        //     RCBool(RemoteConfigKeys.StrictPushDirections, strictPushDirections);
        //
        // public float MegaMergeChargePerMerge =>
        //     RCFloat(RemoteConfigKeys.MegaMergeChargePerMerge, megaMergeChargePerMerge);
    }
}