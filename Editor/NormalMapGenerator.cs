using UnityEngine;
using UnityEditor;
using System.IO;

namespace Luna.GazeShader
{
    public static class NormalMapGenerator
    {
        public static Texture2DArray GenerateNormalMapFromTextureArray(Texture2DArray sourceTextureArray, float strength = 1.0f)
        {
            if (sourceTextureArray == null) return null;

            int width = sourceTextureArray.width;
            int height = sourceTextureArray.height;
            int depth = sourceTextureArray.depth;

            Texture2DArray normalMapArray = new Texture2DArray(width, height, depth, TextureFormat.RGBA32, false);
            normalMapArray.wrapMode = TextureWrapMode.Repeat;
            normalMapArray.filterMode = FilterMode.Bilinear;

            for (int slice = 0; slice < depth; slice++)
            {
                Texture2D sourceTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                RenderTexture renderTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            
                Graphics.Blit(sourceTextureArray, renderTex, slice, 0);
                RenderTexture.active = renderTex;
                sourceTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                sourceTex.Apply();
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(renderTex);

                Texture2D normalMap = GenerateNormalMap(sourceTex, strength);

                Graphics.CopyTexture(normalMap, 0, 0, normalMapArray, slice, 0);

                Object.DestroyImmediate(sourceTex);
                Object.DestroyImmediate(normalMap);
            }

            return normalMapArray;
        }

        public static Texture2D GenerateNormalMapFromTexture(Texture2D sourceTexture, float strength = 1.0f)
        {
            if (sourceTexture == null) return null;
            return GenerateNormalMap(sourceTexture, strength);
        }

        private static Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            int width = source.width;
            int height = source.height;
            Texture2D normalMap = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float left = GetBrightness(source, x - 1, y);
                    float right = GetBrightness(source, x + 1, y);
                    float up = GetBrightness(source, x, y - 1);
                    float down = GetBrightness(source, x, y + 1);

                    float left2 = GetBrightness(source, x - 2, y);
                    float right2 = GetBrightness(source, x + 2, y);
                    float up2 = GetBrightness(source, x, y - 2);
                    float down2 = GetBrightness(source, x, y + 2);

                    float dX = (left - right + 0.5f * (left2 - right2)) * strength;
                    float dY = (up - down + 0.5f * (up2 - down2)) * strength;
                    float dZ = 1.0f;

                    Vector3 normal = new Vector3(dX, dY, dZ).normalized;

                    Color normalColor = new Color(
                        normal.x * 0.5f + 0.5f,
                        normal.y * 0.5f + 0.5f,
                        normal.z * 0.5f + 0.5f,
                        1.0f
                    );

                    normalMap.SetPixel(x, y, normalColor);
                }
            }

            normalMap.Apply();
            return normalMap;
        }

        private static float GetBrightness(Texture2D tex, int x, int y)
        {
            x = Mathf.Clamp(x, 0, tex.width - 1);
            y = Mathf.Clamp(y, 0, tex.height - 1);

            Color color = tex.GetPixel(x, y);
            return color.grayscale;
        }
    }
}