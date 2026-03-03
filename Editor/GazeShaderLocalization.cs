using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Luna.GazeShader
{
    public enum Language
    {
        En, // English
        Cn, // 中文
        Ja  // 日本語
    }

    public static class GazeShaderLocalization
    {
        private const string LANGUAGE_KEY = "GazeShader_Language";

        private static Language? _currentLanguage;

        public static Language CurrentLanguage
        {
            get
            {
                if (_currentLanguage == null)
                {
                    LoadLanguageSetting();
                }
                return _currentLanguage.Value;
            }
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    SaveLanguageSetting();
                }
            }
        }

        // 主获取方法
        public static string GetString(string key)
        {
            return GetLocalizedString(key, CurrentLanguage);
        }

        public static string GetString(string key, params object[] args)
        {
            var format = GetLocalizedString(key, CurrentLanguage);
            return string.Format(format, args);
        }

        // 路径常量
        public static class Paths
        {
            public const string BOOTH_LOGO = "Assets/@Luna/gaze gif shader/logo/s booth.png";
            public const string TWITTER_LOGO = "Assets/@Luna/gaze gif shader/logo/s x.png";
            public const string BILIBILI_LOGO = "Assets/@Luna/gaze gif shader/logo/s b.png";
            public const string AUTHOR_LOGO = "Assets/@Luna/gaze gif shader/logo/@咸鱼子Luna.png";
            public const string LUNA_KO_LOGO = "Assets/@Luna/gaze gif shader/logo/Luna_ko.png";

            public const string BOOTH_URL = "https://xianyuzi-luna.booth.pm/";
            public const string TWITTER_URL = "https://x.com/lunabxgg";
            public const string BILIBILI_URL = "https://space.bilibili.com/3546752913247086?spm_id_from=333.1007.0.0";
        }

        // 界面常量
        public static class Sizes
        {
            public const int BUTTON_SIZE = 30;
            public const int AUTHOR_LOGO_WIDTH = 105;
            public const int AUTHOR_LOGO_HEIGHT = 19;
            public const int LUNA_KO_SIZE = 47;
            public const int SPACING = 3;
        }

        // 主界面文本
        public static class MainUI
        {
            public static string Title => GetString("MAIN_TITLE");
            public static string MaterialManager => GetString("MATERIAL_MANAGER");
            public static string LanguageLabel => GetString("LANGUAGE");
            public static string[] LanguageOptions => new[] { "En", "中", "日" };
            public static string Description => GetString("DESCRIPTION");
        }

        // 资源部分文本
        public static class Resources
        {
            public static string GifConversion => GetString("GIF_CONVERSION");
            public static string TextureArray => GetString("TEXTURE_ARRAY");
            public static string GifSourceFile => GetString("GIF_SOURCE_FILE");
            public static string Selected => GetString("SELECTED");
            public static string TextureSize => GetString("TEXTURE_SIZE");
            public static string ConvertButton => GetString("CONVERT_BUTTON");
            public static string NotGifWarning => GetString("NOT_GIF_WARNING");
            public static string SelectGifPrompt => GetString("SELECT_GIF_PROMPT");
            public static string ReadyStatus => GetString("READY_STATUS");
            public static string NoArrayWarning => GetString("NO_ARRAY_WARNING");
            public static string OptimizeMaterials => GetString("OPTIMIZE_MATERIALS");
            public static string OptimizeTooltip => GetString("OPTIMIZE_TOOLTIP");

            // 添加GetString方法
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }

            public static string GetString(string key, params object[] args)
            {
                return GazeShaderLocalization.GetString(key, args);
            }
        }

        // 效果部分文本
        public static class Effects
        {
            // 在 Effects 类中添加：         
            public static string SpeedChangeMode => GetString("SPEED_CHANGE_MODE");
            public static string SpeedChangeRate => GetString("SPEED_CHANGE_RATE");
            public static string SpeedModeUniform => GetString("SPEED_MODE_UNIFORM");
            public static string SpeedModeAccelerate => GetString("SPEED_MODE_ACCELERATE");
            public static string SpeedModeDecelerate => GetString("SPEED_MODE_DECELERATE");
            public static string[] SpeedModeOptions => new[]
            {
                GetString("SPEED_MODE_UNIFORM"),
                GetString("SPEED_MODE_ACCELERATE"),
                GetString("SPEED_MODE_DECELERATE")
            };

            public static string AnimationSettings => GetString("ANIMATION_SETTINGS");
            public static string GazeEffect => GetString("GAZE_EFFECT");
            public static string Color => GetString("COLOR");
            public static string BaseFPS => GetString("BASE_FPS");
            public static string PlayMode => GetString("PLAY_MODE");
            public static string[] PlayModeOptions => new[]
            {
        GetString("PLAY_MODE_LOOP"),
        GetString("PLAY_MODE_ONCE"),
        GetString("PLAY_MODE_RANDOM"),
        GetString("PLAY_MODE_MANUAL")
    };

            public static string ManualFrame => GetString("MANUAL_FRAME");
            public static string StartFrameRandomization => GetString("START_FRAME_RANDOMIZATION");
            public static string StartFrameTooltip => GetString("START_FRAME_TOOLTIP");
            public static string Gaze => GetString("GAZE");
            public static string GazeTooltip => GetString("GAZE_TOOLTIP");

            // 添加单轴凝视相关文本
            public static string SingleAxisGaze => GetString("SINGLE_AXIS_GAZE");
            public static string SingleAxisGazeTooltip => GetString("SINGLE_AXIS_GAZE_TOOLTIP");
            public static string[] SingleAxisGazeOptions => new[]
            {
        GetString("SINGLE_AXIS_ALL"),
        GetString("SINGLE_AXIS_X"),
        GetString("SINGLE_AXIS_Y"),
        GetString("SINGLE_AXIS_Z")
    };

            public static string WeakenDistanceGaze => GetString("WEAKEN_DISTANCE_GAZE");

            // 添加GetString方法
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }
        }

        // 变化部分文本
        public static class Variations
        {
            public static string Fixed => GetString("FIXED_VARIATIONS");
            public static string Random => GetString("RANDOM_VARIATIONS");
            public static string ExtraXRotation => GetString("EXTRA_X_ROTATION");
            public static string ExtraYRotation => GetString("EXTRA_Y_ROTATION");
            public static string ExtraZRotation => GetString("EXTRA_Z_ROTATION");
            public static string ScaleVariation => GetString("SCALE_VARIATION");
            public static string ScaleVariationTooltip => GetString("SCALE_VARIATION_TOOLTIP");
            public static string RandomXRotation => GetString("RANDOM_X_ROTATION");
            public static string RandomXRotationTooltip => GetString("RANDOM_X_ROTATION_TOOLTIP");
            public static string RandomYRotation => GetString("RANDOM_Y_ROTATION");
            public static string RandomYRotationTooltip => GetString("RANDOM_Y_ROTATION_TOOLTIP");
            public static string RandomZRotation => GetString("RANDOM_Z_ROTATION");
            public static string RandomZRotationTooltip => GetString("RANDOM_Z_ROTATION_TOOLTIP");

            public static string QuickOrientation => GetString("QUICK_ORIENTATION");
            public static string QuickOrientationTooltip => GetString("QUICK_ORIENTATION_TOOLTIP");

            public static string AutoDesyncButton => GetString("AUTO_DESYNC_BUTTON");
            public static string AutoDesyncTooltip => GetString("AUTO_DESYNC_TOOLTIP");
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }
        }

        // 高级设置文本
        public static class Advanced
        {
            public static string UVAdjust => GetString("UV_ADJUST");
            public static string DisplayFix => GetString("DISPLAY_FIX");
            public static string UVXOffset => GetString("UV_X_OFFSET");
            public static string UVYOffset => GetString("UV_Y_OFFSET");
            public static string XScale => GetString("X_SCALE");
            public static string YScale => GetString("Y_SCALE");
            

            public static string BackfaceCulling => GetString("BACKFACE_CULLING");
            public static string BackfaceCullingTooltip => GetString("BACKFACE_CULLING_TOOLTIP");
            public static string LightingEffect => GetString("LIGHTING_EFFECT");
            public static string LightingEffectTooltip => GetString("LIGHTING_EFFECT_TOOLTIP");
            public static string FixArtifacts => GetString("FIX_ARTIFACTS");
            public static string FixArtifactsTooltip => GetString("FIX_ARTIFACTS_TOOLTIP");

            public static string FlipHorizontal => GetString("FLIP_HORIZONTAL");
            public static string FlipVertical => GetString("FLIP_VERTICAL");

            // 添加GetString方法
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }
        }

        // 材质管理器文本
        public static class MaterialManager
        {
            public static string Title => GetString("MATERIAL_MANAGER_TITLE");
            public static string Description => GetString("MATERIAL_MANAGER_DESC");
            public static string QuickActions => GetString("QUICK_ACTIONS");
            public static string AutoOptimizeAll => GetString("AUTO_OPTIMIZE_ALL");
            public static string RefreshScan => GetString("REFRESH_SCAN");
            public static string MaterialGroups => GetString("MATERIAL_GROUPS");
            public static string NoMaterialsWarning => GetString("NO_MATERIALS_WARNING");
            public static string TextureArrayInfo => GetString("TEXTURE_ARRAY_INFO");
            public static string NoTextureArray => GetString("NO_TEXTURE_ARRAY");
            public static string Status => GetString("STATUS");
            public static string SharedInstances => GetString("SHARED_INSTANCES");
            public static string Select => GetString("SELECT");
            public static string Apply => GetString("APPLY");
            public static string CreateSharedInstance => GetString("CREATE_SHARED_INSTANCE");
            public static string OptimizeScene => GetString("OPTIMIZE_SCENE");
            public static string AdvancedActions => GetString("ADVANCED_ACTIONS");
            public static string CreateAllShared => GetString("CREATE_ALL_SHARED");
            public static string CleanupUnused => GetString("CLEANUP_UNUSED");
            public static string HowToUse => GetString("HOW_TO_USE");
            public static string Instructions => GetString("INSTRUCTIONS");
            public static string EnableAutoReplace => GetString("ENABLE_AUTO_REPLACE");
            public static string Success => GetString("SUCCESS");
            public static string Error => GetString("ERROR");
            public static string Warning => GetString("WARNING");
            public static string Complete => GetString("COMPLETE");

            // 添加GetString方法
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }

            public static string GetString(string key, params object[] args)
            {
                return GazeShaderLocalization.GetString(key, args);
            }
        }

        // 转换器文本
        public static class Converter
        {
            public static string Converting => GetString("CONVERTING");
            public static string Processing => GetString("PROCESSING");
            public static string ConversionSuccess => GetString("CONVERSION_SUCCESS");
            public static string ConversionError => GetString("CONVERSION_ERROR");
            public static string SelectGifError => GetString("SELECT_GIF_ERROR");
            public static string NotGifError => GetString("NOT_GIF_ERROR");

            // 添加GetString方法
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }

            public static string GetString(string key, params object[] args)
            {
                return GazeShaderLocalization.GetString(key, args);
            }
        }

        // 作者信息文本
        public static class AuthorInfo
        {
            public static string BoothTooltip => GetString("BOOTH_TOOLTIP");
            public static string TwitterTooltip => GetString("TWITTER_TOOLTIP");
            public static string BilibiliTooltip => GetString("BILIBILI_TOOLTIP");

            // 添加GetString方法
            public static string GetString(string key)
            {
                return GazeShaderLocalization.GetString(key);
            }
        }

        // 私有方法
        private static string GetLocalizedString(string key, Language language)
        {
            return localizedStrings.ContainsKey(key) ?
                localizedStrings[key][(int)language] :
                $"[MISSING:{key}]";
        }

        private static void LoadLanguageSetting()
        {
            string key = $"{LANGUAGE_KEY}_{Application.productName}";
            if (EditorPrefs.HasKey(key))
            {
                _currentLanguage = (Language)EditorPrefs.GetInt(key);
            }
            else
            {
                _currentLanguage = Language.En; // 默认英语
            }
        }

        private static void SaveLanguageSetting()
        {
            string key = $"{LANGUAGE_KEY}_{Application.productName}";
            EditorPrefs.SetInt(key, (int)_currentLanguage.Value);
        }

        // 本地化字符串字典
        private static readonly Dictionary<string, string[]> localizedStrings = new Dictionary<string, string[]>
        {
            // 主界面
            ["MAIN_TITLE"] = new[] { "The Gaze Shader", "凝视着色器", "凝视シェーダー" },
            ["MATERIAL_MANAGER"] = new[] { "Material Manager", "材质管理器", "マテリアルマネージャー" },
            ["LANGUAGE"] = new[] { "Language", "语言", "言語" },
            ["DESCRIPTION"] = new[] {
                "The Gaze Shader creates 'gazing' effects for 2D GIFs that automatically face the player's view. Includes built-in GIF converter and material instance manager.",
                "凝视着色器用于制作GIF自动朝向玩家的\"凝视\"效果。内置GIF转换器和材质实例管理器。",
                "凝視シェーダーは、2D GIFが自動的にプレイヤーの方を向く「凝視」効果を作成します。組み込みGIFコンバーターとマテリアルインスタンスマネージャーを含みます。"
            },

            // 资源部分
            ["GIF_CONVERSION"] = new[] { "GIF Conversion", "GIF转换", "GIF変換" },
            ["TEXTURE_ARRAY"] = new[] { "Texture Array", "纹理数组", "テクスチャ配列" },
            ["GIF_SOURCE_FILE"] = new[] { "GIF Source File", "GIF源文件", "GIFソースファイル" },
            ["SELECTED"] = new[] { "Selected: {0}", "已选择: {0}", "選択済み: {0}" },
            ["TEXTURE_SIZE"] = new[] { "Texture Size", "纹理尺寸", "テクスチャサイズ" },
            ["CONVERT_BUTTON"] = new[] { "Convert to Texture2DArray", "转换为Texture2DArray", "Texture2DArrayに変換" },
            ["NOT_GIF_WARNING"] = new[] { "Selected texture is not a GIF file", "选择的纹理不是GIF文件", "選択したテクスチャはGIFファイルではありません" },
            ["SELECT_GIF_PROMPT"] = new[] {
                "Select a GIF file to enable conversion, reduce frame count to prevent large Texture2DArray files",
                "选择GIF文件以启用转换，请自行缩减帧数，防止Texture2DArray文件过大",
                "変換を有効にするにはGIFファイルを選択してください。大きなTexture2DArrayファイルを防ぐためにフレーム数を減らしてください"
            },
            ["READY_STATUS"] = new[] {
                "Ready - Size: {0}×{1} | Frames: {2}",
                "就绪 - 尺寸: {0}×{1} | 帧数: {2}",
                "準備完了 - サイズ: {0}×{1} | フレーム数: {2}"
            },
            ["NO_ARRAY_WARNING"] = new[] { "No Texture2DArray assigned", "未分配纹理数组", "テクスチャ配列が割り当てられていません" },
            ["OPTIMIZE_MATERIALS"] = new[] { "Optimize Material Instances", "优化材质实例", "マテリアルインスタンスを最適化" },
            ["OPTIMIZE_TOOLTIP"] = new[] {
                "Open Material Manager to optimize performance by sharing material instances",
                "打开材质管理器，通过共享材质实例优化性能",
                "マテリアルマネージャーを開いて、マテリアルインスタンスを共有することでパフォーマンスを最適化"
            },

            // 效果部分
            ["ANIMATION_SETTINGS"] = new[] { "Animation Settings", "动画设置", "アニメーション設定" },
            ["GAZE_EFFECT"] = new[] { "Gaze Effect", "凝视效果", "凝視効果" },
            ["COLOR"] = new[] { "Color", "颜色", "色" },
            ["BASE_FPS"] = new[] { "Base FPS(Speed)", "基础帧率(速度)", "基本FPS(速度)" },
            ["PLAY_MODE"] = new[] { "Play Mode", "播放模式", "再生モード" },
            ["PLAY_MODE_LOOP"] = new[] { "Loop", "循环", "ループ" },
            ["PLAY_MODE_ONCE"] = new[] { "Once", "单次", "一度" },
            ["PLAY_MODE_RANDOM"] = new[] { "Random", "随机", "ランダム" },
            ["PLAY_MODE_MANUAL"] = new[] { "Manual", "手动", "手動" },
            ["MANUAL_FRAME"] = new[] { "Manual Frame", "手动帧", "手動フレーム" },
            ["START_FRAME_RANDOMIZATION"] = new[] { "Start Frame Randomization", "播放随机差异", "開始フレームランダム化" },
            ["START_FRAME_TOOLTIP"] = new[] {
                "When multiple meshes use same material: 0: Unified animation; 1: Maximized random playback difference",
                "多个网格使用同一个材质球时，0：统一的动画效果；1：最大化差异的随机播放效果",
                "複数のメッシュが同じマテリアルを使用する場合：0：統一されたアニメーション；1：最大化されたランダム再生の差異"
            },
            ["GAZE"] = new[] { "Gaze", "凝视", "凝視" },
            ["GAZE_TOOLTIP"] = new[] {
                "Animation plane faces view camera",
                "动画平面面向视角相机",
                "アニメーションプレーンがビューカメラを向く"
            },
            ["WEAKEN_DISTANCE_GAZE"] = new[] { "Weaken Distance Gaze", "弱化远方凝视效果", "距離による凝視効果の弱化" },

            // 距离速度控制    
            ["SPEED_CHANGE_MODE"] = new[] { "Proximity Speed Control", "靠近变速", "近接速度制御" },
            ["SPEED_CHANGE_RATE"] = new[] { "Speed Change Rate", "变化倍率", "変化倍率" },
            ["SPEED_MODE_UNIFORM"] = new[] { "Uniform", "匀速", "均一" },
            ["SPEED_MODE_ACCELERATE"] = new[] { "Accelerate", "加快", "加速" },
            ["SPEED_MODE_DECELERATE"] = new[] { "Decelerate", "减慢", "減速" },

            ["MAX_DISTANCE"] = new[] { "Max Distance", "最远距离", "最大距離" },
            ["MAX_DISTANCE_TOOLTIP"] = new[] {
                "Distance beyond which animation returns to normal speed",
                "超过此距离时动画恢复为正常速度",
                "この距離を超えるとアニメーションが通常速度に戻ります"
            },
            ["SPEED_CHANGE_MODE_TOOLTIP"] = new[] {
                "Animation playback speed changes with the distance between the camera and the object.",
                "动画播放速度会跟随视角与物体的距离发生变化。",
                "アニメーション再生速度は、カメラとオブジェクトの距離に応じて変化します。"
            },
                        ["SPEED_CHANGE_RATE_TOOLTIP"] = new[] {
                "Corresponds to how many times faster or slower the animation plays at close range, default is 1x.",
                "对应近距离时，加速几倍或者减慢几倍，默认为1倍。",
                "近距離でのアニメーション再生速度が何倍になるかを設定します。デフォルトは1倍です。"
            },
                        ["MAX_DISTANCE_TOOLTIP"] = new[] {
                "Animation playback speed returns to 1x when reaching the maximum distance, default is 15. For more noticeable distance speed effects, set the maximum distance to a smaller value.",
                "动画播放速度在到达最远距离后会变为1倍，默认最远距离为15。如果想要更明显的距离变速效果，请将最远距离调小。",
                "最大距離に達するとアニメーション再生速度が1倍に戻ります。デフォルトは15です。より顕著な距離速度効果を得るには、最大距離を小さく設定してください。"
            },

            ["SPEED_FROM_ZERO"] = new[] { "Speed From Zero", "速度从零开始", "速度をゼロから開始" },
            ["SPEED_FROM_ZERO_TOOLTIP"] = new[] {
                "When enabled: accelerate from 0 to set rate, decelerate from set rate to 0. When disabled: accelerate from 1x to set rate, decelerate from set rate to 1x.",
                "启用时：加速从0到设定倍率，减速从设定倍率到0。禁用时：加速从1倍到设定倍率，减速从设定倍率到1倍。",
                "有効時：0から設定倍率に加速、設定倍率から0に減速。無効時：1倍から設定倍率に加速、設定倍率から1倍に減速。"
            },

            ["SINGLE_AXIS_GAZE"] = new[] { "Single Axis Gaze", "单轴凝视", "単軸凝視" },
            ["SINGLE_AXIS_GAZE_TOOLTIP"] = new[] {
                "Restrict gaze effect to rotate only around a specific axis, default 360° follow gaze.",
                "限制凝视效果仅围绕某个轴旋转，默认360°跟随凝视。",
                "凝視効果を特定の軸周りのみの回転に制限し、デフォルトでは360°で凝視に追随します。"
            },
            ["SINGLE_AXIS_ALL"] = new[] { "All", "全", "全" },
            ["SINGLE_AXIS_X"] = new[] { "X", "X", "X" },
            ["SINGLE_AXIS_Y"] = new[] { "Y", "Y", "Y" },
            ["SINGLE_AXIS_Z"] = new[] { "Z", "Z", "Z" },

            // 变化部分
            ["FIXED_VARIATIONS"] = new[] { "Fixed Variations", "固定变化", "固定バリエーション" },
            ["RANDOM_VARIATIONS"] = new[] { "Random Variations", "随机变化", "ランダムバリエーション" },
            ["QUICK_ORIENTATION"] = new[] { "Quick Orientation", "快捷摆正", "クイック向き変更" },

            // 快捷摆正
            ["QUICK_ORIENTATION_TOOLTIP"] = new[] {
                "If the image disappears after enabling gaze, try selecting different options or adjust the extra rotation values to restore. Or quickly enable horizontal/vertical flip in the UV Adjust section below.",
                "如果开启凝视后，图像消失，请尝试选择不同选项或者调整以下额外旋转值尝试恢复。或者在下方\"UV调整\"中快捷打开水平/竖直翻转。",
                "凝視を有効にした後、画像が消える場合は、異なるオプションを選択するか、以下の追加回転値を調整して復元してください。または、下の「UV調整」で水平/垂直反転をすばやく有効にしてください。"
            },

            // 单轴凝视提示
            ["SINGLE_AXIS_GAZE_TOOLTIP"] = new[] {
                "Restrict gaze effect to rotate only around a specific axis, default 360° follow gaze.",
                "限制凝视效果仅围绕某个轴旋转，默认360°跟随凝视。",
                "凝視効果を特定の軸周りのみの回転に制限し、デフォルトでは360°で凝視に追随します。"
            },

            ["EXTRA_X_ROTATION"] = new[] { "Extra X Rotation", "额外X轴旋转", "追加X軸回転" },
            ["EXTRA_Y_ROTATION"] = new[] { "Extra Y Rotation", "额外Y轴旋转", "追加Y軸回転" },
            ["EXTRA_Z_ROTATION"] = new[] { "Extra Z Rotation", "额外Z轴旋转", "追加Z軸回転" },
            ["SCALE_VARIATION"] = new[] { "Scale Variation", "缩放差异", "スケール差異" },
            ["SCALE_VARIATION_TOOLTIP"] = new[] {
                "1: All objects same size\n1.5: Moderate variation (0.75x to 1.25x)\n2: Maximum variation (0.5x to 2x)",
                "1: 所有物体大小相同\n1.5: 中等变化 (0.75倍到1.25倍)\n2: 最大变化 (0.5倍到2倍)",
                "1: すべてのオブジェクトが同じサイズ\n1.5: 中程度の変化 (0.75倍から1.25倍)\n2: 最大の変化 (0.5倍から2倍)"
            },
            ["RANDOM_X_ROTATION"] = new[] { "Random X Rotation Variation", "随机X轴旋转差异", "ランダムX軸回転差異" },
            ["RANDOM_X_ROTATION_TOOLTIP"] = new[] {
                "0: All objects have same X rotation\n1: Objects have random X rotation based on base X rotation",
                "0: 所有物体X轴旋转相同\n1: 物体基于基础X轴旋转值随机变化",
                "0: すべてのオブジェクトが同じX軸回転\n1: オブジェクトが基本X軸回転値に基づいてランダムに変化"
            },
            ["RANDOM_Y_ROTATION"] = new[] { "Random Y Rotation Variation", "随机Y轴旋转差异", "ランダムY軸回転差異" },
            ["RANDOM_Y_ROTATION_TOOLTIP"] = new[] {
                "0: All objects have same Y rotation\n1: Objects have random Y rotation based on base Y rotation",
                "0: 所有物体Y轴旋转相同\n1: 物体基于基础Y轴旋转值随机变化",
                "0: すべてのオブジェクトが同じY軸回転\n1: オブジェクトが基本Y軸回転値に基づいてランダムに変化"
            },
            ["RANDOM_Z_ROTATION"] = new[] { "Random Z Rotation Variation", "随机Z轴旋转差异", "ランダムZ軸回転差異" },
            ["RANDOM_Z_ROTATION_TOOLTIP"] = new[] {
                "0: All objects have same Z rotation\n1: Objects have random Z rotation based on base Z rotation",
                "0: 所有物体Z轴旋转相同\n1: 物体基于基础Y轴旋转值随机变化",
                "0: すべてのオブジェクトが同じZ軸回転\n1: オブジェクトが基本Z軸回転値に基づいてランダムに変化"
            },

            // UV高级设置
            ["UV_ADJUST"] = new[] { "UV Adjust", "UV调整", "UV調整" },
            ["DISPLAY_FIX"] = new[] { "Display Fix", "显示修复", "表示修正" },
            ["UV_X_OFFSET"] = new[] { "UV X Offset", "UV X偏移", "UV Xオフセット" },
            ["UV_Y_OFFSET"] = new[] { "UV Y Offset", "UV Y偏移", "UV Yオフセット" },
            ["X_SCALE"] = new[] { "X Scale", "X缩放", "Xスケール" },
            ["Y_SCALE"] = new[] { "Y Scale", "Y缩放", "Yスケール" },

            // 光影效果区域
            ["LIGHTING_EFFECT"] = new[] { "Lighting Effect", "光影效果", "ライティング効果" },
            ["LIGHTING_EFFECT_TOOLTIP"] = new[] {
                "Enable lighting effects to make the shader respond to scene lighting",
                "启用光影效果使着色器响应场景光照",
                "シェーダーがシーンライティングに応答するようにライティング効果を有効にする"
            },
            ["LIGHTING_EFFECT_SECTION"] = new[] {
                "Lighting Effect",
                "光影效果",
                "ライティング効果"
            },
                        ["LIGHTING_EFFECT_DISABLED_INFO"] = new[] {
                "Enable lighting effect to unlock normal mapping and advanced lighting features",
                "启用光影效果以解锁法线贴图和高级光照功能",
                "ライティング効果を有効にすると、法線マッピングと高度な照明機能が利用可能になります"
            },

            // 翻转选项
            ["FLIP_HORIZONTAL"] = new[] { "Flip Horizontal", "水平翻转", "水平反転" },
            ["FLIP_VERTICAL"] = new[] { "Flip Vertical", "竖直翻转", "垂直反転" },

            ["FIX_ARTIFACTS"] = new[] { "Fix Artifacts", "修复伪影", "アーティファクト修正" },
            ["FIX_ARTIFACTS_TOOLTIP"] = new[] {
                "Fix depth sorting issues with transparent objects",
                "修复透明物体的深度排序问题",
                "透明オブジェクトの深度ソート問題を修正"
            },

            ["BACKFACE_CULLING"] = new[] { "Backface Culling", "背面剔除", "バックフェイスカリング" },
            ["BACKFACE_CULLING_TOOLTIP"] = new[] {
                "Enable backface culling to hide triangles facing away from camera",
                "启用背面剔除以隐藏背对摄像机的三角形",
                "カメラから遠ざかる三角形を非表示にするバックフェイスカリングを有効にする"
            },
            

            // VRC Light Volumes 相关文本
            ["USE_LIGHT_VOLUME"] = new[] {
                "Use Light Volume",
                "使用VRC Light Volumes(光体积)",
                "VRC Light Volumes使用"
            },
            ["USE_LIGHT_VOLUME_TOOLTIP"] = new[] {
                "Enable VRC Light Volumes lighting system",
                "启用 VRC Light Volumes 光照系统",
                "VRC Light Volumes照明システムを有効にする"
            },
            ["LIGHT_VOLUME_INTENSITY"] = new[] {
                "Light Volume Intensity",
                "光体积强度",
                "光ボリューム強度"
            },
            ["LIGHT_VOLUME_INTENSITY_TOOLTIP"] = new[] {
                "Control the intensity of light volume illumination",
                "控制光体积光照的强度",
                "光ボリューム照明の強度を制御する"
            },
            ["LIGHT_VOLUME_SECTION"] = new[] {
                "VRC Light Volumes",
                "VRC光体积",
                "VRC光ボリューム"
            },
            ["LIGHT_VOLUME_ENABLED_INFO"] = new[] {
                "VRC Light Volumes is enabled. The shader will use voxel lighting data from the scene. If you need lighting changes for GIFs with transparency variations, please do not set the GameObject to Static during baking to avoid fixed shadows.",
                "VRC Light Volumes 已启用。着色器将使用场景中的体素光照数据。如果需要透明度变化的 GIF 光影变化效果，烘焙时请不要设置 GameObject 为静态，这样就不会产生固定阴影。",
                "VRC Light Volumes が有効です。シェーダーはシーンからのボクセル照明データを使用します。透明度が変化する GIF のライティング変化が必要な場合は、固定の影が発生しないよう、ベイク時に GameObject を Static に設定しないでください。"
            },
            ["LIGHT_VOLUME_DISABLED_WARNING"] = new[] {
                "VRC Light Volumes is disabled. The shader will use basic ambient lighting. If you need lighting changes for GIFs with transparency variations, please do not set the GameObject to Static during baking to avoid fixed shadows.",
                "VRC Light Volumes 已禁用。着色器将使用基础环境光照。如果需要透明度变化的 GIF 光影变化效果，烘焙时请不要设置 GameObject 为静态，这样就不会产生固定阴影。",
                "VRC Light Volumes が無効です。シェーダーは基本環境光を使用します。透明度が変化する GIF のライティング変化が必要な場合は、固定の影が発生しないよう、ベイク時に GameObject を Static に設定しないでください。"
            },

            // 材质管理器
            ["MATERIAL_MANAGER_TITLE"] = new[] { "Material Instance Manager", "材质实例管理器", "マテリアルインスタンスマネージャー" },
            ["MATERIAL_MANAGER_DESC"] = new[] {
                "Automatically optimizes performance by sharing material instances. Reduces draw calls and memory usage.",
                "通过共享材质实例自动优化性能。减少绘制调用和内存使用。",
                "マテリアルインスタンスを共有することで自動的にパフォーマンスを最適化します。描画コールとメモリ使用量を削減します。"
            },
            ["QUICK_ACTIONS"] = new[] { "Quick Actions", "快速操作", "クイックアクション" },
            ["AUTO_OPTIMIZE_ALL"] = new[] { "Auto Optimize All", "自动优化全部", "すべてを自動最適化" },
            ["REFRESH_SCAN"] = new[] { "Refresh Scan", "重新扫描", "再スキャン" },
            ["MATERIAL_GROUPS"] = new[] { "Material Groups (by Texture Array):", "材质分组 (按纹理数组):", "マテリアルグループ (テクスチャ配列別):" },
            ["NO_MATERIALS_WARNING"] = new[] { "No materials using Gaze Shader found", "未找到使用Gaze Shader的材质", "Gazeシェーダーを使用するマテリアルが見つかりません" },
            ["TEXTURE_ARRAY_INFO"] = new[] { "Texture Array: {0} ({1} frames)", "纹理数组: {0} ({1} 帧)", "テクスチャ配列: {0} ({1} フレーム)" },
            ["NO_TEXTURE_ARRAY"] = new[] { "No Texture Array", "无纹理数组", "テクスチャ配列なし" },
            ["STATUS"] = new[] { "Status: {0}/{1} shared instances", "状态: {0}/{1} 个共享实例", "ステータス: {0}/{1} 共有インスタンス" },
            ["SHARED_INSTANCES"] = new[] { "shared instances", "个共享实例", "共有インスタンス" },
            ["SELECT"] = new[] { "Select", "选择", "選択" },
            ["APPLY"] = new[] { "Apply", "应用", "適用" },
            ["CREATE_SHARED_INSTANCE"] = new[] { "Create Shared Instance", "创建共享实例", "共有インスタンスを作成" },
            ["OPTIMIZE_SCENE"] = new[] { "Optimize Scene", "优化场景", "シーンを最適化" },
            ["ADVANCED_ACTIONS"] = new[] { "Advanced Actions:", "高级操作:", "高度な操作:" },
            ["CREATE_ALL_SHARED"] = new[] { "Create All Shared Instances", "创建所有共享实例", "すべての共有インスタンスを作成" },
            ["CLEANUP_UNUSED"] = new[] { "Cleanup Unused Materials", "清理未使用材质", "未使用マテリアルをクリーンアップ" },
            ["HOW_TO_USE"] = new[] { "How to Use:", "使用说明:", "使用方法:" },
            ["INSTRUCTIONS"] = new[] {
                "1. Click 'Auto Optimize All' to automatically share materials with same texture array\n" +
                "2. Green status means material is shared, red means independent\n" +
                "3. Use 'Apply' to replace scene objects with shared material\n" +
                "4. Enable auto-replacement to automatically update scene references",
                "1. 点击“自动优化全部”自动共享使用相同纹理数组的材质\n" +
                "2. [SHARED]状态表示材质已共享，[INDEPENDENT]表示独立\n" +
                "3. 使用“应用”将场景中的物体替换为共享材质\n" +
                "4. 启用自动替换以自动更新场景引用",
                "1. 「すべてを自動最適化」をクリックして、同じテクスチャ配列を使用するマテリアルを自動共有\n" +
                "2. 緑のステータスはマテリアルが共有されていることを示し、赤は独立していることを示す\n" +
                "3. 「適用」を使用してシーンオブジェクトを共有マテリアルに置き換える\n" +
                "4. 自動置換を有効にしてシーン参照を自動更新"
            },
            ["ENABLE_AUTO_REPLACE"] = new[] { "Enable Auto Material Replacement", "启用自动材质替换", "自動マテリアル置換を有効化" },
            ["SUCCESS"] = new[] { "Success", "成功", "成功" },
            ["ERROR"] = new[] { "Error", "错误", "エラー" },
            ["WARNING"] = new[] { "Warning", "警告", "警告" },
            ["COMPLETE"] = new[] { "Complete", "完成", "完了" },

            // 转换器
            ["CONVERTING"] = new[] { "Converting GIF", "转换GIF", "GIF変換中" },
            ["PROCESSING"] = new[] { "Processing...", "处理中...", "処理中..." },
            ["CONVERSION_SUCCESS"] = new[] {
                "GIF successfully converted to Texture2DArray!\n\nSize: {0}×{1} | Frames: {2}\nSaved to: {3}",
                "GIF成功转换为Texture2DArray！\n\n尺寸: {0}×{1} | 帧数: {2}\n保存为: {3}",
                "GIFがTexture2DArrayに正常に変換されました！\n\nサイズ: {0}×{1} | フレーム数: {2}\n保存先: {3}"
            },
            ["CONVERSION_ERROR"] = new[] { "Conversion failed: {0}", "转换失败: {0}", "変換失敗: {0}" },
            ["SELECT_GIF_ERROR"] = new[] { "Please select a valid GIF file first!", "请先选择有效的GIF文件！", "まず有効なGIFファイルを選択してください！" },
            ["NOT_GIF_ERROR"] = new[] { "Selected texture is not a GIF file!", "选择的纹理不是GIF文件！", "選択したテクスチャはGIFファイルではありません！" },

            // 作者信息
            ["BOOTH_TOOLTIP"] = new[] { "Booth Store", "Booth商店", "Boothストア" },
            ["TWITTER_TOOLTIP"] = new[] { "Twitter / X", "推特 / X", "Twitter / X" },
            ["BILIBILI_TOOLTIP"] = new[] { "Bilibili", "哔哩哔哩", "ビリビリ" },

            //法线
            ["NORMAL_MAPPING"] = new[] { "Normal Mapping", "法线贴图", "法線マッピング" },
            ["USE_NORMAL_MAP"] = new[] { "Use Normal Map", "使用法线贴图", "法線マップを使用" },
            
            ["USE_NORMAL_MAP_TOOLTIP"] = new[] {
                "Enable normal mapping to add 3D depth and lighting effects to the animation",
                "启用法线贴图为动画添加3D深度和光照效果",
                "法線マッピングを有効にして、アニメーションに3Dの深さとライティング効果を追加"
            },
            ["NORMAL_STRENGTH"] = new[] { "Normal Strength", "法线强度", "法線強度" },
            ["NORMAL_STRENGTH_TOOLTIP"] = new[] {
                "Controls the intensity of normal mapping effect. 1: Normal, -1: Inverted, 0: No effect",
                "控制法线贴图效果的强度。1: 正常, -1: 反转, 0: 无效果",
                "法線マッピング効果の強度を制御します。1: 通常, -1: 反転, 0: 効果なし"
            },
            
            ["GENERATE_NORMAL_MAP"] = new[] { "Generate Normal Map", "生成法线贴图", "法線マップを生成" },
            ["GENERATE_NORMAL_MAP_TOOLTIP"] = new[] {
                "Generate normal map from the current texture array to add 3D lighting effects",
                "从当前纹理数组生成法线贴图以添加3D光照效果",
                "現在のテクスチャ配列から法線マップを生成して3Dライティング効果を追加"
            },
            ["BRIGHTNESS"] = new[] { "Brightness", "亮度", "明るさ" },
            ["BRIGHTNESS_TOOLTIP"] = new[] { "Controls the base brightness. 0 is pure black, 1 is normal, >1 makes it very bright.", "控制暗处的底色亮度。0为纯黑，1为正常，超过1会非常亮。", "暗い領域の基本の明るさを制御します。0は真っ黒、1は通常、1以上は非常に明るくなります。" },
            
            ["SPECULAR_SHARPNESS"] = new[] { "Specular Sharpness", "高光锐度", "スペキュラ鋭さ" },
            ["SPECULAR_SHARPNESS_TOOLTIP"] = new[] { "Controls how sharp/focused the highlight is. Higher = smaller and sharper.", "控制高光反射的锐利程度（集中度）。数值越大，高光点越小越锐利。", "スペキュラハイライトの鋭さを制御します。値が大きいほど、ハイライトが小さく鋭くなります。" },
            
            ["SPECULAR_BRIGHTNESS"] = new[] { "Specular Brightness", "高光亮度", "スペキュラ明るさ" },
            ["SPECULAR_BRIGHTNESS_TOOLTIP"] = new[] { "Controls how bright/strong the highlight is.", "控制高光反射的整体亮度和强度。", "スペキュラハイライトの明るさを制御します。" },

            // === 精灵图专属词条 ===
            ["GENERATE_SPRITE_SHEET"] = new[] { "Generate Sprite Sheet", "生成精灵图(Sprite Sheet)", "スプライトシートを生成(Sprite Sheet)" },
            ["GENERATION_MODE"] = new[] { "Generation Mode", "生成模式", "生成モード" },
            ["PERFORMANCE_MODE"] = new[] { "Performance (Max 2048)", "注重性能 (Max 2048)", "パフォーマンス重視 (最大 2048)" },
            ["QUALITY_MODE"] = new[] { "Quality (Max 4096)", "注重画质 (Max 4096)", "画質重視 (最大 4096)" },
            ["SPRITE_SHEET_TEXTURE"] = new[] { "Sprite Sheet Texture", "精灵图纹理", "スプライトシートテクスチャ" },
            ["COLUMNS"] = new[] { "Columns", "列数", "列数" },
            ["ROWS"] = new[] { "Rows", "行数", "行数" },
            ["TOTAL_FRAMES"] = new[] { "Total Frames", "总帧数", "総フレーム数" },
            ["NO_SPRITE_SHEET_WARNING"] = new[] { "No Sprite Sheet assigned.", "未分配精灵图纹理。", "スプライトシートが割り当てられていません。" },
            ["SPRITE_SHEET_TITLE"] = new[] { "Sprite Sheet", "精灵图", "スプライトシート" },
            ["SPRITE_SHEET_READY"] = new[] { 
                "Ready - Size: {0}x{1} | Frames: {2} | Grid: {3} Cols x {4} Rows", 
                "就绪 - 尺寸: {0}x{1} | 帧数: {2} | 网格: {3}列 x {4}行", 
                "準備完了 - サイズ: {0}x{1} | フレーム: {2} | グリッド: {3}列 x {4}行" 
            },

            // 转换器内部的进度条和提示
            ["ANALYZING_GIF"] = new[] { "Analyzing GIF frames...", "正在分析 GIF 帧...", "GIFフレームを分析中..." },
            ["PACKING_FRAMES"] = new[] { "Packing frame {0}/{1}...", "正在拼合第 {0}/{1} 帧...", "フレーム {0}/{1} をパック中..." },
            ["SPRITE_SHEET_SUCCESS_TITLE"] = new[] { "Conversion Successful", "转换成功", "変換成功" },
            ["SPRITE_SHEET_SUCCESS_DESC"] = new[] { 
                "Sprite Sheet generated: {0}\n\nMode: {1}\nFinal Size: {2} × {3}\nGrid: {4} Cols × {5} Rows", 
                "已生成精灵图: {0}\n\n模式: {1}\n最终大图尺寸: {2} × {3}\n网格布局: {4}列 × {5}行", 
                "スプライトシートが生成されました: {0}\n\nモード: {1}\n最終サイズ: {2} × {3}\nグリッド: {4} 列 × {5} 行" 
            },
            ["QUALITY_TEXT_HIGH"] = new[] { "Quality (Max 4K)", "高画质 (Max 4K)", "高画質 (最大 4K)" },
            ["QUALITY_TEXT_LOW"] = new[] { "Performance (Max 2K)", "高性能 (Max 2K)", "パフォーマンス (最大 2K)" },
        
            // 自动随机处理功能词条
            ["AUTO_DESYNC_BUTTON"] = new[] { "Randomize Multi-Objects", "多物品随机化处理", "複数オブジェクトのランダム化" },
            ["AUTO_DESYNC_TOOLTIP"] = new[] { 
                "When multiple objects share the same material instance, click this to generate completely independent random animation effects (such as the [Random] Play Mode or enabling [Start Frame Randomization]). \nThe system will automatically assign extremely subtle scale variations to grant them exclusive random seeds at the physical level.", 
                "当多个对象共用同一材质实例时，若需产生独立的随机动画效果（包括上方【播放模式】的【随机】模式及开启【播放随机差异】）。\n请点击此按钮。系统将自动为其分配极其微弱的不可见缩放差异，从而在物理层面赋予专属的随机种子。", 
                "複数のオブジェクトが同じマテリアルインスタンスを共有している場合、このボタンをクリックすると、完全に独立したランダムアニメーション効果（上の[再生モード]の[ランダム]や[開始フレームランダム化]など）を生成できます。\n極微小なスケール差異を割り当て、物理レベルで専用のランダムシードを付与します。" 
            },
            ["AUTO_DESYNC_SUCCESS_TITLE"] = new[] { "Process Complete", "处理完成", "処理完了" },
            ["AUTO_DESYNC_SUCCESS_DESC"] = new[] { 
                "Successfully assigned imperceptible scale variations (0.00001 level true randomness) to {0} objects.\n\nThey now share the same material (1 DrawCall) while perfectly displaying independent random playback effects, without any flickering when moved!", 
                "已成功为 {0} 个物体分配了极限隐形的缩放差异值（0.00001级真随机）。\n\n现在它们既能共用同一个材质球（1个DrawCall），又能完美呈现出独立的随机播放效果，且在移动时绝对不会产生任何频闪！", 
                "{0} 個のオブジェクトに極小のスケール差異（0.00001レベルの真のランダム）を割り当てました。\n\nこれにより、同じマテリアルを共有（1 DrawCall）しながら、完全に独立したランダム再生効果を表示し、移動時のちらつきも発生しません！" 
            },
            ["AUTO_DESYNC_NO_OBJECTS"] = new[] { "No objects found using the current material in the scene.", "场景里没有找到使用当前材质球的物体。", "現在のマテリアルを使用しているオブジェクトがシーンに見つかりませんでした。" },
            ["PROMPT"] = new[] { "Notice", "提示", "通知" },
            ["OK"] = new[] { "OK", "确定", "確認" },
        };
        }
}