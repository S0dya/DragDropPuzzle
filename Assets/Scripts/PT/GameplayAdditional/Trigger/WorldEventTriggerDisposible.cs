using UnityEngine;
using UnityEngine.Events;

namespace PT.GameplayAdditional.Trigger
{
    public class WorldEventTriggerDisposible : WorldEventTrigger
    {
        [SerializeField] private Collider[] colliders;
        [Space(10)]
        [SerializeField] private UnityEvent restoredEvent;

        private bool _disposed;

        protected override void OnTriggerEnter(Collider other)
        {
            if (_disposed) return;

            base.OnTriggerEnter(other);

            _disposed = true;
            ToggleColliders(false);
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (_disposed) return;

            base.OnTriggerExit(other);

            ToggleColliders(false);
        }

        public void Restore()
        {
            if (_disposed) restoredEvent?.Invoke();

            _disposed = false;
            ToggleColliders(true);
        }

        private void ToggleColliders(bool toggle)
        {
            foreach (var collider in colliders) collider.enabled = toggle;
        }
    }
}
