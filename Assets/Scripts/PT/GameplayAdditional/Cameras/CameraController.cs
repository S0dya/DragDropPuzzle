using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace PT.GameplayAdditional.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private Transform defaultTarget;
        [SerializeField] private Transform proxyTransform;
        
        private Transform _currentTarget;
        private CinemachineBasicMultiChannelPerlin _perlin;

        private void Awake()
        {
            // _perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        public void SetTarget(Transform newTarget)
        {
            if (_currentTarget == newTarget) return;
            
            _currentTarget = newTarget;
            virtualCamera.Follow = proxyTransform;
        }

        private void Update()
        {
            if (_currentTarget != null)
            {
                proxyTransform.position = new Vector2(0, _currentTarget.position.y);
            }
        }

        public void ClearTarget()
        {
            SetTarget(defaultTarget);
        }

        public async void Shake(float amplitude, float duration)
        {
            if (_perlin == null) return;

            // _perlin.m_AmplitudeGain = amplitude;
            // _perlin.m_FrequencyGain = 2f; 

            await UniTask.Delay((int)(duration * 1000));

            // _perlin.m_AmplitudeGain = 0f;
            // _perlin.m_FrequencyGain = 0f;
        }
    }
}