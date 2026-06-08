using PT.Logic.PersistentScene;
using PT.Tools.Helper;
using UnityEngine;
using Zenject;

namespace PT.GameplayAdditional.Cameras
{
    public class LoadingCamera : MonoBehaviour
    {
        [SerializeField] private Camera loadingCamera;
        
        [Inject] private LoadingManager _loadingManager;

        private void Awake()
        {
            ToggleCamera(false);
            
            _loadingManager.OnLoadStarted += LoadStarted;
            _loadingManager.OnLoadFinished += LoadFinished;
        }

        private void LoadStarted() => ToggleCamera(true);
        private void LoadFinished() => ToggleCamera(false);
        
        private void ToggleCamera(bool isLoading)
        {
            loadingCamera.SetActive(isLoading);
        }
        
        private void OnDestroy()
        {
            _loadingManager.OnLoadStarted -= LoadStarted;
            _loadingManager.OnLoadFinished -= LoadFinished;
        }
    }
}