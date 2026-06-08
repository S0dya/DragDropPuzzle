using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PT.Tools.Other
{
    public class EventSystemDuplicateDestroyer : MonoBehaviour
    {
        [SerializeField] private float disableDelay = 1;

        [SerializeField] private EventSystem sceneEventSystem;

        private void Start()
        {
            DisableDelayAsync().Forget();
        }
        private async UniTask DisableDelayAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(disableDelay));

            Destroy(gameObject);
        }

        private void Update()
        {
            var eventSystems = FindObjectsOfType<EventSystem>();

            if (eventSystems.Length > 0)
            {
                for (int i = 0; i < eventSystems.Length; i++)
                {
                    if (eventSystems[i] != sceneEventSystem)
                    {
                        Destroy(eventSystems[i].gameObject);
                    }
                }
            }
        }
    }
}