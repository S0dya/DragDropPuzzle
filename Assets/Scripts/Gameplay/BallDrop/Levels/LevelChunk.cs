using PT.Tools.Helper;
using UnityEngine;

namespace Gameplay.BallDrop.Levels
{
    public class LevelChunk : MonoBehaviour
    {
        [SerializeField] private LevelSizeChangingTrigger sizeChangingTrigger;
        [SerializeField] private float height;
        [Space]
        [SerializeField] private Transform disablableContents;
        
        public LevelSizeChangingTrigger SizeChangingTrigger => sizeChangingTrigger;
        public float Height => height;

        public void Toggle(bool toggle)
        {
            disablableContents.SetActive(toggle);
        }
    }
}