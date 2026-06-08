using Gameplay.BallDrop.Balls;
using PT.Logic.PersistentScene;
using UnityEngine;

namespace PT.Logic.Dependency.Signals
{
    public class ShowAdSignal { }
    public class AdOpenedSignal { }
    public class AdClosedSignal { }
    public class AdCountdownStartSignal { }
    public class AdCountdownStopSignal { }

    public class SceneLoadSignal
    {
        public SceneNameEnum SceneName { get; private set; }
        public SceneLoadSignal(SceneNameEnum sceneName) { SceneName = sceneName; }
    }
    public class SceneUnloadedSignal
    {
        public SceneNameEnum SceneName { get; private set; }
        public SceneUnloadedSignal(SceneNameEnum sceneName) { SceneName = sceneName; }
    }
    
    public class GameMenuOpenedSignal { }
    public class GameMenuClosedSignal { }
    
    public class GameStartedSignal { }
    public class GameEndedSignal { }
    public class GameVictorySignal { }
    public class GameReplaySignal { }

    public class GameIntroOpenedSignal { }
    public class GameIntroClosedSignal { }
    
    public class GameAddCoinsSignal
    {
        public int Amount { get; private set; }
        public GameAddCoinsSignal(int amount) { Amount = amount; }
    }
    
    public class GameSpendCoinsSignal
    {
        public int Amount { get; private set; }
        public GameSpendCoinsSignal(int amount) { Amount = amount; }
    }
    
    public class NewHighestScoreReachedSignal { } 
    
    public class NewLowestBallSignal
    {
        public Ball Ball { get; private set; }
        public NewLowestBallSignal(Ball ball) { Ball = ball; }
    }
    
    public class BallReachedFinishSignal
    {
        public Ball Ball { get; private set; }
        public BallReachedFinishSignal(Ball ball) { Ball = ball; }
    }
    
    public class GameIntroTimerPassedSignal { } 
    public class GameSongEndingSignal { } 
}