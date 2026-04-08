using UnityEngine;
using UnityEditor;
using System.IO;
using Luna.GazeShader;

public class GazeGifShaderGUI : ShaderGUI
{
    private MaterialEditor editor;
    private MaterialProperty[] properties;
    private Material targetMaterial;

    private int selectedTextureSize = 512;
    private int spriteSheetQualityMode = 1; // 0:高性能, 1:高质量 (默认选高质量)

    private bool isSpriteSheetVersion;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        editor = materialEditor;
        properties = props;
        targetMaterial = materialEditor.target as Material;
        
        // 自动识别着色器类型
        isSpriteSheetVersion = targetMaterial.shader.name.Contains("SpriteSheet");

        EditorGUILayout.Space(10);
        DrawHeaderSection();
        EditorGUILayout.Space(15);
        DrawResourceSection();
        EditorGUILayout.Space(15);
        DrawEffectSection();
        EditorGUILayout.Space(15);        
        DrawFixedVariationSection();
        EditorGUILayout.Space(15);
        DrawRandomVariationSection();
        EditorGUILayout.Space(15);
        DrawAdvancedSection();
        EditorGUILayout.Space(15);
        
        DrawLightingEffectSection();  
        
        DrawAuthorSection();

        if (editor.PropertiesGUI() || GUI.changed)
        {
            EditorUtility.SetDirty(targetMaterial);
        }
    }

    private void DrawAuthorSection()
    {
        AuthorInfoDrawer.DrawAuthorInfo();
    }

    private void DrawHeaderSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();
        GUIStyle largeHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
        largeHeaderStyle.fontSize = 14;
        EditorGUILayout.LabelField(GazeShaderLocalization.MainUI.Title, largeHeaderStyle, GUILayout.ExpandWidth(true));

        if (GUILayout.Button(GazeShaderLocalization.MainUI.MaterialManager, GUILayout.Width(150)))
        {
            MaterialInstanceManager.ShowWindow();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(GazeShaderLocalization.MainUI.LanguageLabel, GUILayout.Width(60));
        GUILayout.FlexibleSpace();

        int currentIndex = (int)GazeShaderLocalization.CurrentLanguage;
        int newIndex = EditorGUILayout.Popup(currentIndex, GazeShaderLocalization.MainUI.LanguageOptions, GUILayout.Width(35));
        if (newIndex != currentIndex)
        {
            GazeShaderLocalization.CurrentLanguage = (Luna.GazeShader.Language)newIndex;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(GazeShaderLocalization.MainUI.Description, MessageType.Info);
        EditorGUILayout.EndVertical();
    }
    private void DrawResourceSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(GazeShaderLocalization.Resources.GifConversion, EditorStyles.boldLabel);
        DrawGifConversionModule();
        
        EditorGUILayout.Space(10);
        
        if (isSpriteSheetVersion)
        {
            DrawSpriteSheetModule();
        }
        else
        {
            EditorGUILayout.LabelField(GazeShaderLocalization.Resources.TextureArray, EditorStyles.boldLabel);
            DrawTextureArrayModule();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSpriteSheetModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        
        // 添加多语言小标题
        EditorGUILayout.LabelField(GazeShaderLocalization.GetString("SPRITE_SHEET_TITLE"), EditorStyles.boldLabel);

        MaterialProperty mainTexProp = FindProperty("_MainTex", properties);
        editor.TexturePropertySingleLine(new GUIContent(GazeShaderLocalization.GetString("SPRITE_SHEET_TEXTURE")), mainTexProp);

        if (mainTexProp.textureValue != null)
        {
            // 行列和帧数参数
            MaterialProperty colsProp = FindProperty("_Columns", properties);
            MaterialProperty rowsProp = FindProperty("_Rows", properties);
            MaterialProperty framesProp = FindProperty("_TotalFrames", properties);

            EditorGUILayout.Space(5);
            EditorGUI.indentLevel++;
            
            // 监听用户是否修改了行或列
            EditorGUI.BeginChangeCheck();
            editor.ShaderProperty(colsProp, new GUIContent(GazeShaderLocalization.GetString("COLUMNS")));
            editor.ShaderProperty(rowsProp, new GUIContent(GazeShaderLocalization.GetString("ROWS")));
            if (EditorGUI.EndChangeCheck())
            {
                // 人性化功能：如果用户修改了行列数，自动帮他计算出总帧数（行 x 列）
                framesProp.floatValue = colsProp.floatValue * rowsProp.floatValue;
            }
            
            // 依然保留帧数的修改权限，以防用户的精灵图最后一行没有排满
            editor.ShaderProperty(framesProp, new GUIContent(GazeShaderLocalization.GetString("TOTAL_FRAMES")));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(5);

            Texture2D tex = mainTexProp.textureValue as Texture2D;
            int width = tex != null ? tex.width : 0;
            int height = tex != null ? tex.height : 0;

            string infoText = GazeShaderLocalization.GetString("SPRITE_SHEET_READY", 
                width, height, framesProp.floatValue, colsProp.floatValue, rowsProp.floatValue);
            
            EditorGUILayout.HelpBox(infoText, MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(GazeShaderLocalization.GetString("NO_SPRITE_SHEET_WARNING"), MessageType.Warning);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawEffectSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(GazeShaderLocalization.Effects.AnimationSettings, EditorStyles.boldLabel);
        DrawAnimationModule();
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(GazeShaderLocalization.Effects.GazeEffect, EditorStyles.boldLabel);
        DrawGazeEffectModule();
        EditorGUILayout.EndVertical();
    }

    
    private void GenerateNormalMapForCurrentMaterial()
    {
        if (isSpriteSheetVersion)
        {
            MaterialProperty mainTexProp = FindProperty("_MainTex", properties);
            if (mainTexProp.textureValue == null)
            {
                EditorUtility.DisplayDialog("Error", "No Sprite Sheet assigned!", "OK");
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("Generating Normal Map", "Processing Sprite Sheet...", 0.5f);
                Texture2D sourceTex = mainTexProp.textureValue as Texture2D;

                // 安全读取像素，无视压缩格式
                RenderTexture rt = RenderTexture.GetTemporary(sourceTex.width, sourceTex.height, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(sourceTex, rt);
                Texture2D readableTex = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.RGBA32, false);
                RenderTexture.active = rt;
                readableTex.ReadPixels(new Rect(0, 0, sourceTex.width, sourceTex.height), 0, 0);
                readableTex.Apply();
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rt);

                Texture2D normalMap = NormalMapGenerator.GenerateNormalMapFromTexture(readableTex, 1.0f);
                Object.DestroyImmediate(readableTex);

                string sourcePath = AssetDatabase.GetAssetPath(sourceTex);
                string directory = System.IO.Path.GetDirectoryName(sourcePath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(sourcePath) + "_NormalMap.png";
                string fullPath = System.IO.Path.Combine(directory, fileName);

                File.WriteAllBytes(fullPath, normalMap.EncodeToPNG());
                AssetDatabase.Refresh();

                // 自动将生成的图片设置为 NormalMap 格式
                TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                    importer.SaveAndReimport();
                }

                MaterialProperty normalMapProp = FindProperty("_NormalMap", properties);
                normalMapProp.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);

                FindProperty("_UseNormalMap", properties).floatValue = 1.0f;
                FindProperty("_NormalStrength", properties).floatValue = 1.0f;

                EditorUtility.DisplayDialog("Success", "Sprite Sheet Normal map generated successfully!", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        else
        {
            MaterialProperty texturesProp = FindProperty("_Textures", properties);
            if (texturesProp.textureValue == null) return;

            Texture2DArray sourceArray = texturesProp.textureValue as Texture2DArray;
            try
            {
                EditorUtility.DisplayProgressBar("Generating Normal Map", "Processing frames...", 0.3f);
                Texture2DArray normalMapArray = NormalMapGenerator.GenerateNormalMapFromTextureArray(sourceArray, 1.0f);

                if (normalMapArray != null)
                {
                    string sourcePath = AssetDatabase.GetAssetPath(sourceArray);
                    string directory = System.IO.Path.GetDirectoryName(sourcePath);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(sourcePath) + "_NormalMap.asset";
                    string fullPath = System.IO.Path.Combine(directory, fileName);

                    AssetDatabase.CreateAsset(normalMapArray, fullPath);
                    AssetDatabase.SaveAssets();

                    FindProperty("_NormalMapArray", properties).textureValue = normalMapArray;
                    FindProperty("_UseNormalMap", properties).floatValue = 1.0f;
                    FindProperty("_NormalStrength", properties).floatValue = 1.0f;
                    EditorUtility.DisplayDialog("Success", "Normal map array generated successfully!", "OK");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }

    private void DrawFixedVariationSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(GazeShaderLocalization.Variations.Fixed, EditorStyles.boldLabel);

        DrawQuickOrientationMenu();

        // 添加提示语
        EditorGUILayout.HelpBox(
            GazeShaderLocalization.Variations.QuickOrientationTooltip,
            MessageType.Info);

        DrawFixedVariationModule();

        EditorGUILayout.EndVertical();
    }

    private void DrawRandomVariationSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(GazeShaderLocalization.Variations.Random, EditorStyles.boldLabel);
        DrawRandomVariationModule();
        EditorGUILayout.EndVertical();
    }

    private void DrawAdvancedSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box); 
        
        // 模块 1: UV 调整
        EditorGUILayout.LabelField(GazeShaderLocalization.Advanced.UVAdjust, EditorStyles.boldLabel);
        DrawUVAdjustModule();
        
        EditorGUILayout.Space(10);
        
        // 模块 2: 显示修复
        EditorGUILayout.LabelField(GazeShaderLocalization.Advanced.DisplayFix, EditorStyles.boldLabel);
        DrawDisplayFixModule(); 
        
        EditorGUILayout.EndVertical();
    }

    private void DrawDisplayFixModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box); 

        // 1. 背面剔除 
        MaterialProperty backfaceProp = FindProperty("_BackfaceCulling", properties);
        if (backfaceProp != null)
        {
            editor.ShaderProperty(backfaceProp, new GUIContent(
                GazeShaderLocalization.Advanced.BackfaceCulling, 
                GazeShaderLocalization.Advanced.BackfaceCullingTooltip));
        }

        // 2. 修复伪影
        MaterialProperty fixTranspProp = FindProperty("_FixTransp", properties);
        if (fixTranspProp != null)
        {
            editor.ShaderProperty(fixTranspProp, new GUIContent(
                GazeShaderLocalization.Advanced.FixArtifacts, 
                GazeShaderLocalization.Advanced.FixArtifactsTooltip));
        }

        // 3. 渲染深度 
        MaterialProperty queueProp = FindProperty("_CustomRenderQueue", properties);
        if (queueProp != null)
        {
            EditorGUI.BeginChangeCheck();
            
            editor.ShaderProperty(queueProp, new GUIContent(
                GazeShaderLocalization.Advanced.RenderQueue, 
                GazeShaderLocalization.Advanced.RenderQueueTooltip));
            
            if (EditorGUI.EndChangeCheck())
            {
                targetMaterial.renderQueue = (int)queueProp.floatValue;
            }
        }

        EditorGUILayout.EndVertical(); 
    }

    private void DrawLightingEffectSection()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        MaterialProperty lightingEffectProp = FindProperty("_LightingEffect", properties);

        EditorGUILayout.BeginHorizontal();
        
        // 使用 EditorStyles.boldLabel 让标题加粗
        EditorGUILayout.LabelField(
            new GUIContent(
                GazeShaderLocalization.GetString("LIGHTING_EFFECT_SECTION"), 
                GazeShaderLocalization.Advanced.LightingEffectTooltip
            ),
            EditorStyles.boldLabel, 
            GUILayout.Width(EditorGUIUtility.labelWidth - 5)
        );

        EditorGUI.BeginChangeCheck();
        bool newLightingEffect = EditorGUILayout.Toggle(lightingEffectProp.floatValue > 0.5f);
        EditorGUILayout.EndHorizontal();
        // -----------------------------------------------------------
        if (EditorGUI.EndChangeCheck())
        {
            lightingEffectProp.floatValue = newLightingEffect ? 1.0f : 0.0f;

            if (!newLightingEffect)
            {
                MaterialProperty useNormalMapProp = FindProperty("_UseNormalMap", properties);
                if (useNormalMapProp != null) useNormalMapProp.floatValue = 0.0f;
                targetMaterial.DisableKeyword("_NORMAL_MAP");
                
                MaterialProperty useLightVolumeProp = FindProperty("_UseLightVolume", properties);
                if (useLightVolumeProp != null) useLightVolumeProp.floatValue = 0.0f;
                targetMaterial.DisableKeyword("_USE_LIGHT_VOLUME");
            }
            SetupMaterialKeywords(targetMaterial);
        }

        // 如果开启了光影效果，显示下面的所有功能
        if (lightingEffectProp.floatValue > 0.5f)
        {
            EditorGUILayout.Space(5);

            // --- 颜色(Color)
            MaterialProperty colorProp = FindProperty("_Color", properties);
            editor.ShaderProperty(colorProp, new GUIContent(GazeShaderLocalization.Effects.Color));

            // --- 亮度 (Brightness) ---
            MaterialProperty brightnessProp = FindProperty("_Brightness", properties);
            editor.ShaderProperty(brightnessProp, new GUIContent(
                GazeShaderLocalization.GetString("BRIGHTNESS"), 
                GazeShaderLocalization.GetString("BRIGHTNESS_TOOLTIP")));
            
            EditorGUILayout.Space(5);

            // --- 使用法线贴图 (Normal Map) ---
            MaterialProperty useNormalMapProp = FindProperty("_UseNormalMap", properties);
            EditorGUI.BeginChangeCheck();
            bool useNormal = EditorGUILayout.Toggle(
                new GUIContent(
                    GazeShaderLocalization.GetString("USE_NORMAL_MAP"),
                    GazeShaderLocalization.GetString("USE_NORMAL_MAP_TOOLTIP")
                ),
                useNormalMapProp.floatValue > 0.5f
            );
            if (EditorGUI.EndChangeCheck())
            {
                useNormalMapProp.floatValue = useNormal ? 1.0f : 0.0f;
                SetupMaterialKeywords(targetMaterial);
            }

            if (useNormal)
            {
                EditorGUI.indentLevel++;
                DrawNormalMappingSubControls(); 
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // --- 使用 VRC Light Volumes ---
            MaterialProperty useLightVolumeProp = FindProperty("_UseLightVolume", properties);
            EditorGUI.BeginChangeCheck();
            bool useVolume = EditorGUILayout.Toggle(
                new GUIContent(
                    GazeShaderLocalization.GetString("USE_LIGHT_VOLUME"),
                    GazeShaderLocalization.GetString("USE_LIGHT_VOLUME_TOOLTIP")
                ),
                useLightVolumeProp.floatValue > 0.5f
            );
            if (EditorGUI.EndChangeCheck())
            {
                useLightVolumeProp.floatValue = useVolume ? 1.0f : 0.0f;
                SetupMaterialKeywords(targetMaterial);
            }

            if (useVolume)
            {
                EditorGUI.indentLevel++;
                MaterialProperty lightVolumeIntensityProp = FindProperty("_LightVolumeIntensity", properties);
                editor.ShaderProperty(lightVolumeIntensityProp, new GUIContent(
                    GazeShaderLocalization.GetString("LIGHT_VOLUME_INTENSITY"),
                    GazeShaderLocalization.GetString("LIGHT_VOLUME_INTENSITY_TOOLTIP")
                ));
                EditorGUI.indentLevel--;
                
                EditorGUILayout.HelpBox(
                    GazeShaderLocalization.GetString("LIGHT_VOLUME_ENABLED_INFO"),
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    GazeShaderLocalization.GetString("LIGHT_VOLUME_DISABLED_WARNING"),
                    MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.HelpBox(
                GazeShaderLocalization.GetString("LIGHTING_EFFECT_DISABLED_INFO"),
                MessageType.Info
            );
        }

        EditorGUILayout.EndVertical();
    }

    // 法线贴图子区域函数
    private void DrawNormalMappingSubControls()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        
        // 生成法线贴图按钮
        if (GUILayout.Button(new GUIContent(
            GazeShaderLocalization.GetString("GENERATE_NORMAL_MAP"),
            GazeShaderLocalization.GetString("GENERATE_NORMAL_MAP_TOOLTIP")), GUILayout.Height(25)))
        {
            GenerateNormalMapForCurrentMaterial();
        }
        EditorGUILayout.Space(5);

        // --- 智能区分法线纹理槽 ---
        if (isSpriteSheetVersion)
        {
            MaterialProperty normalMapProp = FindProperty("_NormalMap", properties);
            editor.TexturePropertySingleLine(new GUIContent("Normal Map (SpriteSheet)"), normalMapProp);
        }
        else
        {
            MaterialProperty normalMapArrayProp = FindProperty("_NormalMapArray", properties);
            editor.TexturePropertySingleLine(new GUIContent("Normal Map Array"), normalMapArrayProp);
        }

        MaterialProperty normalStrengthProp = FindProperty("_NormalStrength", properties);
        editor.ShaderProperty(normalStrengthProp, new GUIContent(
            GazeShaderLocalization.GetString("NORMAL_STRENGTH"),
            "1: 正常法线\n-1: 凹凸反转\n0: 无凹凸效果\n其他值: 增强/减弱效果"
        ));

        // 高光参数
        MaterialProperty specularSharpnessProp = FindProperty("_SpecularSharpness", properties);
        editor.ShaderProperty(specularSharpnessProp, new GUIContent(
            GazeShaderLocalization.GetString("SPECULAR_SHARPNESS"),
            GazeShaderLocalization.GetString("SPECULAR_SHARPNESS_TOOLTIP")
        ));

        MaterialProperty specularBrightnessProp = FindProperty("_SpecularBrightness", properties);
        editor.ShaderProperty(specularBrightnessProp, new GUIContent(
            GazeShaderLocalization.GetString("SPECULAR_BRIGHTNESS"),
            GazeShaderLocalization.GetString("SPECULAR_BRIGHTNESS_TOOLTIP")
        ));

        EditorGUILayout.EndVertical();
    }
   

    private void SetupMaterialKeywords(Material material)
    {
        SetupKeyword(material, "_GAZE_ON", material.GetFloat("_Gaze") > 0.5f);
        SetupKeyword(material, "_FIX_TRANSPARENCY", material.GetFloat("_FixTransp") > 0.5f);
        SetupKeyword(material, "_BACKFACE_CULLING", material.GetFloat("_BackfaceCulling") > 0.5f);
        SetupKeyword(material, "_LIGHTING_EFFECT", material.GetFloat("_LightingEffect") > 0.5f);
        SetupKeyword(material, "_NORMAL_MAP", material.GetFloat("_UseNormalMap") > 0.5f && material.GetFloat("_LightingEffect") > 0.5f);
        
        SetupKeyword(material, "_USE_LIGHT_VOLUME", material.GetFloat("_UseLightVolume") > 0.5f);
    }

    private static void SetupKeyword(Material material, string keyword, bool state)
    {
        if (state)
            material.EnableKeyword(keyword);
        else
            material.DisableKeyword(keyword);
    }

    private void DrawGifConversionModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        MaterialProperty gifTextureProp = FindProperty("_GifTexture", properties);
        editor.TexturePropertySingleLine(new GUIContent(GazeShaderLocalization.Resources.GifSourceFile), gifTextureProp);

        if (gifTextureProp.textureValue != null)
        {
            string fileName = gifTextureProp.textureValue.name;
            string infoText = GazeShaderLocalization.Resources.GetString("SELECTED", fileName);
            EditorGUILayout.HelpBox(infoText, MessageType.Info);
        }

        if (isSpriteSheetVersion)
        {
            string[] qualityNames = new string[] { 
                GazeShaderLocalization.GetString("PERFORMANCE_MODE"), 
                GazeShaderLocalization.GetString("QUALITY_MODE") 
            };
            int[] qualityValues = new int[] { 0, 1 };
            spriteSheetQualityMode = EditorGUILayout.IntPopup(
                GazeShaderLocalization.GetString("GENERATION_MODE"), 
                spriteSheetQualityMode, 
                qualityNames, 
                qualityValues);
        }
        else
        {
            selectedTextureSize = EditorGUILayout.IntPopup(GazeShaderLocalization.Resources.TextureSize, selectedTextureSize,
                new string[] { "128×128", "256×256", "512×512", "1024×1024" },
                new int[] { 128, 256, 512, 1024 });
        }

        bool canConvert = gifTextureProp.textureValue != null &&
                         AssetDatabase.GetAssetPath(gifTextureProp.textureValue).ToLower().EndsWith(".gif");

        EditorGUI.BeginDisabledGroup(!canConvert);
        
        string buttonLabel = isSpriteSheetVersion ? GazeShaderLocalization.GetString("GENERATE_SPRITE_SHEET") : GazeShaderLocalization.Resources.ConvertButton;
        
        if (GUILayout.Button(buttonLabel, GUILayout.Height(25)))
        {
            if (isSpriteSheetVersion)
            {
                GifToSpriteSheetConverter.ConvertGifToSpriteSheet(
                    gifTextureProp, 
                    spriteSheetQualityMode == 1, 
                    targetMaterial, 
                    properties
                );
            }
            else
            {
                GifToTextureArrayConverter.ConvertGif(
                    gifTextureProp,
                    selectedTextureSize,
                    GazeShaderLocalization.CurrentLanguage,
                    targetMaterial,
                    properties
                );
            }
        }
        EditorGUI.EndDisabledGroup();

        if (!canConvert && gifTextureProp.textureValue != null)
        {
            EditorGUILayout.HelpBox(GazeShaderLocalization.Resources.NotGifWarning, MessageType.Warning);
        }
        else if (gifTextureProp.textureValue == null)
        {
            EditorGUILayout.HelpBox(GazeShaderLocalization.Resources.SelectGifPrompt, MessageType.Info);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawTextureArrayModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        MaterialProperty texturesProp = FindProperty("_Textures", properties);
        editor.TexturePropertySingleLine(new GUIContent(GazeShaderLocalization.Resources.TextureArray), texturesProp);

        if (texturesProp.textureValue != null)
        {
            Texture2DArray array = texturesProp.textureValue as Texture2DArray;
            if (array != null)
            {
                string infoText = GazeShaderLocalization.Resources.GetString("READY_STATUS", array.width, array.height, array.depth);
                EditorGUILayout.HelpBox(infoText, MessageType.Info);
            }
        }
        else
        {
            EditorGUILayout.HelpBox(GazeShaderLocalization.Resources.NoArrayWarning, MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawAnimationModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        editor.ShaderProperty(FindProperty("_fps", properties), GazeShaderLocalization.Effects.BaseFPS);

        MaterialProperty playModeProp = FindProperty("_PlayMode", properties);
        float playModeValue = playModeProp.floatValue;
        int selectedPlayMode = Mathf.Clamp((int)playModeValue, 0, 3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Effects.PlayMode));
        int newPlayMode = GUILayout.Toolbar(selectedPlayMode, GazeShaderLocalization.Effects.PlayModeOptions);
        if (newPlayMode != selectedPlayMode)
        {
            playModeProp.floatValue = newPlayMode;
        }
        EditorGUILayout.EndHorizontal();

        if (playModeProp.floatValue > 2.5f)
        {
            MaterialProperty manualFrameProp = FindProperty("_ManualFrame", properties);
            float maxFrames = 100;

            // --- 智能读取不同 Shader 的最大帧数 ---
            if (isSpriteSheetVersion)
            {
                // 精灵图版本：直接读取 _TotalFrames 属性
                MaterialProperty totalFramesProp = FindProperty("_TotalFrames", properties);
                if (totalFramesProp != null)
                {
                    maxFrames = totalFramesProp.floatValue;
                }
            }
            else
            {
                // 数组版本：读取 _Textures 数组的深度
                MaterialProperty texturesProp = FindProperty("_Textures", properties);
                if (texturesProp != null && texturesProp.textureValue != null)
                {
                    Texture2DArray array = texturesProp.textureValue as Texture2DArray;
                    if (array != null)
                    {
                        maxFrames = array.depth;
                    }
                }
            }

            // 限制当前帧数不超过最大帧数
            float currentFrame = manualFrameProp.floatValue;
            if (currentFrame > maxFrames)
            {
                manualFrameProp.floatValue = maxFrames;
            }
            else if (currentFrame < 1)
            {
                manualFrameProp.floatValue = 1;
            }

            // 绘制滑块
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Effects.ManualFrame));
            float newFrame = EditorGUILayout.Slider(manualFrameProp.floatValue, 1, maxFrames);
            manualFrameProp.floatValue = Mathf.Round(newFrame);
            EditorGUILayout.LabelField($"/ {maxFrames}", GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
        }

        MaterialProperty randomProp = FindProperty("_StartFrameRandomization", properties);
        editor.ShaderProperty(randomProp, new GUIContent(GazeShaderLocalization.Effects.StartFrameRandomization, GazeShaderLocalization.Effects.StartFrameTooltip));

        // 距离速度控制
        EditorGUILayout.Space(5);

        MaterialProperty speedChangeModeProp = FindProperty("_SpeedChangeMode", properties);
        float speedChangeModeValue = speedChangeModeProp.floatValue;
        int selectedSpeedChangeMode = Mathf.Clamp((int)speedChangeModeValue, 0, 2);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(
            GazeShaderLocalization.Effects.SpeedChangeMode,
            GazeShaderLocalization.GetString("SPEED_CHANGE_MODE_TOOLTIP")
        ));
        int newSpeedChangeMode = GUILayout.Toolbar(selectedSpeedChangeMode, GazeShaderLocalization.Effects.SpeedModeOptions);
        if (newSpeedChangeMode != selectedSpeedChangeMode)
        {
            speedChangeModeProp.floatValue = newSpeedChangeMode;
        }
        EditorGUILayout.EndHorizontal();

        if (speedChangeModeProp.floatValue > 0.5f)
        {
            // 速度从零开始勾选框
            MaterialProperty speedFromZeroProp = FindProperty("_SpeedFromZero", properties);
            if (speedFromZeroProp != null)
            {
                editor.ShaderProperty(speedFromZeroProp, new GUIContent(
                    GazeShaderLocalization.GetString("SPEED_FROM_ZERO"),
                    GazeShaderLocalization.GetString("SPEED_FROM_ZERO_TOOLTIP")
                ));
            }

            MaterialProperty speedChangeRateProp = FindProperty("_SpeedChangeRate", properties);
            editor.ShaderProperty(speedChangeRateProp, new GUIContent(
                GazeShaderLocalization.Effects.SpeedChangeRate,
                GazeShaderLocalization.GetString("SPEED_CHANGE_RATE_TOOLTIP")
            ));

            // 最远距离控制
            MaterialProperty maxDistanceProp = FindProperty("_MaxDistance", properties);
            if (maxDistanceProp != null)
            {
                editor.ShaderProperty(maxDistanceProp, new GUIContent(
                    GazeShaderLocalization.GetString("MAX_DISTANCE"),
                    GazeShaderLocalization.GetString("MAX_DISTANCE_TOOLTIP")
                ));
            }
        }

        EditorGUILayout.EndVertical();
    }
    private void DrawGazeEffectModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        MaterialProperty gazeProp = FindProperty("_Gaze", properties);
        editor.ShaderProperty(gazeProp, new GUIContent(GazeShaderLocalization.Effects.Gaze, GazeShaderLocalization.Effects.GazeTooltip));

        if (gazeProp.floatValue > 0.5f)
        {
            // 添加单轴凝视选项
            MaterialProperty singleAxisGazeProp = FindProperty("_SingleAxisGaze", properties);
            float singleAxisGazeValue = singleAxisGazeProp.floatValue;
            int selectedSingleAxisGaze = Mathf.Clamp((int)singleAxisGazeValue, 0, 3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Effects.SingleAxisGaze, GazeShaderLocalization.Effects.SingleAxisGazeTooltip));
            int newSingleAxisGaze = GUILayout.Toolbar(selectedSingleAxisGaze, GazeShaderLocalization.Effects.SingleAxisGazeOptions);
            if (newSingleAxisGaze != selectedSingleAxisGaze)
            {
                singleAxisGazeProp.floatValue = newSingleAxisGaze;
            }
            EditorGUILayout.EndHorizontal();

            MaterialProperty weakenProp = FindProperty("_WeakenDistanceGaze", properties);
            editor.ShaderProperty(weakenProp, GazeShaderLocalization.Effects.WeakenDistanceGaze);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawFixedVariationModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();
        MaterialProperty rotXProp = FindProperty("_ExtraRotX", properties);
        EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Variations.ExtraXRotation));
        float newRotX = EditorGUILayout.Slider(rotXProp.floatValue, -180, 180);
        rotXProp.floatValue = newRotX;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        MaterialProperty rotYProp = FindProperty("_ExtraRotY", properties);
        EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Variations.ExtraYRotation));
        float newRotY = EditorGUILayout.Slider(rotYProp.floatValue, -180, 180);
        rotYProp.floatValue = newRotY;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        MaterialProperty rotZProp = FindProperty("_ExtraRotZ", properties);
        EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Variations.ExtraZRotation));
        float newRotZ = EditorGUILayout.Slider(rotZProp.floatValue, -180, 180);
        rotZProp.floatValue = newRotZ;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawRandomVariationModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        
        // 带有悬停提示和多语言支持的按钮
        if (GUILayout.Button(new GUIContent(
            GazeShaderLocalization.Variations.AutoDesyncButton, 
            GazeShaderLocalization.Variations.AutoDesyncTooltip), 
            GUILayout.Height(25)))
        {
            AutoDesyncObjects();
        }
        EditorGUILayout.Space(5);

        MaterialProperty scaleVarProp = FindProperty("_ScaleVariation", properties);
        editor.ShaderProperty(scaleVarProp, new GUIContent(GazeShaderLocalization.Variations.ScaleVariation, GazeShaderLocalization.Variations.ScaleVariationTooltip));

        MaterialProperty rotXVarProp = FindProperty("_RandomRotXVariation", properties);
        editor.ShaderProperty(rotXVarProp, new GUIContent(GazeShaderLocalization.Variations.RandomXRotation, GazeShaderLocalization.Variations.RandomXRotationTooltip));

        MaterialProperty rotYVarProp = FindProperty("_RandomRotYVariation", properties);
        editor.ShaderProperty(rotYVarProp, new GUIContent(GazeShaderLocalization.Variations.RandomYRotation, GazeShaderLocalization.Variations.RandomYRotationTooltip));

        MaterialProperty rotZVarProp = FindProperty("_RandomRotZVariation", properties);
        editor.ShaderProperty(rotZVarProp, new GUIContent(GazeShaderLocalization.Variations.RandomZRotation, GazeShaderLocalization.Variations.RandomZRotationTooltip));
        EditorGUILayout.EndVertical();
    }

    private void DrawUVAdjustModule()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        editor.ShaderProperty(FindProperty("_UVx", properties), GazeShaderLocalization.Advanced.UVXOffset);
        editor.ShaderProperty(FindProperty("_UVy", properties), GazeShaderLocalization.Advanced.UVYOffset);
        editor.ShaderProperty(FindProperty("_ScaleX", properties), GazeShaderLocalization.Advanced.XScale);
        editor.ShaderProperty(FindProperty("_ScaleY", properties), GazeShaderLocalization.Advanced.YScale);

        // 添加水平翻转和垂直翻转
        DrawFlipToggles();

        EditorGUILayout.EndVertical();
    }

    private void DrawFlipToggles()
    {
        // 获取当前Scale值
        MaterialProperty scaleXProp = FindProperty("_ScaleX", properties);
        MaterialProperty scaleYProp = FindProperty("_ScaleY", properties);

        // 根据当前Scale值确定Toggle状态
        bool isFlipX = scaleXProp.floatValue < 0;
        bool isFlipY = scaleYProp.floatValue < 0;

        EditorGUI.BeginChangeCheck();
        isFlipX = EditorGUILayout.Toggle(GazeShaderLocalization.Advanced.FlipHorizontal, isFlipX);
        isFlipY = EditorGUILayout.Toggle(GazeShaderLocalization.Advanced.FlipVertical, isFlipY);
        if (EditorGUI.EndChangeCheck())
        {
            // 翻转X：如果原本是正数，翻转后变为负数，绝对值保持不变；如果原本是负数，翻转后变为正数。
            scaleXProp.floatValue = isFlipX ? -Mathf.Abs(scaleXProp.floatValue) : Mathf.Abs(scaleXProp.floatValue);
            scaleYProp.floatValue = isFlipY ? -Mathf.Abs(scaleYProp.floatValue) : Mathf.Abs(scaleYProp.floatValue);
        }
    }

    private void DrawQuickOrientationMenu()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        // 定义旋转组合
        string[] quickOrientations = new string[]
        {
            "★·X0° Y0° Z0°·★",
            "X0° Y0° Z90°",
            "X0° Y0° Z180°",
            "X0° Y90° Z0°",
            "X0° Y90° Z90°",
            "X0° Y90° Z180°",
            "X0° Y180° Z0°",
            "X0° Y180° Z90°",
            "X0° Y180° Z180°",
            "X90° Y0° Z0°",
            "X90° Y0° Z90°",
            "X90° Y0° Z180°",
            "X90° Y90° Z0°",
            "X90° Y90° Z90°",
            "X90° Y90° Z180°",
            "X90° Y180° Z0°",
            "X90° Y180° Z90°",
            "★·X90° Y180° Z180°·★",
            "X180° Y0° Z90°",
            "X180° Y90° Z0°",
            "X180° Y90° Z90°",
            "X180° Y90° Z180°",
            "X180° Y180° Z0°",
            "X180° Y180° Z90°"
        };

        // 对应的旋转值数组
        Vector3[] rotationValues = new Vector3[]
        {
            new Vector3(0, 0, 0),      // ★·X0° Y0° Z0°·★
            new Vector3(0, 0, 90),
            new Vector3(0, 0, 180),
            new Vector3(0, 90, 0),
            new Vector3(0, 90, 90),
            new Vector3(0, 90, 180),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 90),
            new Vector3(0, 180, 180),
            new Vector3(90, 0, 0),
            new Vector3(90, 0, 90),
            new Vector3(90, 0, 180),
            new Vector3(90, 90, 0),
            new Vector3(90, 90, 90),
            new Vector3(90, 90, 180),
            new Vector3(90, 180, 0),
            new Vector3(90, 180, 90),
            new Vector3(90, 180, 180),  // ★·X90° Y180° Z180°·★
            new Vector3(180, 0, 90),
            new Vector3(180, 90, 0),
            new Vector3(180, 90, 90),
            new Vector3(180, 90, 180),
            new Vector3(180, 180, 0),
            new Vector3(180, 180, 90)
        };

        // 获取当前旋转值
        MaterialProperty rotXProp = FindProperty("_ExtraRotX", properties);
        MaterialProperty rotYProp = FindProperty("_ExtraRotY", properties);
        MaterialProperty rotZProp = FindProperty("_ExtraRotZ", properties);

        Vector3 currentRotation = new Vector3(rotXProp.floatValue, rotYProp.floatValue, rotZProp.floatValue);

        // 查找当前选择的索引
        int selectedIndex = 0;
        for (int i = 0; i < rotationValues.Length; i++)
        {
            if (Vector3.Distance(currentRotation, rotationValues[i]) < 0.1f)
            {
                selectedIndex = i;
                break;
            }
        }

        // 绘制下拉菜单
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(GazeShaderLocalization.Variations.QuickOrientation));
        int newIndex = EditorGUILayout.Popup(selectedIndex, quickOrientations);
        EditorGUILayout.EndHorizontal();

        // 应用选择的旋转值
        if (newIndex != selectedIndex)
        {
            rotXProp.floatValue = rotationValues[newIndex].x;
            rotYProp.floatValue = rotationValues[newIndex].y;
            rotZProp.floatValue = rotationValues[newIndex].z;
        }

        EditorGUILayout.EndVertical();
    }
    private void AutoDesyncObjects()
    {
        var renderers = Object.FindObjectsOfType<Renderer>();
        int count = 0;
        float baseOffset = 0.00001f; // 极限隐形差异值 (0.00001)
        
        foreach (var r in renderers)
        {
            // 只处理当前选中的材质球
            if (r.sharedMaterial == targetMaterial)
            {
                Undo.RecordObject(r.transform, "Auto Desync Objects");
                
                Vector3 s = r.transform.localScale;
                // 先把旧的微调抹平，保留两位小数的主缩放（防止多次点击导致物体无限变大）
                s.x = (float)System.Math.Round(s.x, 2);
                s.y = (float)System.Math.Round(s.y, 2);
                s.z = (float)System.Math.Round(s.z, 2);
                
                // 物体会被随机分配诸如 1.00037, 1.00081 的缩放值
                float randomMultiplier = UnityEngine.Random.Range(1f, 999f);
                float offset = randomMultiplier * baseOffset;
                
                // 将随机出来的微小不可见偏移，加到原始缩放上
                r.transform.localScale = new Vector3(s.x + offset, s.y + offset, s.z + offset);
                count++;
                
                EditorUtility.SetDirty(r.transform);
            }
        }
        
        // 多语言弹窗
        if (count > 0)
        {
            EditorUtility.DisplayDialog(
                GazeShaderLocalization.GetString("AUTO_DESYNC_SUCCESS_TITLE"), 
                GazeShaderLocalization.GetString("AUTO_DESYNC_SUCCESS_DESC", count), 
                GazeShaderLocalization.GetString("OK")
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                GazeShaderLocalization.GetString("PROMPT"), 
                GazeShaderLocalization.GetString("AUTO_DESYNC_NO_OBJECTS"), 
                GazeShaderLocalization.GetString("OK")
            );
        }
    }
}