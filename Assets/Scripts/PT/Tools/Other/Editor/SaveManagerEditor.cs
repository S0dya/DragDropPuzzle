using System.IO;
using PT.Logic.Save;
using UnityEditor;
using UnityEngine;

namespace PT.Tools.Other.Editor
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
           
            if (GUILayout.Button("Delete Saves", GUILayout.Width(200), GUILayout.Height(20)))
            {
                PlayerPrefs.DeleteAll();
                File.Delete(Path.Combine(Application.persistentDataPath, "GameData.json"));

                string pluginSavePath = Path.Combine(Application.dataPath, "PluginYourGames/Editor/SavesEditorYG2.json");
                if (File.Exists(pluginSavePath))
                {
                    File.Delete(pluginSavePath);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
