using UnityEditor;
using UnityEngine;

namespace Gameplay.BallDrop.Levels.Editor
{
    [CustomEditor(typeof(RendererColorSetter))]
    public class RendererColorSetterEditor : UnityEditor.Editor
    {
        private SerializedProperty _sr;

        private void OnEnable()
        {
            _sr = serializedObject.FindProperty("sr");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Find Sprite Renderer"))
            {
                var setter = (RendererColorSetter)target;

                var spriteRenderer = setter.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null) _sr.objectReferenceValue = spriteRenderer;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}