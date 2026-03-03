using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Luna.GazeShader
{
    public class MaterialInstanceManager : EditorWindow
    {
        private static MaterialInstanceManager window;

        private Dictionary<Texture, List<Material>> materialGroups = new Dictionary<Texture, List<Material>>();
        private Dictionary<string, List<Material>> nullTextureGroups = new Dictionary<string, List<Material>>();
        private Vector2 scrollPosition;
        private bool autoReplaceEnabled = true;

        [MenuItem("Tools/@Luna/Gaze Shader Material Manager")]
        public static void ShowWindow()
        {
            window = GetWindow<MaterialInstanceManager>("Material Instance Manager");
            window.minSize = new Vector2(450, 400);
            window.RefreshMaterialGroups();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawQuickActions();
            DrawMaterialGroups();
            DrawActions();
        }

        // --- 智能获取当前 Shader 的核心纹理 ---
        private Texture GetPrimaryTexture(Material mat)
        {
            if (mat == null || mat.shader == null) return null;
            if (mat.shader.name.Contains("SpriteSheet"))
                return mat.GetTexture("_MainTex"); // 精灵图版本
            else
                return mat.GetTexture("_Textures"); // 数组版本
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(GazeShaderLocalization.MaterialManager.Title, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            int currentIndex = (int)GazeShaderLocalization.CurrentLanguage;
            int newIndex = EditorGUILayout.Popup(currentIndex, GazeShaderLocalization.MainUI.LanguageOptions, GUILayout.Width(35));
            if (newIndex != currentIndex)
            {
                GazeShaderLocalization.CurrentLanguage = (Language)newIndex;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(GazeShaderLocalization.MaterialManager.Description, MessageType.Info);
            EditorGUILayout.Space(10);
        }

        private void DrawQuickActions()
        {
            EditorGUILayout.LabelField(GazeShaderLocalization.MaterialManager.QuickActions, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(GazeShaderLocalization.MaterialManager.AutoOptimizeAll, GUILayout.Height(30)))
            {
                AutoOptimizeAll();
            }
            if (GUILayout.Button(GazeShaderLocalization.MaterialManager.RefreshScan, GUILayout.Height(30)))
            {
                RefreshMaterialGroups();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawMaterialGroups()
        {
            EditorGUILayout.LabelField(GazeShaderLocalization.MaterialManager.MaterialGroups, EditorStyles.boldLabel);
            if (materialGroups.Count == 0 && nullTextureGroups.Count == 0)
            {
                EditorGUILayout.HelpBox(GazeShaderLocalization.MaterialManager.NoMaterialsWarning, MessageType.Warning);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            foreach (var group in materialGroups)
            {
                DrawMaterialGroup(group.Key, group.Value);
            }
            foreach (var group in nullTextureGroups)
            {
                DrawNullTextureGroup(group.Key, group.Value);
            }
            EditorGUILayout.EndScrollView();
        }

        // 参数改为 Texture
        private void DrawMaterialGroup(Texture targetTexture, List<Material> materials)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();

            string arrayInfo = targetTexture != null ? targetTexture.name : GazeShaderLocalization.MaterialManager.NoTextureArray;
            EditorGUILayout.LabelField(arrayInfo, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            int instanceCount = materials.Count(m => IsMaterialInstance(m));
            string statusText = GazeShaderLocalization.MaterialManager.GetString("STATUS", instanceCount, materials.Count);

            if (instanceCount == 0)
                EditorGUILayout.HelpBox(statusText, MessageType.Warning);
            else if (instanceCount < materials.Count)
                EditorGUILayout.HelpBox(statusText, MessageType.Info);
            else
                EditorGUILayout.HelpBox(statusText, MessageType.None);

            EditorGUI.indentLevel++;
            foreach (var material in materials)
            {
                EditorGUILayout.BeginHorizontal();
                bool isInstance = IsMaterialInstance(material);
                string status = isInstance ? "[SHARED]" : "[INDEPENDENT]";
                EditorGUILayout.LabelField($"{status} {material.name}", GUILayout.Width(250));

                if (GUILayout.Button(GazeShaderLocalization.MaterialManager.Select, GUILayout.Width(50)))
                {
                    Selection.activeObject = material;
                    EditorGUIUtility.PingObject(material);
                }

                if (GUILayout.Button(GazeShaderLocalization.MaterialManager.Apply, GUILayout.Width(50)) && isInstance)
                {
                    ApplyMaterialToScene(material);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;

            if (materials.Count > 1)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(GazeShaderLocalization.MaterialManager.CreateSharedInstance))
                {
                    CreateSharedInstanceForGroup(materials);
                }
                if (GUILayout.Button(GazeShaderLocalization.MaterialManager.OptimizeScene))
                {
                    OptimizeSceneForGroup(materials);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawNullTextureGroup(string groupName, List<Material> materials)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(groupName, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            foreach (var material in materials)
            {
                EditorGUILayout.BeginHorizontal();
                bool isInstance = IsMaterialInstance(material);
                string status = isInstance ? "[SHARED]" : "[INDEPENDENT]";
                EditorGUILayout.LabelField($"{status} {material.name}", GUILayout.Width(250));

                if (GUILayout.Button(GazeShaderLocalization.MaterialManager.Select, GUILayout.Width(50)))
                {
                    Selection.activeObject = material;
                    EditorGUIUtility.PingObject(material);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawActions()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(GazeShaderLocalization.MaterialManager.AdvancedActions, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(GazeShaderLocalization.MaterialManager.CreateAllShared, GUILayout.Height(30)))
            {
                CreateAllSharedInstances();
            }
            if (GUILayout.Button(GazeShaderLocalization.MaterialManager.CleanupUnused, GUILayout.Height(30)))
            {
                CleanupUnusedMaterials();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField(GazeShaderLocalization.MaterialManager.HowToUse, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(GazeShaderLocalization.MaterialManager.Instructions, MessageType.Info);
            EditorGUILayout.Space(10);
            autoReplaceEnabled = EditorGUILayout.Toggle(GazeShaderLocalization.MaterialManager.EnableAutoReplace, autoReplaceEnabled);
        }

        private void AutoOptimizeAll()
        {
            RefreshMaterialGroups();
            int totalOptimized = 0;
            int instancesCreated = 0;

            foreach (var group in materialGroups.Values)
            {
                if (group.Count > 1)
                {
                    var instanceCount = group.Count(m => IsMaterialInstance(m));
                    if (instanceCount == 0)
                    {
                        CreateSharedInstanceForGroup(group, true);
                        instancesCreated++;
                    }
                }
            }

            foreach (var group in materialGroups.Values)
            {
                if (group.Count > 1)
                {
                    var sharedInstances = group.Where(m => IsMaterialInstance(m)).ToList();
                    if (sharedInstances.Count > 0)
                    {
                        Material primaryInstance = sharedInstances[0];
                        Texture targetTexture = GetPrimaryTexture(primaryInstance);

                        var allRenderers = FindObjectsOfType<Renderer>();
                        foreach (var renderer in allRenderers)
                        {
                            if (renderer.sharedMaterial != null && renderer.sharedMaterial != primaryInstance)
                            {
                                Texture rendererTexture = GetPrimaryTexture(renderer.sharedMaterial);
                                if (rendererTexture == targetTexture)
                                {
                                    renderer.sharedMaterial = primaryInstance;
                                    totalOptimized++;
                                }
                            }
                        }
                    }
                }
            }

            RefreshMaterialGroups();
            string message = $"Auto optimization complete!\nCreated {instancesCreated} shared instances\nOptimized {totalOptimized} scene objects";
            EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Success, message, "OK");
        }

        private void RefreshMaterialGroups()
        {
            materialGroups.Clear();
            nullTextureGroups.Clear();

            string[] materialGuids = AssetDatabase.FindAssets("t:Material");
            foreach (string guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (material != null && material.shader != null && material.shader.name.Contains("Gaze"))
                {
                    Texture textureArray = GetPrimaryTexture(material);

                    if (textureArray == null)
                    {
                        string groupName = "Materials without Main Texture";
                        if (!nullTextureGroups.ContainsKey(groupName))
                        {
                            nullTextureGroups[groupName] = new List<Material>();
                        }
                        nullTextureGroups[groupName].Add(material);
                    }
                    else
                    {
                        if (!materialGroups.ContainsKey(textureArray))
                        {
                            materialGroups[textureArray] = new List<Material>();
                        }
                        materialGroups[textureArray].Add(material);
                    }
                }
            }

            var sceneMaterials = Resources.FindObjectsOfTypeAll<Material>()
                .Where(m => m.shader != null && m.shader.name.Contains("Gaze"))
                .Where(m => !AssetDatabase.Contains(m));

            foreach (var material in sceneMaterials)
            {
                Texture textureArray = GetPrimaryTexture(material);

                if (textureArray == null)
                {
                    string groupName = "Scene Materials without Main Texture";
                    if (!nullTextureGroups.ContainsKey(groupName))
                    {
                        nullTextureGroups[groupName] = new List<Material>();
                    }
                    if (!nullTextureGroups[groupName].Contains(material))
                    {
                        nullTextureGroups[groupName].Add(material);
                    }
                }
                else
                {
                    if (!materialGroups.ContainsKey(textureArray))
                    {
                        materialGroups[textureArray] = new List<Material>();
                    }
                    if (!materialGroups[textureArray].Contains(material))
                    {
                        materialGroups[textureArray].Add(material);
                    }
                }
            }
        }

        private bool IsMaterialInstance(Material material)
        {
            return AssetDatabase.Contains(material);
        }

        private void CreateSharedInstanceForGroup(List<Material> materials, bool silent = false)
        {
            if (materials.Count == 0) return;

            Material template = materials[0];
            string defaultName = $"{template.name}_Shared";
            string defaultPath = AssetDatabase.GetAssetPath(template);
            string targetDir = System.IO.Path.GetDirectoryName(defaultPath);

            string path;
            if (silent)
            {
                path = System.IO.Path.Combine(targetDir, defaultName + ".mat");
                int counter = 1;
                while (System.IO.File.Exists(path))
                {
                    path = System.IO.Path.Combine(targetDir, $"{defaultName}_{counter}.mat");
                    counter++;
                }
            }
            else
            {
                path = EditorUtility.SaveFilePanelInProject(
                    "Save Shared Material Instance",
                    defaultName,
                    "mat",
                    "Choose where to save the shared material");

                if (string.IsNullOrEmpty(path)) return;
            }

            Material sharedMaterial = new Material(template);
            AssetDatabase.CreateAsset(sharedMaterial, path);
            AssetDatabase.SaveAssets();

            foreach (var material in materials)
            {
                ReplaceMaterialReferences(material, sharedMaterial);
            }

            if (!silent)
            {
                RefreshMaterialGroups();
                string message = $"Created shared material instance and applied to {materials.Count} materials";
                EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Success, message, "OK");
            }
        }

        private void ApplyMaterialToScene(Material material)
        {
            int replaceCount = 0;
            var allRenderers = FindObjectsOfType<Renderer>();
            Texture targetTexture = GetPrimaryTexture(material);

            foreach (var renderer in allRenderers)
            {
                if (renderer.sharedMaterial != null)
                {
                    Texture rendererTexture = GetPrimaryTexture(renderer.sharedMaterial);
                    if (rendererTexture == targetTexture)
                    {
                        renderer.sharedMaterial = material;
                        replaceCount++;
                    }
                }
            }

            string message = $"Applied material to {replaceCount} objects";
            EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Complete, message, "OK");
        }

        private void OptimizeSceneForGroup(List<Material> materials)
        {
            if (materials.Count == 0) return;

            int optimizedCount = 0;
            var sharedInstances = materials.Where(m => IsMaterialInstance(m)).ToList();

            if (sharedInstances.Count == 0)
            {
                EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Warning,
                    "No shared material instances found, please create a shared instance first",
                    "OK");
                return;
            }

            Material primaryInstance = sharedInstances[0];
            Texture targetTexture = GetPrimaryTexture(primaryInstance);

            var allRenderers = FindObjectsOfType<Renderer>();
            foreach (var renderer in allRenderers)
            {
                if (renderer.sharedMaterial != null)
                {
                    Texture rendererTexture = GetPrimaryTexture(renderer.sharedMaterial);
                    if (rendererTexture == targetTexture && renderer.sharedMaterial != primaryInstance)
                    {
                        renderer.sharedMaterial = primaryInstance;
                        optimizedCount++;
                    }
                }
            }

            string message = $"Optimized {optimizedCount} objects, now using shared material instance";
            EditorUtility.DisplayDialog("Optimization Complete", message, "OK");
        }

        private void CreateAllSharedInstances()
        {
            int createdCount = 0;
            foreach (var group in materialGroups.Values)
            {
                if (group.Count > 1)
                {
                    var instanceCount = group.Count(m => IsMaterialInstance(m));
                    if (instanceCount == 0)
                    {
                        CreateSharedInstanceForGroup(group, true);
                        createdCount++;
                    }
                }
            }
            RefreshMaterialGroups();
            string message = $"Created {createdCount} shared material instances";
            EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Complete, message, "OK");
        }

        private void CleanupUnusedMaterials()
        {
            RefreshMaterialGroups();
            int removedCount = 0;
            string message = $"Found {removedCount} unused materials (cleanup logic needs implementation)";
            EditorUtility.DisplayDialog("Cleanup Complete", message, "OK");
        }

        private void ReplaceMaterialReferences(Material oldMaterial, Material newMaterial)
        {
            var allRenderers = FindObjectsOfType<Renderer>();
            foreach (var renderer in allRenderers)
            {
                if (renderer.sharedMaterial == oldMaterial)
                {
                    renderer.sharedMaterial = newMaterial;
                }
            }
        }
    }
}