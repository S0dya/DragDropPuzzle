using PT.Tools.Debugging;
using UnityEngine;
using UnityEngine.Events;

namespace PT.GameplayAdditional.Trigger
{
    public class WorldEventTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent triggerEnterEvent;
        [SerializeField] private UnityEvent triggerExitEvent;

        protected virtual void OnTriggerEnter(Collider other)
        {
            DebugManager.Log(DebugCategory.Gameplay, "World Event Triggered Enter");

            triggerEnterEvent?.Invoke();
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            DebugManager.Log(DebugCategory.Gameplay, "World Event Triggered Exit");

            triggerExitEvent?.Invoke();
        }
    }
}