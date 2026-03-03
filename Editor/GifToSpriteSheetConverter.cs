using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Luna.GazeShader
{
    public static class GifToSpriteSheetConverter
    {
        public static void ConvertGifToSpriteSheet(MaterialProperty gifTextureProp, bool highQuality,
            Material targetMaterial, MaterialProperty[] properties)
        {
            if (gifTextureProp.textureValue == null)
            {
                EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Error, 
                    GazeShaderLocalization.Converter.SelectGifError, "OK");
                return;
            }

            string gifPath = AssetDatabase.GetAssetPath(gifTextureProp.textureValue);
            
            try
            {

                EditorUtility.DisplayProgressBar("Sprite Sheet", GazeShaderLocalization.GetString("ANALYZING_GIF"), 0.1f);
                List<Texture2D> frames = GetGifFrames(gifPath);
                if (frames == null || frames.Count == 0) return;

                int frameCount = frames.Count;
                int frameWidth = frames[0].width;
                int frameHeight = frames[0].height;

                // 1. 计算最佳行列布局
                int columns = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                int rows = Mathf.CeilToInt((float)frameCount / columns);

                // 2. 智能缩放算法：反推最大单帧尺寸
                // 高性能模式：大图不超过 2048x2048；高画质模式：大图不超过 4096x4096
                int maxAtlasSize = highQuality ? 4096 : 2048;
                
                // 计算在不超标的情况下，每一格最大能有多大
                float maxCellW = (float)maxAtlasSize / columns;
                float maxCellH = (float)maxAtlasSize / rows;

                // 计算缩放比例 (严禁放大，只允许原比例或缩小)
                float scaleX = maxCellW / frameWidth;
                float scaleY = maxCellH / frameHeight;
                float scale = Mathf.Min(1f, Mathf.Min(scaleX, scaleY));

                int finalCellW = Mathf.RoundToInt(frameWidth * scale);
                int finalCellH = Mathf.RoundToInt(frameHeight * scale);
                int atlasW = finalCellW * columns;
                int atlasH = finalCellH * rows;

                // 3. 创建拼合大图
                Texture2D atlas = new Texture2D(atlasW, atlasH, TextureFormat.RGBA32, false);
                Color[] clearColors = new Color[atlasW * atlasH];
                for(int i=0; i<clearColors.Length; i++) clearColors[i] = Color.clear;
                atlas.SetPixels(clearColors);

                for (int i = 0; i < frameCount; i++)
                {
                    int col = i % columns;
                    int row = rows - 1 - (i / columns);

                    Texture2D processedFrame = (scale < 0.99f) ? ResizeTexture(frames[i], finalCellW, finalCellH) : frames[i];
                    atlas.SetPixels(col * finalCellW, row * finalCellH, finalCellW, finalCellH, processedFrame.GetPixels());
                    
                    if (scale < 0.99f) Object.DestroyImmediate(processedFrame);
                    Object.DestroyImmediate(frames[i]);

                    string progressMsg = GazeShaderLocalization.GetString("PACKING_FRAMES", i + 1, frameCount);
                    EditorUtility.DisplayProgressBar("Sprite Sheet", progressMsg, 0.2f + (0.6f * i / frameCount));
                }
                atlas.Apply();

                // 4. 自动防覆盖命名逻辑
                string baseName = Path.GetFileNameWithoutExtension(gifPath);
                string targetDir = Path.GetDirectoryName(gifPath);
                string baseFileName = $"{baseName}_SpriteSheet";
                string extension = ".png";

                string finalFileName = baseFileName + extension;
                string finalPath = Path.Combine(targetDir, finalFileName);

                int counter = 1;
                while (File.Exists(finalPath))
                {
                    finalFileName = $"{baseFileName}_{counter}{extension}";
                    finalPath = Path.Combine(targetDir, finalFileName);
                    counter++;
                }

                // 5. 保存与配置
                byte[] bytes = atlas.EncodeToPNG();
                File.WriteAllBytes(finalPath, bytes);
                AssetDatabase.Refresh();
                ConfigureTextureImportSettings(finalPath);

                // 6. 自动分配参数
                Texture2D savedAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(finalPath);
                if (savedAtlas != null)
                {
                    targetMaterial.SetTexture("_MainTex", savedAtlas);
                    targetMaterial.SetFloat("_Columns", columns);
                    targetMaterial.SetFloat("_Rows", rows);
                    targetMaterial.SetFloat("_TotalFrames", frameCount);
                    targetMaterial.SetFloat("_LightingEffect", 1.0f);
                    targetMaterial.EnableKeyword("_LIGHTING_EFFECT");

                    string qualityText = highQuality ? 
                        GazeShaderLocalization.GetString("QUALITY_TEXT_HIGH") : 
                        GazeShaderLocalization.GetString("QUALITY_TEXT_LOW");

                    string successDesc = GazeShaderLocalization.GetString("SPRITE_SHEET_SUCCESS_DESC", 
                        finalFileName, qualityText, atlasW, atlasH, columns, rows);

                    EditorUtility.DisplayDialog(
                        GazeShaderLocalization.GetString("SPRITE_SHEET_SUCCESS_TITLE"), 
                        successDesc, 
                        "OK");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Sprite Sheet conversion failed: {e.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void ConfigureTextureImportSettings(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                // 1. 先将类型设置为 Sprite
                importer.textureType = TextureImporterType.Sprite;
                
                // 2. 获取 Importer 的设置对象来修改具体的 Sprite 属性
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                
                // 设置为 FullRect 以防止边缘被自动裁剪
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteExtrude = 0;
                settings.spriteGenerateFallbackPhysicsShape = false;
                importer.SetTextureSettings(settings);

                // 3. 通用设置
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.wrapMode = TextureWrapMode.Clamp;

                // 4. 设置平台压缩
                TextureImporterPlatformSettings platformSettings = importer.GetDefaultPlatformTextureSettings();
                platformSettings.format = TextureImporterFormat.Automatic;
                platformSettings.compressionQuality = 50; 
                importer.SetPlatformTextureSettings(platformSettings);

                // 保存并重新导入
                importer.SaveAndReimport();
            }
        }

        // --- 核心逻辑：从 GIF 文件中提取每一帧 ---
        private static List<Texture2D> GetGifFrames(string path)
        {
            List<Texture2D> gifFrames = new List<Texture2D>();
            try
            {
                var gifImage = System.Drawing.Image.FromFile(path);
                var dimension = new System.Drawing.Imaging.FrameDimension(gifImage.FrameDimensionsList[0]);
                int frameCount = gifImage.GetFrameCount(dimension);

                for (int i = 0; i < frameCount; i++)
                {
                    gifImage.SelectActiveFrame(dimension, i);
                    var frame = new System.Drawing.Bitmap(gifImage.Width, gifImage.Height);
                    System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, System.Drawing.Point.Empty);
                    
                    // 转换为 Unity Texture2D
                    Texture2D tex = new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false);
                    for (int x = 0; x < frame.Width; x++)
                    {
                        for (int y = 0; y < frame.Height; y++)
                        {
                            System.Drawing.Color c = frame.GetPixel(x, y);
                            tex.SetPixel(x, frame.Height - 1 - y, new Color32(c.R, c.G, c.B, c.A));
                        }
                    }
                    tex.Apply();
                    gifFrames.Add(tex);
                    frame.Dispose();

                    if (EditorUtility.DisplayCancelableProgressBar("Sprite Sheet", $"提取帧 {i+1}/{frameCount}", (float)i / frameCount))
                        break;
                }
                gifImage.Dispose();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GIF 帧提取失败: {e.Message}");
                return null;
            }
            return gifFrames;
        }

        // 专为保留原图比例设计的缩放函数
        private static Texture2D ResizeTexture(Texture2D source, int targetX, int targetY)
        {
            RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 0, RenderTextureFormat.ARGB32);
            RenderTexture.active = rt;
            // 使用线性过滤保证缩放质量
            source.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, rt);
            
            Texture2D result = new Texture2D(targetX, targetY, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return result;
        }
    }
}