#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PT.Tools.Colliders.Editor
{
    [CustomEditor(typeof(CircleEdgeColliderTool))]
    public class CircleEdgeColliderToolEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Circle Collider"))
            {
                var tool = (CircleEdgeColliderTool)target;

                Undo.RecordObject(tool.GetComponent<EdgeCollider2D>(), "Generate Circle Collider");

                tool.Generate();

                EditorUtility.SetDirty(tool);
                EditorUtility.SetDirty(tool.GetComponent<EdgeCollider2D>());
            }
        }
    }
}
#endif