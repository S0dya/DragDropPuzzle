using UnityEngine;

namespace Gameplay.BallDrop.Levels
{
    public class LevelSizeChangingTrigger : MonoBehaviour
    {
        [SerializeField] private float scaleSize = 0.2f;
        
        public float ScaleSize => scaleSize;
    }
}