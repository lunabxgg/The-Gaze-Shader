using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

namespace Luna.GazeShader
{
    public static class GifToTextureArrayConverter
    {
        public static void ConvertGif(MaterialProperty gifTextureProp, int selectedTextureSize,
            Language currentLanguage, Material targetMaterial, MaterialProperty[] properties)
        {
            if (gifTextureProp.textureValue == null)
            {
                string errorMsg = GazeShaderLocalization.Converter.SelectGifError;
                EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Error, errorMsg, "OK");
                return;
            }

            string gifPath = AssetDatabase.GetAssetPath(gifTextureProp.textureValue);
            if (string.IsNullOrEmpty(gifPath) || Path.GetExtension(gifPath).ToLower() != ".gif")
            {
                string errorMsg = GazeShaderLocalization.Converter.NotGifError;
                EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Error, errorMsg, "OK");
                return;
            }

            try
            {
                string progressTitle = GazeShaderLocalization.Converter.Converting;
                EditorUtility.DisplayProgressBar(progressTitle, GazeShaderLocalization.Converter.Processing, 0.3f);

                Texture2DArray textureArray = OriginalGifToTextureArray(gifPath, selectedTextureSize);

                if (textureArray != null)
                {
                    string baseName = Path.GetFileNameWithoutExtension(gifPath);
                    string targetDir = Path.GetDirectoryName(gifPath);
                    string baseFileName = $"{baseName}_2DArray_{selectedTextureSize}";
                    string extension = ".asset";

                    string finalFileName = baseFileName + extension;
                    string finalPath = Path.Combine(targetDir, finalFileName);

                    if (File.Exists(finalPath))
                    {
                        int counter = 1;
                        do
                        {
                            finalFileName = $"{baseFileName}_{counter}{extension}";
                            finalPath = Path.Combine(targetDir, finalFileName);
                            counter++;
                        } while (File.Exists(finalPath));
                    }

                    AssetDatabase.CreateAsset(textureArray, finalPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Texture2DArray loadedArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(finalPath);

                    if (loadedArray != null)
                    {
                        MaterialProperty texturesProp = FindPropertyInArray("_Textures", properties);
                        texturesProp.textureValue = loadedArray;
                        targetMaterial.SetTexture("_Textures", loadedArray);

                        // 设置默认值确保统一播放
                        targetMaterial.SetFloat("_StartFrameRandomization", 0f); // 确保默认统一播放
                        targetMaterial.SetFloat("_PlayMode", 0f); // 默认循环模式
                        targetMaterial.SetFloat("_ManualFrame", 0f); // 默认第一帧

                        // 设置凝视轴默认开启
                        targetMaterial.SetFloat("_GazeRotX", 1f);
                        targetMaterial.SetFloat("_GazeRotY", 1f);
                        targetMaterial.SetFloat("_GazeRotZ", 1f);

                        // 设置背面剔除默认关闭
                        targetMaterial.SetFloat("_BackfaceCulling", 0f);
                        targetMaterial.DisableKeyword("_BACKFACE_CULLING"); // 默认禁用背面剔除关键字

                        // 设置材质的默认关键字状态
                        SetupMaterialKeywords(targetMaterial);

                        EditorUtility.SetDirty(targetMaterial);

                        string successMsg = GazeShaderLocalization.Converter.GetString("CONVERSION_SUCCESS",
                            loadedArray.width, loadedArray.height, loadedArray.depth, finalFileName);
                        EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Success, successMsg, "OK");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GIF conversion failed: {e.Message}");
                string errorMsg = GazeShaderLocalization.Converter.GetString("CONVERSION_ERROR", e.Message);
                EditorUtility.DisplayDialog(GazeShaderLocalization.MaterialManager.Error, errorMsg, "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        // 设置材质的关键字状态
        private static void SetupMaterialKeywords(Material material)
        {
            // 根据材质的Toggle属性设置对应的关键字
            SetupKeyword(material, "_GAZE_ON", material.GetFloat("_Gaze") > 0.5f);
            SetupKeyword(material, "_FIX_TRANSPARENCY", material.GetFloat("_FixTransp") > 0.5f);
        }

        // 设置单个关键字
        private static void SetupKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        private static MaterialProperty FindPropertyInArray(string propertyName, MaterialProperty[] properties)
        {
            foreach (var prop in properties)
            {
                if (prop.name == propertyName)
                    return prop;
            }
            return null;
        }

        private static Texture2DArray OriginalGifToTextureArray(string path, int selectedTextureSize)
        {
            List<Texture2D> frames = GetGifFrames(path, selectedTextureSize);
            if (frames == null || frames.Count == 0)
            {
                Debug.LogError("GIF is empty or System.Drawing is not working.");
                return null;
            }

            return CreateTexture2DArrayFromFrames(frames.ToArray(), selectedTextureSize);
        }

        private static List<Texture2D> GetGifFrames(string path, int selectedTextureSize)
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

                    Texture2D resizedFrame = ResizeTexture(frame, selectedTextureSize);
                    gifFrames.Add(resizedFrame);

                    if (EditorUtility.DisplayCancelableProgressBar(GazeShaderLocalization.Converter.Converting,
                        $"Processing frame {i + 1}/{frameCount}", (float)i / frameCount))
                    {
                        break;
                    }
                }

                gifImage.Dispose();
            }
            catch (System.Exception e)
            {
                // 只有当 System.Drawing 真的在某台电脑上崩溃时，才使用备用方案防报错
                Debug.LogError($"System.Drawing error: {e.Message}");
                return CreateFallbackFrames(path, selectedTextureSize);
            }

            EditorUtility.ClearProgressBar();
            return gifFrames;
        }
        private static Texture2D ResizeTexture(System.Drawing.Bitmap source, int targetSize)
        {
            // 计算原始宽高比和缩放比例
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            // 确定缩放比例 - 以长边为基准
            float scale;
            if (sourceWidth > sourceHeight)
            {
                scale = (float)targetSize / sourceWidth;
            }
            else
            {
                scale = (float)targetSize / sourceHeight;
            }

            // 计算缩放后的尺寸
            int newWidth = Mathf.RoundToInt(sourceWidth * scale);
            int newHeight = Mathf.RoundToInt(sourceHeight * scale);

            // 计算居中位置
            int offsetX = (targetSize - newWidth) / 2;
            int offsetY = (targetSize - newHeight) / 2;

            // 创建目标位图
            var resized = new System.Drawing.Bitmap(targetSize, targetSize);
            using (var graphics = System.Drawing.Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // 清除背景为透明
                graphics.Clear(System.Drawing.Color.Transparent);

                // 绘制缩放后的图像到居中位置
                graphics.DrawImage(source, offsetX, offsetY, newWidth, newHeight);
            }

            // 转换为Unity纹理
            Texture2D texture = new Texture2D(targetSize, targetSize, TextureFormat.RGBA32, false);

            for (int x = 0; x < targetSize; x++)
            {
                for (int y = 0; y < targetSize; y++)
                {
                    System.Drawing.Color sourceColor = resized.GetPixel(x, y);
                    texture.SetPixel(x, targetSize - 1 - y,
                        new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                }
            }

            texture.Apply();
            resized.Dispose();
            return texture;
        }

        private static List<Texture2D> CreateFallbackFrames(string path, int selectedTextureSize)
        {
            List<Texture2D> fallbackFrames = new List<Texture2D>();

            Texture2D gifTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (gifTexture != null)
            {
                // 创建临时RenderTexture用于缩放
                RenderTexture rt = RenderTexture.GetTemporary(selectedTextureSize, selectedTextureSize, 0, RenderTextureFormat.ARGB32);
                RenderTexture.active = rt;

                // 计算缩放比例和偏移量
                float sourceAspect = (float)gifTexture.width / gifTexture.height;
                float targetAspect = 1.0f; // 正方形

                Vector2 scale, offset;
                if (sourceAspect > targetAspect)
                {
                    // 原始图像更宽
                    scale = new Vector2(1.0f, targetAspect / sourceAspect);
                    offset = new Vector2(0, (1 - scale.y) * 0.5f);
                }
                else
                {
                    // 原始图像更高
                    scale = new Vector2(sourceAspect / targetAspect, 1.0f);
                    offset = new Vector2((1 - scale.x) * 0.5f, 0);
                }

                // 设置缩放和偏移
                GL.PushMatrix();
                GL.LoadOrtho();

                for (int i = 0; i < 4; i++)
                {
                    Graphics.Blit(gifTexture, rt);

                    Texture2D frame = new Texture2D(selectedTextureSize, selectedTextureSize, TextureFormat.RGBA32, false);
                    frame.ReadPixels(new Rect(0, 0, selectedTextureSize, selectedTextureSize), 0, 0);
                    frame.Apply();
                    fallbackFrames.Add(frame);
                }

                GL.PopMatrix();
                RenderTexture.ReleaseTemporary(rt);
                RenderTexture.active = null;
            }

            return fallbackFrames;
        }

        private static Texture2DArray CreateTexture2DArrayFromFrames(Texture2D[] frames, int selectedTextureSize)
        {
            if (frames == null || frames.Length == 0)
                return null;

            Texture2DArray textureArray = new Texture2DArray(selectedTextureSize, selectedTextureSize, frames.Length, TextureFormat.RGBA32, false);

            for (int i = 0; i < frames.Length; i++)
            {
                Graphics.CopyTexture(frames[i], 0, 0, textureArray, i, 0);
            }

            textureArray.Apply();
            return textureArray;
        }
    }
}