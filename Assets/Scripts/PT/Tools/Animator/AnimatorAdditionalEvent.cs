using UnityEngine;
using UnityEngine.Events;

namespace PT.Tools.Animator
{
    public class AnimatorAdditionalEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent additionalAnimationEvent;

        public void OnInvokeAdditionalAnimationEvent() => additionalAnimationEvent.Invoke();
    }
}