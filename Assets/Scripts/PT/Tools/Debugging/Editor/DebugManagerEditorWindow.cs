#if UNITY_EDITOR
using System;
using UnityEditor;

namespace PT.Tools.Debugging.Editor
{
    public class DebugManagerEditorWindow : EditorWindow
    {
        [MenuItem("Debug/Debug Manager")]
        public static void ShowWindow()
        {
            GetWindow<DebugManagerEditorWindow>("Debug Manager");
        }

        private void OnGUI()
        {
            foreach (DebugCategory category in Enum.GetValues(typeof(DebugCategory)))
            {
                if (category == DebugCategory.None) continue;

                bool isEnabled = DebugManager.IsDebugEnabled(category);
                bool newEnabled = EditorGUILayout.Toggle(category.ToString(), isEnabled);
                if (newEnabled != isEnabled)
                {
                    if (newEnabled)
                        DebugManager.EnableDebug(category);
                    else
                        DebugManager.DisableDebug(category);
                }

                DebugManager.SaveDebugCategories();
            }
        }
    }
}
#endif
