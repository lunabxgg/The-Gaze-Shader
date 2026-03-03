using UnityEngine;
using UnityEditor;

namespace Luna.GazeShader
{
    public static class AuthorInfoDrawer
    {
        private const string BOOTH_LOGO_PATH = "Assets/@Luna/gaze gif shader/logo/s booth.png";
        private const string TWITTER_LOGO_PATH = "Assets/@Luna/gaze gif shader/logo/s x.png";
        private const string BILIBILI_LOGO_PATH = "Assets/@Luna/gaze gif shader/logo/s b.png";
        private const string AUTHOR_LOGO_PATH = "Assets/@Luna/gaze gif shader/logo/@咸鱼子Luna.png";
        private const string LUNA_KO_LOGO_PATH = "Assets/@Luna/gaze gif shader/logo/Luna_ko.png";

        private const string BOOTH_URL = "https://xianyuzi-luna.booth.pm/";
        private const string TWITTER_URL = "https://x.com/lunabxgg";
        private const string BILIBILI_URL = "https://space.bilibili.com/3546752913247086?spm_id_from=333.1007.0.0";

        private const int BUTTON_SIZE = 30;
        private const int AUTHOR_LOGO_WIDTH = 105;
        private const int AUTHOR_LOGO_HEIGHT = 19;
        private const int LUNA_KO_SIZE = 47;
        private const int SPACING = 3;

        public static void DrawAuthorInfo()
        {
            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawContent();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
        }

        private static void DrawContent()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawButtons();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawAuthorLogo();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            DrawLunaKoLogo();
        }

        private static void DrawButtons()
        {
            Texture2D boothLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(BOOTH_LOGO_PATH);
            Texture2D twitterLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(TWITTER_LOGO_PATH);
            Texture2D bilibiliLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(BILIBILI_LOGO_PATH);

            if (boothLogo != null)
            {
                if (GUILayout.Button(new GUIContent(boothLogo, GazeShaderLocalization.AuthorInfo.BoothTooltip),
                    GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    Application.OpenURL(BOOTH_URL);
                }
            }
            else
            {
                DrawFallbackButton("Booth", BOOTH_URL);
            }

            GUILayout.Space(SPACING);

            if (twitterLogo != null)
            {
                if (GUILayout.Button(new GUIContent(twitterLogo, GazeShaderLocalization.AuthorInfo.TwitterTooltip),
                    GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    Application.OpenURL(TWITTER_URL);
                }
            }
            else
            {
                DrawFallbackButton("X", TWITTER_URL);
            }

            GUILayout.Space(SPACING);

            if (bilibiliLogo != null)
            {
                if (GUILayout.Button(new GUIContent(bilibiliLogo, GazeShaderLocalization.AuthorInfo.BilibiliTooltip),
                    GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    Application.OpenURL(BILIBILI_URL);
                }
            }
            else
            {
                DrawFallbackButton("Bili", BILIBILI_URL);
            }
        }

        private static void DrawAuthorLogo()
        {
            Texture2D authorLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(AUTHOR_LOGO_PATH);

            if (authorLogo != null)
            {
                Rect logoRect = GUILayoutUtility.GetRect(AUTHOR_LOGO_WIDTH, AUTHOR_LOGO_HEIGHT);
                GUI.DrawTexture(logoRect, authorLogo, ScaleMode.StretchToFill);
            }
            else
            {
                GUILayout.Label("Luna", GUILayout.Width(AUTHOR_LOGO_WIDTH), GUILayout.Height(AUTHOR_LOGO_HEIGHT));
            }
        }

        private static void DrawLunaKoLogo()
        {
            Texture2D lunaKoLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(LUNA_KO_LOGO_PATH);

            if (lunaKoLogo != null)
            {
                Rect logoRect = GUILayoutUtility.GetRect(LUNA_KO_SIZE, LUNA_KO_SIZE);
                GUI.DrawTexture(logoRect, lunaKoLogo, ScaleMode.ScaleToFit);
            }
            else
            {
                GUILayout.Label("Luna", GUILayout.Width(LUNA_KO_SIZE), GUILayout.Height(LUNA_KO_SIZE));
            }
        }

        private static void DrawFallbackButton(string label, string url)
        {
            if (GUILayout.Button(label, GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
            {
                Application.OpenURL(url);
            }
        }
    }
}