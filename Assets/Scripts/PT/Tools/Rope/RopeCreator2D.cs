using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace PT.Tools.Rope
{
    public class RopeCreator2D : MonoBehaviour
    {
        [SerializeField, Range(2, 50)] private int segmentsCount = 2;
        [SerializeField] private float segmentHeight = 0.5f;
        [Space]
        [SerializeField] private Transform ropeParent;
        [Space] 
        [SerializeField] private bool resetOffsetOfFirstInChain;
        [Space]
        [SerializeField] private HingeJoint2D startPointHinge;
        [SerializeField] private HingeJoint2D ropePrefab;
        
        [Button]
        private void CreateRope()
        {
#if UNITY_EDITOR
            // ClearOldRope();
            //
            // var lastBody = startPointHinge.GetComponent<Rigidbody2D>();
            //
            // for (int i = 0; i < segmentsCount; i++)
            // {
            //     var position = GetSegmentPosition(i);
            //     var hinge = (GameObject) (PrefabUtility.InstantiatePrefab(ropePrefab, ropeParent)).GetComponent<HingeJoint2D>();
            //     hinge.transform.position = position;
            //     var rb = hinge.GetComponent<Rigidbody2D>();
            //
            //     if (resetOffsetOfFirstInChain && i == 0) hinge.connectedAnchor = Vector2.zero;
            //     
            //     hinge.connectedBody = lastBody;
            //     lastBody = rb;
            // }
#endif
        }

        private Vector2 GetSegmentPosition(int index)
        {
            return (Vector2)startPointHinge.transform.position + Vector2.down * segmentHeight * (index + 1);
        }
        
        private void ClearOldRope()
        {
            while (ropeParent.childCount > 0)
                DestroyImmediate(ropeParent.GetChild(0).gameObject);
        }
    }
}