using UnityEngine;
using UnityEngine.Events;

namespace PT.Tools.Tutorials.Target
{
    public abstract class TutorialTargetBase : MonoBehaviour
    {
        [SerializeField] protected TutorialTargetEnum id;
        [Space]
        [SerializeField] protected UnityEvent invokedAction;
        [Space]
        [SerializeField] protected Camera cam;
        
        
        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public TutorialTargetEnum Id => id;

        public abstract Camera GetCamera();
        public abstract Vector2 GetScreenPosition();
        public abstract bool IsHit(Vector2 screenPos);
        
        public virtual void InvokeAction()
        {
            invokedAction?.Invoke();
            
            gameObject.SetActive(false);
        }
    }
}