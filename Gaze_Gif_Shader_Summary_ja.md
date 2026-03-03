# Gaze Shader - 完全説明書・ユーザーマニュアル（日本語）

**作成者**: @咸鱼子Luna  
**プロジェクト**: Gaze Gif Shader 凝視追跡システム  
**バージョン**: 1.0  
**最終更新**: 2026-03-03

---

## 📖 目次

1. [プロジェクト概要](#プロジェクト概要)
2. [機能](#機能)
3. [シェーダー種類比較](#シェーダー種類比較)
4. [クイックスタート](#クイックスタート)
5. [パラメータ解説](#パラメータ解説)
6. [エディタツール](#エディタツール)
7. [ワークフロー](#ワークフロー)
8. [VRC連携](#vrc連携)
9. [FAQ](#faq)
10. [技術的詳細](#技術的詳細)

---

## プロジェクト概要

**Gaze Shader** は Unity と VRChat 向けに開発された高度なアニメーションレンダリングシステムで、**凝視追跡型GIFアニメーション**の表示を目的としています。機能は以下のとおりです：

- 🎬 GIFをリアルタイム再生し、フレームレートを自在に制御
- 👁️ カメラに常に面するようオブジェクトを回転（ビルボード効果）
- 📏 カメラ距離に応じて再生速度を変更
- 💡 法線マップや鏡面反射、VRCライトボリューム対応
- 🎭 ループ／一回／ランダム／手動の再生モード
- 🔀 縮尺・回転のランダム化オプション

---

## 機能

### コア機能

| 機能 | 説明 |
|------|------|
| **GIF再生** | GIFからフレームを抽出し順次描画 |
| **凝視追跡** | 常にカメラ方向を向く |
| **距離速度制御** | 視聴者距離により再生速度変化 |
| **再生モード** | ループ/一回/ランダム/手動 |
| **法線マッピング** | 法線マップ生成・利用でハイライト |
| **ライトボリューム** | VRChatのLight Volumeに対応 |

### 上級オプション

| オプション | 説明 |
|------------|------|
| **開始フレームランダム化** | オブジェクトごとにランダム開始 |
| **回転バリエーション** | X/Y/Z軸にランダム回転 |
| **スケールバリエーション** | 大きさのランダム化 |
| **軸限定凝視** | 任意軸のみ追跡 possible |
| **距離フェード** | 遠距離で凝視効果を弱める |
| **アルファ修正** | 透明度アーティファクトを補正 |

---

## シェーダー種類比較

### 1️⃣ **Gaze Gif** (`@Luna/Gaze Gif`)

- **テクスチャ形式**: Texture2DArray（推奨）
- **用途**: メモリに余裕があり、フレーム数を大量に扱う時
- **利点**: 最高の性能、無制限のフレーム、1フレームあたり低メモリ
- **欠点**: GIFを配列に変換する必要があり、すべてのプラットフォームで未対応
- **推奨**: デスクトップ/VRアプリ、高品質VRChatワールド

### 2️⃣ **Gaze GIF SpriteSheet** (`@Luna/Gaze GIF SpriteSheet`)

- **テクスチャ形式**: 2Dスプライトシート格子
- **用途**: 汎用・最高の互換性
- **利点**: どこでも動作、変換が簡単、行列調整可能
- **欠点**: 大きな単一テクスチャはメモリ消費が多く、性能は少し劣る
- **推奨**: VRChatワールド、Quest、一般制作

### 3️⃣ **Gaze GIF SpriteSheet Cutout** (`@Luna/Gaze GIF SpriteSheet Cutout`)

- **テクスチャ形式**: 2Dスプライトシート（不透明のみ）
- **用途**: 完全不透明GIFで最高性能を求める場合
- **利点**: 最速性能、深度書き込み、アルファブレンド不要
- **欠点**: 半透明画像を扱えない
- **推奨**: 不透明GIFアニメ、モバイル/Quest

---

## クイックスタート

### ステップ 1: GIF準備

1. GIFファイルを用意（透過対応）。
2. `Assets/` フォルダに配置。
3. インスペクタでインポートタイプを **Texture** に設定。

### ステップ 2: マテリアル作成

#### 方法A – SpriteSheet（初心者向け）

1. 右クリック → Create → Material。
2. シェーダーに `@Luna/Gaze GIF SpriteSheet` を選択。
3. インスペクタで：
   - GIFを "GIF Source" にドラッグ。
   - **Generate Sprite Sheet** ボタンをクリック。
   - Shaderが MainTex、Columns、Rows、TotalFrames を自動設定。

#### 方法B – TextureArray（上級者向け）

1. 同様にマテリアルを作成。
2. `@Luna/Gaze Gif` シェーダーを選択。
3. ツールメニューから *GIF to Texture Array* を選択。
   - 解像度を指定して生成。

### ステップ 3: パラメータ設定

```
基本設定:
├─ FPS: アニメレート（通常10‑24）
├─ Play Mode: Loop/Once/Random/Manual
└─ Color: ティント色

凝視:
├─ Gaze: 追跡ON/OFF
├─ Single Axis Gaze: 軸指定
└─ Weaken Distance Gaze: 距離で弱める

距離速度:
├─ Speed Change Mode
├─ Speed Change Rate
└─ Max Distance
```

### ステップ 4: シーンに適用

1. Quad、Plane、任意のメッシュを作成。
2. レンダラーにマテリアルをアサイン。
3. プレイして確認。

---

## パラメータ解説

### 📦 テクスチャ類

#### Gaze Gif.shader

```
_Textures (Texture Array)
  ├─ 説明: GIFから変換された配列
  ├─ デフォルト: black
  └─ 用途: アニメフレームを格納

_NormalMapArray (2DArray, オプション)
  ├─ 説明: 法線マップ配列
  ├─ デフォルト: bump
  └─ 用途: 鏡面反射効果
```

#### Gaze GIF SpriteSheet.shader

```
_MainTex (2D)
  ├─ 説明: スプライトシート画像
  ├─ デフォルト: white
  └─ 用途: フレームを格納

_Columns & _Rows (IntRange)
  ├─ 説明: 格子の列数・行数
  ├─ 範囲: 1‑64
  └─ 例: 4列×4行=16フレーム

_TotalFrames (IntRange)
  ├─ 説明: 有効フレーム数
  ├─ 範囲: 1‑4096
  └─ 行×列より少なく設定可
```

### 🎬 アニメ制御

```
_fps (IntRange, デフォルト12)
  ├─ 範囲: 1‑60
  └─ フレームレート

_PlayMode (Enum, デフォルトLoop)
  ├─ Loop (0)
  ├─ Once (1)
  ├─ Random (2)
  └─ Manual (3)

_ManualFrame (IntRange, デフォルト1)
  ├─ 範囲: 1‑100
  ├─ PlayMode=Manual時に使用
  └─ 1が最初のフレーム

_StartFrameRandomization (Float, デフォルト0)
  ├─ 範囲: 0‑1
  └─ オブジェクトごとの開始オフセット
```

### 📏 凝視制御

```
_Gaze (Toggle, デフォルトON)
  ├─ 有効でカメラ方向追跡
  └─ 無効で元の向きを維持

_SingleAxisGaze (Enum, デフォルトAll)
  ├─ All (0): 全軸追跡
  ├─ X (1)
  ├─ Y (2)
  └─ Z (3)

_WeakenDistanceGaze (Float, デフォルト0)
  ├─ 範囲: 0‑1
  └─ 距離に応じて効果を弱める

_ExtraRotX/Y/Z (Float, デフォルト0)
  ├─ 範囲: ‑180〜180°
  └─ 追加回転オフセット
```

### 📉 距離速度制御

```
_SpeedChangeMode (Enum, デフォルトUniform)
  ├─ Uniform (0): 距離影響なし
  ├─ Accelerate (1): 近距離ほど速く
  └─ Decelerate (2): 近距離ほど遅く

_SpeedChangeRate (Float, デフォルト1)
  ├─ 範囲: 1‑10
  └─ 影響強度

_MaxDistance (Float, デフォルト15)
  ├─ 範囲: 5‑50m
  └─ 参照距離

_SpeedFromZero (Toggle, デフォルトOFF)
  ├─ ON: 近距離で速度0可
  └─ OFF: 最低FPS維持
```

### 🎨 色・スケール

```
_Color (Color, デフォルトWhite)
  ├─ テクスチャの色味
  └─ Alphaは透明度調整

_ScaleX / _ScaleY (Float, デフォルト1)
  ├─ UVスケール
  └─ 表示範囲の拡大縮小

_UVx / _UVy (Float, デフォルト0)
  ├─ UVオフセット
  └─ テクスチャの位置移動

_Brightness (Float, デフォルト1.0)
  ├─ 範囲: 0‑5
  └─ 全体輝度調整
```

### 🎭 ランダム変化

```
_ScaleVariation (Float, デフォルト1)
  ├─ 範囲: 1‑2
  └─ ランダムスケール倍率

_RandomRotXVariation (Float, デフォルト0)
  ├─ 範囲: 0‑1
  └─ X軸ランダム回転

_RandomRotYVariation (Float, デフォルト0)
  └─ Y軸ランダム回転

_RandomRotZVariation (Float, デフォルト0)
  └─ Z軸ランダム回転
```

### 💡 ライティング

```
_LightingEffect (Toggle, デフォルトON)
  ├─ ライティング計算を有効
  └─ 無効でテクスチャのみ表示

_Brightness (Float, デフォルト1.0)
  ├─ 範囲: 0‑5
  └─ 光の明るさ調整

_UseNormalMap (Toggle, デフォルトOFF)
  ├─ 法線マップを使用
  └─ _NormalMapArrayまたは_NormalMapが必要

_NormalStrength (Float, デフォルト1)
  ├─ 範囲: ‑5〜5
  └─ 法線強度

_SpecularSharpness (Float, デフォルト20)
  ├─ 範囲: 1‑100
  └─ 鏡面反射の鋭さ

_SpecularBrightness (Float, デフォルト0.5)
  ├─ 範囲: 0‑1
  └─ 鏡面反射の明るさ
```

### 🌈 VRC機能

```
_UseLightVolume (Toggle, デフォルトON)
  ├─ VRChat Light Volume対応
  └─ ライトボリュームの影響を受ける

_LightVolumeIntensity (Float, デフォルト1.0)
  ├─ 範囲: 0‑2
  └─ 影響の強さ
```

### ☑️ 表示修正

```
_BackfaceCulling (Toggle, デフォルトON)
  ├─ バックフェイスカリングを有効
  └─ 無効で裏面を表示

_FixTransp (Toggle, デフォルトOFF)
  ├─ 透明度アーティファクトを修正
  └─ エッジがちらつく場合にON
```

---

## エディタツール

### 🎛️ Gaze Gif Shader GUI

シェーダー用のカスタムインスペクター。

#### 主なセクション

1. **作者情報** – ソーシャルリンク（Booth/X/Bilibili）
2. **ヘッダー** – Material Managerボタン、シェーダー切り替え
3. **リソース**
   - SpriteSheet版: [Generate Sprite Sheet] ボタン
   - TextureArray版: 変換用ツールメニュー
4. **エフェクト** – 再生モード、FPS、ランダム開始、速度設定
5. **固定変化** – 回転、スケール、UV等
6. **ランダム変化** – スケール＋回転のランダム化
7. **詳細** – 法線マップ、鏡面、カリングオプション

---

### 📁 Material Instance Manager

`Tools → @Luna → Gaze Shader Material Manager` から開く。

#### 機能

```
左パネル：
├─ 言語選択 (EN/CN/JA)
├─ クイック操作
│  ├─ [Auto Optimize All]
│  └─ [Refresh Scan]
└─ テクスチャ別／nullのマテリアル表示

右パネル操作：
├─ [Create Shared Instance]
├─ [Optimize Scene]
└─ [Cleanup Unused]
```

#### ワークフロー

1. マネージャを開く
2. [Refresh Scan] をクリック
3. グループと提案を確認
4. 自動または手動で最適化
5. 終了

---

### 🎨 Normal Map Generator

法線マップを自動生成。

#### 使い方

1. マテリアルで `_UseNormalMap` をONに。
2. インスペクタで **[Generate Normal Map]** をクリック。
3. パラメータを調整：
   - `_NormalStrength` (‑5〜5)
   - `_SpecularSharpness` (1〜100)
   - `_SpecularBrightness` (0〜1)

#### 原理

隣接ピクセルの明暗差を計算し、凹凸法線を生成。バンプマッピングとスペキュラ反射に使用。

---

### 🔄 GIF変換ツール

#### GIF to SpriteSheet Converter

- 高速で全プラットフォーム対応。
- 2つの品質モード：
  - 高性能: 最大2048×2048
  - 高品質: 最大4096×4096

**手順**:
1. マテリアルでGIFを選択
2. [Generate Sprite Sheet] をクリック
3. 品質モードを選択
4. 最適な格子を計算しアトラスを生成

| モード | 最大サイズ | 性能 | 用途 |
|--------|------------|------|------|
| 高性能 | 2048² | 最速 | モバイル/VRChat大規模 |
| 高品質 | 4096² | 良好 | デスクトップ/ハイエンド |

#### GIF to Texture Array Converter

- 最高性能、フレーム無制限
- 対応は限定的

**手順**:
1. GIFをマテリアルにドラッグ
2. 解像度を選択 (256–2048)
3. [Convert] をクリック
4. Texture2DArray が生成される

解像度ガイド:
```
256: 超小・高性能
512: 推奨バランス
1024: 高品質
2048: 最高品質
```

---

## ワークフロー

### ワークフロー1 – プロトタイピング

1. GIFを準備 (<256²推奨)
2. SpriteSheetマテリアルを作成
3. GIFをシートに変換
4. FPS=12‑24、カラー、Gaze=ON設定
5. Quadに適用してプレビュー

### ワークフロー2 – VRChat最適化

1. リソース策定と分類
2. 高性能モードで一括変換 (<4096)
3. Material Managerで共有マテリアル作成
4. `_UseLightVolume` を有効にし強度調整
5. Quest上でテストしDrawCall/メモリを監視

### ワークフロー3 – 高度なビジュアル

1. SpriteSheetから開始
2. 凝視パラメータと追加回転を微調整
3. 法線マップを有効にし強度設定
4. 鏡面設定を構成
5. 距離速度制御を調整
6. ランダム変化を追加し複製

---

## VRC連携

### Light Volume対応

VRChatのLight Volumeは特定領域の光を追加するツール。シェーダーは自動的に反応。

### UdonSharpスクリプト

ファイル: `UdonGazeGifTrigger.cs`

アニメ開始時間をプレイヤー間で同期させる。

**使用法**:
1. Gazeマテリアルを持つオブジェクトにスクリプトを付与
2. OnEnableで `_StartTime` を `Time.timeSinceLevelLoad` に設定
3. シェーダーが現在フレームを計算

例:
```csharp
private void OnEnable() {
    meshRenderer = GetComponent<Renderer>();
    if(meshRenderer?.material != null)
        meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
}
```

### VRChatでのベストプラクティス

```
- ネットワーク更新を最小限に; AnimatorControllerを利用
- シーン内のGaze Shaderオブジェクトは50未満に
- クロスプラットフォーム互換性のためSpriteSheet優先
- 遠距離ではLODを有効化し解像度低下
- QuestおよびPCでテスト
- 光volumeを有効にして実環境照明
- 法線マップはハイエンドデバイスのみ
- シーンスケールに応じて MaxDistance 設定
```

---

## FAQ

### Q1: どのシェーダーを選べばよい？

**A**: 優先順位は以下の通りです：

1. **SpriteSheet** (`@Luna/Gaze GIF SpriteSheet`)
   - ほとんどのケースで互換性抜群
2. **TextureArray** (`@Luna/Gaze Gif`)
   - 大量フレームや最高性能が必要な場合
   - 対応プラットフォームか確認
3. **Cutout** (`@Luna/Gaze GIF SpriteSheet Cutout`)
   - 完全不透明かつ最速が必要な場合

### Q2: シートの解像度はどれが適切？

```
モバイル/Quest: 512×512 または 1024×1024
VRChat推奨: 1024×1024 〜 2048×2048
ハイエンドPC: 2048×2048 〜 4096×4096
複数アセットで均一解像度にするとバッチが向上
```

### Q3: 凝視が不自然に見える場合は？

```
1. _SingleAxisGaze の設定を確認
2. _WeakenDistanceGaze を上げて遠距離効果を緩和
3. _ExtraRotX/Y/Z で微調整 (±5‑15°程度)
4. 法線マップを有効にして質感を向上
```

### Q4: 変換の "高性能" と "高品質" モードの違い？

```
高性能モード:
├─ 最大アトラス 2048×2048
├─ Quest、多人数シーン向け
└─ 高速読み込み・低メモリ

高品質モード:
├─ 最大アトラス 4096×4096
├─ デスクトップ/ハイエンド向け
└─ 最高のディテール
```

常に視覚要件を満たす最低解像度を使用してください。

### Q5: _NormalStrength の適切な値は？

```
負値 (-1〜-5): 法線反転（非推奨）
0〜1: 微妙〜標準ハイライト（1がデフォルト）
2〜3: はっきりとした凹凸
4〜5: 極端な誇張効果
```

### Q6: VRChatでちらつく場合は？

`_FixTransp = ON` に設定して透明度アーティファクトを修正。

または：
```
1. GIFアルファにノイズがないか確認
2. シェーダーブレンドモードを調整
3. カメラがZTest LEqualを使用しているか確認
```

### Q7: DrawCallが多すぎる場合の最適化は？

Material Managerを使用:
```
1. Tools → @Luna → Gaze Shader Material Manager
2. [Auto Optimize All] をクリック
3. 同じテクスチャで共有マテリアルを生成
4. シーン内のマテリアルを自動置換
```
DrawCallが大幅に減少します。

### Q8: インスペクタで設定変更が反映されない？

```
1. シェーダーがまだコンパイル中
2. マテリアルがレンダラーに正しく割り当てられていない
3. キャッシュ問題（Library/ShaderCacheを削除）
4. Play Mode中の変更は失われる（編集モードで行う）
```

---

## 技術的詳細

### シェーダー構成

```
Vertex Shader:
├─ ワールド/カメラデータの取得
├─ 凝視回転行列の算出
├─ ランダムシード生成
├─ スケール・回転変化を適用
└─ 変換後頂点を出力

Fragment Shader:
├─ 現在フレームインデックス計算
│   ├─ 時間とFPSから算出
│   ├─ 再生モードロジック含む
│   └─ 距離速度制御を考慮
├─ テクスチャサンプリング
│   ├─ TextureArray: UNITY_SAMPLE_TEX2DARRAY
│   └─ SpriteSheet: UV計算
├─ ライティング計算（任意）
│   ├─ 法線マップサンプル
│   ├─ 鏡面反射計算
│   └─ ライトボリューム統合
└─ 最終色を出力
```

### 主要アルゴリズム

#### 1. 凝視追跡

```glsl
float3 cameraDir = normalize(cameraPos - worldPos);
float3x3 rotMatrix = RotateTowardsCamera(cameraDir);
float3 finalPos = mul(rotMatrix, vertexPos);
```

回転行列を使用しジンバルロックを回避。軸制限や距離減衰対応。

#### 2. 距離速度制御

```glsl
float distance = length(worldPos - cameraPos);
float speedMultiplier;

if (SpeedChangeMode == 1) {
    speedMultiplier = 1.0 + (1 - distance / maxDist) * rate;
} else if (SpeedChangeMode == 2) {
    speedMultiplier = 1.0 - (1 - distance / maxDist) * rate;
}

float adjustedFPS = fps * speedMultiplier;
```

#### 3. フレーム計算

```glsl
float elapsedTime = time - startTime;
float frameIndex = floor(elapsedTime * adjustedFPS) % totalFrames;
if (playMode == ONCE) {
    frameIndex = min(frameIndex, totalFrames - 1);
} else if (playMode == RANDOM) {
    frameIndex = RandomSegment(seed);
} else if (playMode == MANUAL) {
    frameIndex = manualFrame;
}
```

#### 4. SpriteSheet UV計算

```glsl
int column = int(frameIndex) % columns;
int row = int(frameIndex) / columns;
float u = (column + uv.x) / columns;
float v = (row + uv.y) / rows;
float2 spriteSheetUV = float2(u, v);
```

### パフォーマンス

```
TextureArray:
├─ 長所: 最高性能・メモリ効率
├─ 短所: 対応プラットフォーム制限
└─ 基準: 100%

SpriteSheet:
├─ 長所: 幅広い互換性
├─ 短所: 大きなテクスチャ
└─ 性能: 約95%

Cutout:
├─ 長所: 最速（アルファなし）
├─ �短所: 不透明のみ
└─ 性能: 約105%
```

### 互換性

```
TextureArray (Gaze Gif):
├─ Windows/Mac: ✅
├─ WebGL: ❌
├─ Android/Quest: ⚠️
└─ iOS: ❌

SpriteSheet:
├─ 全プラットフォーム対応: ✅

Cutout:
├─ すべてのプラットフォーム: ✅ (最高互換性)
```

---

## 付録

### クイックプリセット

**デフォルト（初心者向け）**:
```
FPS: 12
PlayMode: Loop
Gaze: ON
SingleAxisGaze: All
SpeedChangeMode: Uniform
Brightness: 1.0
LightingEffect: ON
UseLightVolume: ON
```

**高性能設定**:
```
SpriteSheet解像度: 512×512
FPS: 12
LightingEffect: OFF
UseNormalMap: OFF
Gaze: Y軸 (単軸は性能向上)
```

**高品質設定**:
```
SpriteSheet解像度: 2048×2048 または TextureArray
FPS: 24
LightingEffect: ON
UseNormalMap: ON
NormalStrength: 1.0
SpecularSharpness: 30
```

### ファイル配置

```
Assets/@Luna/gaze gif shader/
├─ Editor/ (全エディタスクリプト + 本ドキュメント)
├─ Shader/ (3つのシェーダーとlight volume include)
├─ Scripts/ (UdonGazeGifTrigger.cs)
├─ logo/ (ソーシャルアイコン)
└─ README.md
```

### ソーシャル

- Booth: https://xianyuzi-luna.booth.pm/
- X (Twitter): https://x.com/lunabxgg
- Bilibili: https://space.bilibili.com/3546752913247086

---

## 更新履歴

### v1.0 (2026-03-03)

- 初版リリース：全機能、エディタツール、変換ツール、法線マップジェネレータ、マテリアルマネージャ、VRC連携、Udon対応、多言語化、ドキュメントを含む

---

### 🛒 商用ライセンス

このGitHubリポジトリのオープンソースコンテンツは**個人の非商用使用のみ**とします（例：非公開VRChatアバター、無料のVRChatワールドなど）。

### ⚠️ 商用利用制限

このシェーダーを直接または間接的に商業収益活動に使用することは禁止されています。例：

- 有料3Dモデル販売
- 有償アバターデザイン依頼
- 有料ワールド/シーン構築
- その他営利行為

### 💳 商用ライセンス購入

商用プロジェクトや有料作品での利用が必要な場合は、**BOOTH**で商用ライセンスを購入してください：

**👉 [商用ライセンスを購入](https://xianyuzi-luna.booth.pm/items/7555039) 👈**

---

**Gaze Shaderをご利用いただきありがとうございます！
ご質問はシェーダー右下のロゴから作者のソーシャルへお問い合わせください。**