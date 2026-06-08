using UnityEngine;

namespace PT.Tools.Tutorials.Target
{
    public class TutorialTargetWorld : TutorialTargetBase
    {
        public override Camera GetCamera() => cam != null ? cam : Camera.main;

        public override Vector2 GetScreenPosition()
        {
            return GetCamera() != null 
                ? GetCamera().WorldToScreenPoint(transform.position) 
                : Vector2.zero;
        }

        public override bool IsHit(Vector2 screenPos)
        {
            var ray = GetCamera().ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out var hit, 100f) &&
                   (hit.collider.gameObject == gameObject ||
                    hit.collider.transform.IsChildOf(transform));
        }
    }
}