using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gameplay.BallDrop.Balls;
using Gameplay.BallDrop.Configs;
using Gameplay.BallDrop.Datas;
using UnityEditor;
using UnityEngine;

namespace Gameplay.BallDrop.Editor
{
    public class SongsConfigEditor : EditorWindow
    {
        private const string ResourcesMovedFolder = "resources_moved";
        private SongsConfig _songsConfig;

        [MenuItem("Tools/Songs Config/Populate Singers")]
        public static void ShowWindow()
        {
            GetWindow<SongsConfigEditor>("Songs Config Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Songs Config Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _songsConfig = (SongsConfig)EditorGUILayout.ObjectField("Songs Config", _songsConfig, typeof(SongsConfig), false);

            GUILayout.Space(10);

            if (GUILayout.Button("Populate Singers from Resources Folder"))
            {
                if (_songsConfig == null)
                {
                    EditorUtility.DisplayDialog("Error", "Please assign a Songs Config first.", "OK");
                    return;
                }

                PopulateSingers();
            }
        }

        private void PopulateSingers()
        {
            string resourcesPath = Path.Combine(Application.dataPath, ResourcesMovedFolder);

            if (!Directory.Exists(resourcesPath))
            {
                EditorUtility.DisplayDialog("Error", $"Resources folder not found at: {resourcesPath}", "OK");
                return;
            }

            // Get all audio files in the folder
            string[] audioFiles = Directory.GetFiles(resourcesPath, "*.mp3")
                .Concat(Directory.GetFiles(resourcesPath, "*.wav"))
                .Concat(Directory.GetFiles(resourcesPath, "*.ogg"))
                .ToArray();

            if (audioFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No audio files found in resources folder.", "OK");
                return;
            }

            // Parse filenames to extract song and character info
            Dictionary<string, HashSet<CharacterNameEnum>> songToCharacters = new Dictionary<string, HashSet<CharacterNameEnum>>();

            foreach (string filePath in audioFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Parse format: nameofcharacter_nameofsong
                string[] parts = fileName.Split('_');

                if (parts.Length >= 2)
                {
                    string characterName = parts[0];
                    string songName = string.Join("_", parts.Skip(1));

                    // Try to parse character name to enum
                    if (System.Enum.TryParse<CharacterNameEnum>(characterName, true, out CharacterNameEnum characterEnum))
                    {
                        if (!songToCharacters.ContainsKey(songName))
                        {
                            songToCharacters[songName] = new HashSet<CharacterNameEnum>();
                        }

                        songToCharacters[songName].Add(characterEnum);
                    }
                }
            }

            // Update SongsConfig
            SerializedObject serializedConfig = new SerializedObject(_songsConfig);
            SerializedProperty songDatasProperty = serializedConfig.FindProperty("songDatas");

            for (int i = 0; i < songDatasProperty.arraySize; i++)
            {
                SerializedProperty songDataProperty = songDatasProperty.GetArrayElementAtIndex(i);
                SerializedProperty musicNameProperty = songDataProperty.FindPropertyRelative("musicName");
                SerializedProperty singersProperty = songDataProperty.FindPropertyRelative("singers");

                string songName = musicNameProperty.enumNames[musicNameProperty.enumValueIndex];

                if (songToCharacters.TryGetValue(songName, out HashSet<CharacterNameEnum> characters))
                {
                    singersProperty.ClearArray();
                    singersProperty.arraySize = characters.Count;

                    int charIndex = 0;
                    foreach (CharacterNameEnum character in characters)
                    {
                        singersProperty.GetArrayElementAtIndex(charIndex).enumValueIndex = (int)character;
                        charIndex++;
                    }

                    Debug.Log($"Updated singers for song: {songName} - {string.Join(", ", characters)}");
                }
                else
                {
                    singersProperty.ClearArray();
                    Debug.LogWarning($"No vocal files found for song: {songName}");
                }
            }

            serializedConfig.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", "Singers populated successfully!", "OK");
        }
    }
}
