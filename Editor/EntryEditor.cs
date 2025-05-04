using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Entry))]
    public class EntryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var entry = (Entry)target;
            DrawDefaultInspector();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
                PlayClip(entry.Sound);
            
            if (GUILayout.Button("Stop"))
                StopAllClips();
            
            EditorGUILayout.EndHorizontal();

            if (entry.Icon == null)
                return;

            var iconRect = entry.Icon.textureRect;
            var totalRect = GUILayoutUtility.GetRect(iconRect.width, iconRect.height);
            var totalWidth = totalRect.width;
            var availableWidth = totalWidth - 100f;
            var scale = availableWidth / iconRect.width;
            scale = Mathf.Clamp(scale, 1f, 3f);
            var offset = (totalWidth - iconRect.width * scale) / 2f;
            var rect = new Rect(totalRect.x + offset, totalRect.y, iconRect.width * scale, iconRect.height * scale);

            var texture = entry.Icon.texture;
            var width = texture.width;
            var height = texture.height;

            var uvRect = new Rect(
                iconRect.x / width, iconRect.y / height,
                iconRect.width / width, iconRect.height / height);

            GUI.DrawTextureWithTexCoords(rect, entry.Icon.texture, uvRect);
        }

        private static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            if (clip == null) return;

            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );

            if (method != null) 
                method.Invoke(null, new object[] { clip, startSample, loop });
        }

        private static void StopAllClips()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );

            if (method != null) 
                method.Invoke(null, null);
        }

    }
}