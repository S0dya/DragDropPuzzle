using UnityEditor;
using UnityEngine;

namespace Gameplay.BallDrop.Levels.Editor
{
    [CustomEditor(typeof(LevelChunk))]
    public class LevelChunkEditor : UnityEditor.Editor
    {
        private SerializedProperty sizeChangingTrigger;

        private void OnEnable()
        {
            sizeChangingTrigger = serializedObject.FindProperty("sizeChangingTrigger");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Collect Size Changing Triggers"))
            {
                CollectTriggers();
            }
        }

        private void CollectTriggers()
        {
            LevelChunk levelChunk = (LevelChunk)target;

            var trigger = levelChunk.GetComponentInChildren<LevelSizeChangingTrigger>(true);

            serializedObject.Update();

            if (sizeChangingTrigger == null)
            {
                Debug.LogError("sizeChangingTriggers field not found.");
                return;
            }

            sizeChangingTrigger.objectReferenceValue = trigger;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(levelChunk);

            Debug.Log($"Collected LevelSizeChangingTrigger components.");
        }
    }
}