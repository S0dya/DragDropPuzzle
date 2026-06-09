using UnityEngine;

namespace PT.Logic.Configs
{
    [CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig")]
    public class GameConfig : BaseGameConfig
    {
        [Space(20)]
        [Header("GAME settings:")]
        [SerializeField] private float gameEndDelay = 1;
        public float GameEndDelay => gameEndDelay;

        [Space(20)]
        [Header("DRAG DROP PUZZLE settings:")]
        [SerializeField] private float snapDistance = 1f;
        [SerializeField] private float snapSpeed = 15f;
        [SerializeField] private float unsnapDistance = 1.5f;
        [SerializeField] private float snapTolerance = 0.01f;
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private int handItemPoolCapacity = 20;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        public float SnapDistance => snapDistance;
        public float SnapSpeed => snapSpeed;
        public float UnsnapDistance => unsnapDistance;
        public float SnapTolerance => snapTolerance;
        public float AnimationDuration => animationDuration;
        public int HandItemPoolCapacity => handItemPoolCapacity;
        public Color HighlightColor => highlightColor;
    }
}