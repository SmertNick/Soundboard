using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ColorKeyTool : EditorWindow
    {
        private const float ButtonsHeight = 30f;
        private readonly List<Texture2D> _selectedTextures = new();
        private readonly string[] _formats = { "PNG", "TGA" };
        
        private string _outputFormat = "PNG";
        private Color _alphaKey = Color.cyan;
        private float _alphaTolerance = 0.1f;
        private Color _shadowKey = Color.magenta;
        private float _shadowTolerance = 0.1f;
        private bool _generateMipmaps;
        private bool _makeReadable;
        
        private bool _showHelp;
        private Vector2 _scrollPosition;
    
        [MenuItem("Tools/Color Key Tool")]
        public static void ShowWindow() => GetWindow<ColorKeyTool>("Color Key Tool");

        private void OnGUI()
        {
            DrawSettings();
            DrawButtons();
            DrawHint();
            DrawList();
        }
    
        private void ProcessTextures()
        {
            var processed = 0;
            var skipped = 0;
            var converted = 0;

            for (var index = 0; index < _selectedTextures.Count; index++)
            {
                var texture = _selectedTextures[index];
                var path = AssetDatabase.GetAssetPath(texture);
                var extension = Path.GetExtension(path).ToLower();
                var isBmp = extension == ".bmp";
                var outputPath = path;

                if (isBmp)
                {
                    outputPath = Path.ChangeExtension(path, "." + _outputFormat.ToLower());
                    converted++;
                }

                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                {
                    Debug.LogError($"Could not get TextureImporter for {texture.name}");
                    skipped++;
                    continue;
                }

                var wasReadable = importer.isReadable;
                importer.isReadable = true;

                importer.SaveAndReimport();

                var sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (sourceTexture == null)
                {
                    Debug.LogError($"Could not load texture at path: {path}");
                    skipped++;
                    continue;
                }

                var modifiedTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32,
                    _generateMipmaps);

                var pixels = sourceTexture.GetPixels();

                for (var i = 0; i < pixels.Length; i++)
                {
                    var dist = ColorDistance(pixels[i], _alphaKey);
                    var alpha = pixels[i].a;
                    // Make pixel transparent. Ensure others are opaque. BMP files may not have proper alpha values
                    pixels[i].a = dist < _alphaTolerance || alpha == 0f ? 0f : 1.0f;
                }

                // Apply changes to the modified texture
                modifiedTexture.SetPixels(pixels);
                modifiedTexture.Apply();

                // Encode based on selected output format
                var encodedData = _outputFormat switch
                {
                    "PNG" => modifiedTexture.EncodeToPNG(),
                    "TGA" => modifiedTexture.EncodeToTGA(),
                    _ => modifiedTexture.EncodeToPNG()
                };

                // Write the modified texture to the output file
                File.WriteAllBytes(outputPath, encodedData);

                // If we converted from BMP to another format, delete the original asset reference
                var fileSwapped = isBmp && outputPath != path;
                if (fileSwapped)
                    AssetDatabase.DeleteAsset(path);

                // Refresh the asset database
                AssetDatabase.Refresh();

                // Get the importer for the new file
                var newImporter = AssetImporter.GetAtPath(outputPath) as TextureImporter;
                if (newImporter != null)
                {
                    // Update import settings
                    newImporter.textureType = TextureImporterType.Sprite;
                    newImporter.alphaIsTransparency = true;
                    newImporter.isReadable = _makeReadable || wasReadable;

                    // Reimport with the new settings
                    newImporter.SaveAndReimport();
                }
                
                if (fileSwapped)
                    _selectedTextures[index] = AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);

                // Clean up
                DestroyImmediate(modifiedTexture);
                processed++;
            }
            //
            // EditorUtility.DisplayDialog("Process Complete", 
            //     $"Processed {processed} textures.\n" +
            //     $"Converted {converted} BMP files to {_outputFormat}.\n" +
            //     $"Skipped {skipped} textures.", 
            //     "OK");
        }
    
        private float ColorDistance(Color a, Color b)
        {
            var fa = new float4(a.r, a.g, a.b, a.a);
            var fb = new float4(b.r, b.g, b.b, b.a);
            var diff = fa - fb;
            var dist = math.length(diff.xyz);
            return dist;
        }

        private void DrawSettings()
        {
            _alphaKey = EditorGUILayout.ColorField("Alpha Key", _alphaKey);
            _alphaTolerance = EditorGUILayout.Slider("Tolerance", _alphaTolerance, 0.0f, 1.0f);
            
            _shadowKey = EditorGUILayout.ColorField("Shadow Key", _shadowKey);
            _shadowTolerance = EditorGUILayout.Slider("Shadow Tolerance", _shadowTolerance, 0.0f, 1.0f);
            
            _generateMipmaps = EditorGUILayout.Toggle("Generate Mipmaps", _generateMipmaps);
            _makeReadable = EditorGUILayout.Toggle("Make Texture Readable", _makeReadable);
            
            var formatIndex = System.Array.IndexOf(_formats, _outputFormat);
            formatIndex = EditorGUILayout.Popup("Output Format", formatIndex, _formats);
            _outputFormat = _formats[formatIndex];
            
            EditorGUILayout.Space();
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Selected", GUILayout.Height(ButtonsHeight)))
                    AddSelectedTextures();
                
                GUI.enabled = _selectedTextures.Count > 0;
            
                if (GUILayout.Button("Process Textures", GUILayout.Height(ButtonsHeight)))
                    ProcessTextures();
            
                GUI.enabled = true;

                if (GUILayout.Button("Clear", GUILayout.Height(ButtonsHeight)))
                    _selectedTextures.Clear();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHint()
        {
            _showHelp = EditorGUILayout.Foldout(_showHelp, "How to use");
            if (!_showHelp) 
                return;
            
            EditorGUILayout.HelpBox(
                "1. Select your textures/sprites in the Project window\n" +
                "2. Click 'Add Selected Textures'\n" +
                "3. Choose a color to make transparent\n" +
                "4. Adjust tolerance (higher = more colors will match)\n" +
                "5. Click 'Process Textures'\n\n" +
                "Note: For BMP files, this will convert them to PNG with transparency.\n" +
                "Make backups of your original files if needed!", 
                MessageType.Info);
        }

        private void DrawList()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Textures", EditorStyles.boldLabel);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                for (var i = 0; i < _selectedTextures.Count; i++)
                    DrawEntry(ref i);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawEntry(ref int i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                var texture = _selectedTextures[i];
                var path = AssetDatabase.GetAssetPath(texture);
                var isBmp = Path.GetExtension(path).ToLower() == ".bmp";
                EditorGUILayout.ObjectField(texture, typeof(Texture2D), false);

                if (isBmp)
                    EditorGUILayout.LabelField("BMP - Will convert to " + _outputFormat, EditorStyles.miniLabel);

                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    _selectedTextures.RemoveAt(i);
                    i--;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    
        private void AddSelectedTextures()
        {
            var selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
        
            foreach (var obj in selection)
            {
                if (obj is Texture2D texture && !_selectedTextures.Contains(texture))
                    _selectedTextures.Add(texture);
            }
        }
    }
}