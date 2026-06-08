using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PT.Tools.Tutorials.Target
{
    public class TutorialTargetUI : TutorialTargetBase
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Button button;
        
        public override Camera GetCamera()
        {
            if (cam != null) return cam;
            
            var canvas = rect != null ? rect.GetComponentInParent<Canvas>() : null;
            return (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;
        }

        public override Vector2 GetScreenPosition()
        {
            return rect.position;
        }

        public override bool IsHit(Vector2 screenPos)
        {
            if (!rect) return false;
    
            var eventData = new PointerEventData(EventSystem.current) { position = screenPos };
            var results = new List<RaycastResult>();
            var raycaster = rect.GetComponentInParent<GraphicRaycaster>();
            raycaster.Raycast(eventData, results);
    
            return results.Any(r => r.gameObject == rect.gameObject || r.gameObject.transform.IsChildOf(rect));
        }
        
        public override void InvokeAction()
        {
            base.InvokeAction();
            if (invokedAction != null) return; 
            
            if (!button) button = rect?.GetComponent<Button>();
            
            button?.onClick.Invoke();
        }
    }
}