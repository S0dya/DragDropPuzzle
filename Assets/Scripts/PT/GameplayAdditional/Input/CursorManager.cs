using UnityEngine;
using Zenject;

namespace PT.GameplayAdditional.Input
{
    public class CursorManager : MonoBehaviour
    {
        
        [Inject] private SignalBus _signalBus;

        private void Awake()
        {
            
            // _signalBus.Subscribe<GameMenuOpenedSignal>(OnToggleVisibleOn);
            // _signalBus.Subscribe<GameMenuClosedSignal>(OnToggleVisibleOff);
            // _signalBus.Subscribe<AdClosedSignal>(OnToggleVisibleOn);
            // _signalBus.Subscribe<AdOpenedSignal>(OnToggleVisibleOff);
        }
        private void Start()
        {
            OnToggleVisibleOff();
        }
        void OnDisable()
        {
            OnToggleVisibleOn();
        }

        private void OnToggleVisibleOn()
        {
            Cursor.visible = true;

#if UNITY_WEBGL
            Cursor.lockState = CursorLockMode.None;
#endif
        }
        private void OnToggleVisibleOff()
        {
            Cursor.visible = false;

#if UNITY_WEBGL
            Cursor.lockState = CursorLockMode.Locked;
#endif
        }
    }
}
