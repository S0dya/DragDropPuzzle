using PT.Tools.Debugging;
using UnityEngine;

namespace PT.GameplayAdditional.Cameras
{
    public class CamerasFOVChanger : MonoBehaviour
    {
        [SerializeField] private Camera[] cameras;
        [SerializeField] private float levelWidth = 10f;
        [SerializeField] private float levelHeight = 15f;
        [SerializeField] private bool changeRuntime = true;

        private ScreenOrientation _lastOrientation;

        private void Awake()
        {
            _lastOrientation = Screen.orientation;
            UpdateOrtho();
        }

        private void Update()
        {
            if (changeRuntime && Screen.orientation != _lastOrientation)
            {
                _lastOrientation = Screen.orientation;
                UpdateOrtho();
            }
        }

        public void UpdateOrtho()
        {
            foreach (var camera in cameras)
            {
                float halfHeight = levelHeight / 2f;
                float halfWidth = (levelWidth / 2f) / camera.aspect;

                float orthoSize = Mathf.Max(halfHeight, halfWidth);
                camera.orthographicSize = orthoSize;

                DebugManager.Log(DebugCategory.Misc, $"Updated camera ortho size: {orthoSize} (aspect {camera.aspect})");
            }
        }
    }
}