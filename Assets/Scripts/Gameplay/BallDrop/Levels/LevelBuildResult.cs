namespace Gameplay.BallDrop.Levels
{
    public class LevelBuildResult
    {
        public LevelSizeChangingTrigger[] SizeChangingTriggers { get; private set; }
        public float LastYPosition { get; private set; }
        public LevelChunk[] LevelChunks { get; private set; }

        public LevelBuildResult(LevelSizeChangingTrigger[] triggers, float pastYPosition, LevelChunk[] levelChunks)
        {
            SizeChangingTriggers = triggers;
            LastYPosition = pastYPosition;
            LevelChunks = levelChunks;
        }
    }
}