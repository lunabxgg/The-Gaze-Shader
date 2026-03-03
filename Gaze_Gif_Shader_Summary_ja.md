# Gaze Shader - 完全な説明とマニュアル

**作者**: @咸鱼子Luna  
**プロジェクト**: Gaze Gif Shader 視線追従システム  
**バージョン**: 1.0  
**更新日**: 2026-03-03

---

## 📖 目次

1. [プロジェクト概要](#プロジェクト概要)
2. [機能と特徴](#機能と特徴)
3. [3つのShaderの比較](#3つのshaderの比較)
4. [クイックスタート](#クイックスタート)
5. [Shaderパラメータ詳細](#shaderパラメータ詳細)
6. [Editorツールの使用](#editorツールの使用)
7. [ワークフローガイド](#ワークフローガイド)
8. [VRC統合に関する説明](#vrc統合に関する説明)
9. [よくある質問](#よくある質問)
10. [技術的な詳細](#技術的な詳細)

---

## プロジェクト概要

**Gaze Shader** は、VRChat および Unity 向けに設計された高度なアニメーションレンダリングシステムであり、主に**視線追従型GIFアニメーション**効果を実現するために使用されます。このシステムは以下のことが可能です：

- 🎬 **GIFアニメーションのリアルタイム読み込みと再生**、柔軟なフレームレート制御をサポート
- 👁️ **自動カメラ追従**、アニメーションオブジェクトが常に観察者の方向を向く（ビルボード効果）
- 📏 **距離感知制御**、カメラとの距離に応じて再生速度を動的に調整
- 💡 **高度なライティング効果**、ノーマルマップ、スペキュラ反射、VRC Light Volume 統合を含む
- 🎭 **多彩な再生モード**、ループ、1回再生、ランダム、手動フレーム制御をサポート
- 🔀 **ランダムバリエーションのサポート**、スケール、回転などのパラメータのランダム化オプションを提供

---

## 機能と特徴

### コア機能

| 機能 | 説明 |
|------|------|
| **GIFアニメーション再生** | GIFファイルから自動的にフレームシーケンスを抽出して再生 |
| **視線追従** | オブジェクトが自動的に回転し、カメラの方向を向く（ビルボード効果） |
| **距離感知** | カメラまでの距離に応じてアニメーション速度を調整 |
| **再生モード** | ループ/1回/ランダム/手動の4つのモード |
| **ノーマルマップ** | ノーマルマップの生成と使用をサポートし、ハイライト効果を実現 |
| **ライティング統合** | VRC Light Volume（光照体積）との統合 |

### 高度な機能

| 特徴 | 説明 |
|------|------|
| **開始フレームのランダム化** | 各オブジェクトでランダムに開始フレームを選択可能 |
| **回転バリエーション** | X/Y/Z軸のランダムな回転変化をサポート |
| **スケールバリエーション** | オブジェクトのサイズをランダムに変化 |
| **多軸視線追従** | 全方向、X軸、Y軸、Z軸の単軸追従をサポート |
| **距離減衰** | 遠距離時に視線追従効果を弱める |
| **Alpha修正** | 透明度のアーティファクト問題を修正 |

---

## 3つのShaderの比較

### 1️⃣ **Gaze Gif** (@Luna/Gaze Gif)

- **テクスチャフォーマット**: Texture2DArray（推奨）
- **用途**: フレームシーケンスを正確に制御する必要があり、メモリに余裕がある場合
- **メリット**:
  - 最高のパフォーマンス効率
  - 最も多くのフレーム数をサポート
  - 1フレームあたりのメモリ消費が少ない
- **デメリット**:
  - GIFをTextureArrayに変換する必要がある
  - 一部のプラットフォームではTextureArrayがサポートされていない場合がある
- **推奨シーン**: 高品質なVRChatワールド、デスクトップアプリ、VRアプリ

### 2️⃣ **Gaze GIF SpriteSheet** (@Luna/Gaze GIF SpriteSheet)

- **テクスチャフォーマット**: 2D SpriteSheet（グリッドレイアウト）
- **用途**: 一般的なシーン、互換性が最も高い
- **メリット**:
  - **最強の互換性**、すべてのプラットフォームをサポート
  - 変換が簡単で高速
  - 列数、行数、フレーム数の調整が容易
- **デメリット**:
  - 1枚の大きなテクスチャが比較的多くのメモリを消費する可能性がある
  - パフォーマンスはTextureArrayよりわずかに劣る
- **推奨シーン**: VRChatワールド、Questプラットフォーム、一般的な制作環境

### 3️⃣ **Gaze GIF SpriteSheet Cutout** (@Luna/Gaze GIF SpriteSheet Cutout)

- **テクスチャフォーマット**: 2D SpriteSheet（不透明バージョン）
- **用途**: 完全に不透明なGIF、最高のパフォーマンスが必要な場合
- **メリット**:
  - **最高のパフォーマンス**
  - 深度書き込み（ZWrite）をサポート
  - Alphaブレンド計算が不要
- **デメリット**:
  - **完全に不透明な画像にのみ適用可能**
  - 半透明効果は表示不可
- **推奨シーン**: 不透明なGIFアニメーション、パフォーマンスに敏感なモバイルプラットフォーム

---

## クイックスタート

### ステップ 1: GIFファイルの準備

1. GIFファイルを準備します（透明背景をサポート）
2. GIFを `Assets/` ディレクトリに配置します
3. InspectorでGIFのインポートタイプを **Texture** に設定します

### ステップ 2: マテリアルの作成

#### 方式 A：SpriteSheetを使用（初心者推奨）

1. **マテリアル作成**: 右クリック → Create → Material
2. **Shader設定**: `@Luna/Gaze GIF SpriteSheet` を選択
3. **Inspectorにて**:
   - GIFファイルを "GIF Source" フィールドにドラッグ＆ドロップ
   - **"スプライトシートを生成 (生成精灵图)"** ボタンをクリック
   - Shaderが自動的にMainTex、Columns、Rows、TotalFramesを入力します

#### 方式 B：TextureArrayを使用（上級者向け）

1. **マテリアル作成**: 右クリック → Create → Material
2. **Shader設定**: `@Luna/Gaze Gif` を選択
3. **GIF変換**:
   - InspectorでToolメニューを見つけます
   - "GIF to Texture Array" を選択
   - 解像度を設定して生成します

### ステップ 3: パラメータの設定

```
基本設定（必須）：
├─ FPS (Base FPS): アニメーションのフレームレートを設定、通常 10-24
├─ Play Mode: 再生モードを選択（Loop/Once/Random/Manual）
└─ Color: 色の調整

視線追従：
├─ Gaze: 追従の有効/無効化
├─ Single Axis Gaze: 追従する軸を選択
└─ Weaken Distance Gaze: 遠距離時に追従を弱める

距離速度制御：
├─ Speed Change Mode: 速度変化モードを選択
├─ Speed Change Rate: 変化の強度を調整
└─ Max Distance: 影響する距離範囲を設定
```

### ステップ 4: シーンへの適用

1. Quad、Plane、または3Dモデルを作成します
2. Rendererに作成したマテリアルを適用します
3. Play Modeで効果をプレビューします

---

## Shaderパラメータ詳細

### 📦 テクスチャ部分 (Textures)

#### Gaze Gif.shader

```
_Textures (Texture Array)
  ├─ 説明: GIF変換で得られた Texture2DArray
  ├─ デフォルト: black
  └─ 用途: すべてのアニメーションフレームを保存

_NormalMapArray (2DArray, オプション)
  ├─ 説明: 対応するノーマルマップ配列
  ├─ デフォルト: bump
  └─ 用途: スペキュラ反射とハイライト効果を実現
```

#### Gaze GIF SpriteSheet.shader

```
_MainTex (2D)
  ├─ 説明: SpriteSheet結合画像
  ├─ デフォルト: white
  └─ 用途: すべてのアニメーションフレームを含むグリッド

_Columns & _Rows (IntRange)
  ├─ 説明: SpriteSheetの列数と行数
  ├─ 範囲: 1-64
  └─ 例: 4 列 × 4 行 = 16 フレーム

_TotalFrames (IntRange)
  ├─ 説明: 有効な合計フレーム数
  ├─ 範囲: 1-4096
  └─ 説明: 行数×列数より小さい値を設定して一部のフレームのみ使用可能
```

### 🎬 アニメーション制御 (Animation Control)

```
_fps (IntRange, デフォルト: 12)
  ├─ 範囲: 1-60
  └─ 説明: アニメーションの再生フレームレート
  
_PlayMode (Enum, デフォルト: Loop)
  ├─ Loop (0): ループ再生
  ├─ Once (1): 1回再生して停止
  ├─ Random (2): 異なるセグメントをランダムにループ再生
  └─ Manual (3): 手動でフレームを指定

_ManualFrame (IntRange, デフォルト: 1)
  ├─ 範囲: 1-100
  ├─ 用途: PlayMode=Manual の時に表示するフレーム番号を指定
  └─ 説明: 値 1 は最初のフレームを意味する

_StartFrameRandomization (Float, デフォルト: 0)
  ├─ 範囲: 0-1
  ├─ 説明: 各オブジェクトの開始フレームのランダム化の度合い
  └─ 例: 0.5 = 各オブジェクトが50%の範囲でランダムな開始フレームを選択
```

### 📏 視線追従 (Gaze Control)

```
_Gaze (Toggle, デフォルト: ON)
  ├─ ON: オブジェクトがカメラの方向を向く
  └─ OFF: オブジェクトが元の向きを維持する

_SingleAxisGaze (Enum, デフォルト: All)
  ├─ All (0): 全方向追従（最もリアル）
  ├─ X (1): X軸のみ追従
  ├─ Y (2): Y軸のみ追従（直立したキャラクターによく使われる）
  └─ Z (3): Z軸のみ追従

_WeakenDistanceGaze (Float, デフォルト: 0)
  ├─ 範囲: 0-1
  ├─ 説明: カメラから遠ざかるほど追従効果を弱める
  └─ 例: 0.5 = 最遠距離で追従強度が半分になる

_ExtraRotX/Y/Z (Float, デフォルト: 0)
  ├─ 範囲: -180 ~ 180°
  ├─ 説明: 追加の回転オフセット（視線回転に重畳）
  └─ 用途: 視線方向の調整やオブジェクトの向きの修正
```

### 📉 距離速度制御 (Distance Speed Control)

```
_SpeedChangeMode (Enum, デフォルト: Uniform)
  ├─ Uniform (0): 距離が速度に影響しない
  ├─ Accelerate (1): 近いほど速度が速くなる
  └─ Decelerate (2): 近いほど速度が遅くなる

_SpeedChangeRate (Float, デフォルト: 1)
  ├─ 範囲: 1-10
  ├─ 説明: 距離が速度に与える影響の強度
  └─ 例: 値が大きいほど速度変化が激しくなる

_MaxDistance (Float, デフォルト: 15)
  ├─ 範囲: 5-50 メートル
  ├─ 説明: 距離速度制御の基準となる距離
  └─ 説明: この距離を超えると加速/減速が停止する

_SpeedFromZero (Toggle, デフォルト: OFF)
  ├─ ON: 最短距離で速度を0にできる
  └─ OFF: 最短距離でも基本FPSを維持する
```

### 🎨 カラーとスケール (Color & Scale)

```
_Color (Color, デフォルト: White)
  ├─ 説明: テクスチャの色合いを調整
  └─ Alpha: 全体の透明度に影響

_ScaleX / _ScaleY (Float, デフォルト: 1)
  ├─ 説明: UVテクスチャのスケール
  └─ 用途: 表示されるテクスチャ部分の拡大/縮小

_UVx / _UVy (Float, デフォルト: 0)
  ├─ 説明: UVオフセット
  └─ 用途: テクスチャの表示位置の平行移動

_Brightness (Float, デフォルト: 1.0)
  ├─ 範囲: 0-5
  └─ 説明: 全体の明るさを調整
```

### 🎭 ランダムバリエーション (Random Variation)

```
_ScaleVariation (Float, デフォルト: 1)
  ├─ 範囲: 1-2
  ├─ 説明: オブジェクトのスケールのランダム変化幅
  └─ 例: 1.5 = スケールが 1x-1.5x の間でランダムに変化

_RandomRotXVariation (Float, デフォルト: 0)
  ├─ 範囲: 0-1
  ├─ 説明: X軸の回転のランダム化の度合い
  └─ 1.0 = ±180° のランダムな回転が可能

_RandomRotYVariation (Float, デフォルト: 0)
  ├─ 範囲: 0-1
  └─ 説明: Y軸の回転のランダム化の度合い

_RandomRotZVariation (Float, デフォルト: 0)
  ├─ 範囲: 0-1
  └─ 説明: Z軸の回転のランダム化の度合い
```

### 💡 ライティング効果 (Lighting Effect)

```
_LightingEffect (Toggle, デフォルト: ON)
  ├─ ON: ライティング計算を有効化
  └─ OFF: テクスチャそのもののみを表示

_Brightness (Float, デフォルト: 1.0)
  ├─ 範囲: 0-5
  └─ 説明: ライティングの明るさを調整

_UseNormalMap (Toggle, デフォルト: OFF)
  ├─ 説明: ノーマルマップのサポートを有効化
  └─ 依存: _NormalMapArray または _NormalMap と組み合わせる必要がある

_NormalStrength (Float, デフォルト: 1)
  ├─ 範囲: -5 ~ 5
  ├─ 説明: ノーマルマップの強度
  └─ 負の値は法線の方向を反転させる

_SpecularSharpness (Float, デフォルト: 20)
  ├─ 範囲: 1-100
  ├─ 説明: スペキュラ反射のシャープネス
  └─ 値が高いほど鋭く集中する

_SpecularBrightness (Float, デフォルト: 0.5)
  ├─ 範囲: 0-1
  └─ 説明: スペキュラ反射の明るさ
```

### 🌈 VRC 統合 (VRC Features)

```
_UseLightVolume (Toggle, デフォルト: ON)
  ├─ 説明: VRC Light Volume サポートを有効化
  └─ 用途: VRCワールドでLight Volumeの影響を受ける

_LightVolumeIntensity (Float, デフォルト: 1.0)
  ├─ 範囲: 0-2
  └─ 説明: Light Volume が影響を与える強度
```

### ☑️ 表示修正 (Display Fix)

```
_BackfaceCulling (Toggle, デフォルト: ON)
  ├─ 説明: 背面カリングを有効化
  └─ OFFにするとオブジェクトの背面が表示される

_FixTransp (Toggle, デフォルト: OFF)
  ├─ 説明: 透明度のアーティファクトを修正
  └─ エッジのちらつきが見られる場合はこのオプションをONにする
```

---

## Editorツールの使用

### 🎛️ Gaze Gif Shader GUI

これはShaderのカスタムエディタパネルです。

#### 主な機能エリア

**1. 作者情報エリア**
- Lunaのソーシャルメディアリンクを表示
- Booth、X (Twitter)、Bilibili

**2. ヘッダーエリア**
- Material Manager ボタン：マテリアルマネージャーを開く
- クイックShader切り替え（Gaze Gif / SpriteSheet / Cutout）
- 既存のマテリアルの即時編集

**3. リソースエリア (Resource Section)**

SpriteSheet バージョンの場合：
```
[スプライトシートを生成] ボタン
  ├─ 入力: GIFファイル
  ├─ 処理: SpriteSheetに自動変換
  └─ 出力: MainTex、Columns、Rows、TotalFrames
```

TextureArray バージョンの場合：
```
Tool メニュー
  ├─ GIF to Texture Array: GIFを変換
  └─ GIF to SpriteSheet: SpriteSheetに変換
```

**4. エフェクトエリア (Effect Section)**

- Play Mode の選択
- FPS の調整
- 開始フレームのランダム化
- 速度変化モードの設定

**5. 固定バリエーションエリア (Fixed Variation)**

- Extra Rotation (X/Y/Z)
- Scale、UV Offset など

**6. ランダムバリエーションエリア (Random Variation)**

- Scale Variation
- Random Rotation Variation

**7. 高度な設定エリア (Advanced)**

- ノーマルマップ設定
- スペキュラ反射パラメータ
- 背面カリングオプション

---

### 📁 Material Instance Manager

開く: `Tools → @Luna → Gaze Shader Material Manager`

#### 機能

```
左側パネル：
├─ 言語選択 (EN/CN/JA)
├─ クイックアクションボタン
│  ├─ [すべて自動最適化] - シーン内のマテリアルを一括最適化
│  └─ [再スキャン] - シーンのマテリアルを再スキャン
└─ マテリアルグループ表示
   ├─ テクスチャごとのグループ
   └─ テクスチャが割り当てられていないマテリアル

右側操作：
├─ [共有インスタンスの作成] - 同じテクスチャに対して共有マテリアルを作成
├─ [シーンの最適化] - 共有マテリアルインスタンスに置換
└─ [未使用のクリーンアップ] - シーン内の未使用マテリアルを削除
```

#### ワークフロー

```
1. Material Managerを開く
2. [再スキャン] をクリックしてすべてのマテリアルをスキャン
3. マテリアルグループと最適化の提案を確認
4. 手動または [すべて自動最適化] をクリックして最適化を実行
5. ウィンドウを閉じて結果を確認
```

---

### 🎨 Normal Map Generator

ノーマルマップを自動生成します。

#### 使用手順

1. **マテリアルで有効化**:
   - `_UseNormalMap` = ON に設定

2. **自動生成**:
   - Inspectorで **[法線マップを生成 (生成法线贴图)]** ボタンをクリック
   - システムがTextureに基づいて対応するノーマルマップを自動生成します

3. **パラメータの調整**:
   - `_NormalStrength`: 法線の強度（範囲 -5~5）
   - `_SpecularSharpness`: スペキュラ反射のシャープネス（範囲 1-100）
   - `_SpecularBrightness`: スペキュラ反射の明るさ（範囲 0-1）

#### ノーマルマップの原理

ノーマルマップは隣接するピクセルの輝度差を計算し、3Dの法線方向を生成することで以下の目的で使用されます：
- 凹凸の視覚効果を実現
- スペキュラ反射の計算
- ディテール感の強化

---

### 🔄 GIF 変換ツール

#### GIF to SpriteSheet Converter

**特徴**:
- ✅ 高速な変換
- ✅ 最強の互換性
- ✅ 高パフォーマンスモードと高品質モードをサポート

**プロセス**:
```
1. マテリアルのInspectorでGIFを選択
2. [スプライトシートを生成] をクリック
3. 品質モードを選択:
   - 高パフォーマンス: 1枚あたり ≤2048×2048
   - 高品質: 1枚あたり ≤4096×4096
4. グリッドレイアウト（行数×列数）を自動計算
5. Sprite Sheet を生成して適用
```

**品質の比較**:
| モード | 最大サイズ | パフォーマンス | 用途 |
|------|---------|------|------|
| 高パフォーマンス | 2048×2048 | 最適 | モバイル、VRChatマルチプレイ |
| 高品質 | 4096×4096 | 良好 | デスクトップ、ハイエンドプラットフォーム |

#### GIF to Texture Array Converter

**特徴**:
- ✅ パフォーマンスが最適
- ✅ フレーム数無制限
- ❌ 互換性に制限あり

**プロセス**:
```
1. マテリアルにGIFをドラッグ
2. 解像度を選択 (256-2048)
3. [変換] をクリック
4. システムが自動的にすべてのフレームを抽出
5. Texture2DArray ファイルを生成
```

**解像度選択ガイド**:
```
256×256:   超小型、高パフォーマンス
512×512:   推奨（バランス重視）
1024×1024: 高品質、高負荷
2048×2048: 最高品質、最高負荷
```

---

## ワークフローガイド

### ワークフロー 1: 高速プロトタイピング

```
ステップ 1: リソースの準備
  └─ GIFファイルを準備 (256×256以内を推奨)

ステップ 2: マテリアルの作成
  ├─ Create → Material
  └─ Shader = @Luna/Gaze GIF SpriteSheet

ステップ 3: GIF変換
  ├─ GIFを "GIF Source" にドラッグ
  └─ [スプライトシートを生成] をクリック

ステップ 4: 基本パラメータの設定
  ├─ FPS = 12-24
  ├─ Color = 適切に調整
  └─ Gaze = ON

ステップ 5: シーンへの適用
  ├─ Quadを作成
  └─ マテリアルを適用してプレビュー
```

### ワークフロー 2: VRChat ワールド最適化

```
ステップ 1: リソースの計画
  ├─ GIFアニメーションがいくつ必要か評価
  └─ 分類：小、中、大

ステップ 2: 一括変換（SpriteSheet）
  ├─ 高パフォーマンスモードを使用
  ├─ 4096×4096 以内に抑える
  └─ 同じサイズのGIFには統一された解像度を使用

ステップ 3: 共有マテリアルの作成
  ├─ Material Managerを開く
  ├─ [すべて自動最適化] をクリック
  └─ 同じテクスチャに対して共有インスタンスを作成

ステップ 4: Light Volume の設定
  ├─ _UseLightVolume を有効化
  ├─ 環境に応じて強度を設定 (0.8-1.2)
  └─ ライティング効果をテスト

ステップ 5: パフォーマンステスト
  ├─ Quest デバイスでテスト
  ├─ Draw Call とメモリを監視
  └─ 必要に応じて解像度を調整
```

### ワークフロー 3: 高度なビジュアルエフェクト

```
ステップ 1: 基本設定
  ├─ Gaze GIF SpriteSheet を選択
  └─ GIFを変換して適用

ステップ 2: 視線追従の微調整
  ├─ SingleAxisGaze: オブジェクトに合わせて軸を選択
  ├─ WeakenDistanceGaze: 遠距離時の効果を調整
  └─ ExtraRot: 向きを修正

ステップ 3: ノーマルマップの有効化
  ├─ _UseNormalMap = ON に設定
  ├─ [法線マップを生成] をクリック
  └─ _NormalStrength を調整 (1-2 が自然)

ステップ 4: スペキュラ反射の設定
  ├─ _SpecularSharpness = 20-50
  ├─ _SpecularBrightness = 0.3-0.7
  └─ ライティング下で効果を観察

ステップ 5: 距離速度制御
  ├─ SpeedChangeMode: Accelerate または Decelerate を選択
  ├─ SpeedChangeRate: 1-3（推奨）
  ├─ MaxDistance: 10-20 メートル
  └─ 異なる距離でのアニメーション速度をテスト

ステップ 6: ランダムバリエーション
  ├─ ScaleVariation: 1.2-1.5
  ├─ RandomRotVariation: 0.1-0.3（微調整）
  └─ 複数のコピーを作成してランダム効果を観察
```

---

## VRC統合に関する説明

### VRC Light Volume サポート

**Light Volumeとは？**

VRC Light Volumeは、VRChatワールド内の特定のエリアにライティングを追加するためのツールです。この機能を有効にすると、Gaze Shaderは自動的にLight Volumeのライティングに反応します。

**有効化の手順**:

1. **Shaderで有効化**:
   ```
   _UseLightVolume = ON
   _LightVolumeIntensity = 1.0 (デフォルト)
   ```

2. **VRCワールド内**:
   - ワールドにLight Volumeが配置されていることを確認
   - Shaderが自動的に検出し反応します

3. **強度の調整**:
   ```
   0.0: Light Volumeの影響を全く受けない
   1.0: 通常の影響（推奨）
   2.0: 影響を強化（明らかなライティングの変化）
   ```

### UdonSharp スクリプト統合

ファイル: [UdonGazeGifTrigger.cs](./Scripts/UdonGazeGifTrigger.cs)

**機能**: 視線アニメーションの再生時間を同期

**使用方法**:

1. Gaze Shaderマテリアルを含むGameObjectにこのスクリプトを追加します
2. スクリプトはOnEnable時に現在の時間を自動的に取得します
3. 時間をShaderの `_StartTime` パラメータに同期します
4. すべてのプレイヤーが同期されたアニメーションを見れるようにします

**動作原理**:

```csharp
// ゲームオブジェクトが有効になった時に自動実行
OnEnable() {
    // マテリアルを取得
    meshRenderer = GetComponent<Renderer>();
    
    // 開始時間を現在のゲーム時間に設定
    meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
    
    // Shaderはこの時間を使用して現在表示すべきフレームを計算
}
```

### VRChatでのベストプラクティス

```
1. ネットワーク問題
   ├─ マテリアルパラメータの頻繁な変更を避ける
   └─ AnimatorControllerを使用してアニメーションパラメータをプリセットする

2. パフォーマンス最適化
   ├─ シーン内の単一のGaze Shaderは50個を超えないようにする
   ├─ SpriteSheetを合理的に使用（推奨）
   └─ LODシステムを有効にする（遠くで解像度を下げる）

3. クロスプラットフォーム互換性
   ├─ TextureArrayではなくSpriteSheetを優先的に使用する
   ├─ QuestおよびPCプラットフォームでテストする
   └─ Draw Callの数を監視する

4. ビジュアルエフェクト
   ├─ Light Volume：環境光の影響を得るために有効化
   ├─ ノーマルマップ：ハイエンドデバイスでのみ有効化
   └─ 距離制御：シーンの空間に合わせてMaxDistanceを調整
```

---

## よくある質問

### Q1: どのShaderバージョンを選ぶべきですか？

**A**: 優先順位：

1. **第一候補: SpriteSheetバージョン** (`@Luna/Gaze GIF SpriteSheet`)
   - 最強の互換性
   - 99%のシーンに適しています

2. **選択: TextureArrayバージョン** (`@Luna/Gaze Gif`)
   - 条件：非常に多くのフレーム数が必要 (>256)
   - 条件：メモリに余裕がある
   - 条件：プラットフォームがサポートしていることを確認

3. **選択: Cutoutバージョン** (`@Luna/Gaze GIF SpriteSheet Cutout`)
   - 条件：画像が完全に不透明
   - 条件：最高のパフォーマンスが必要

### Q2: 変換後のSpriteSheetの解像度はどれくらいが適切ですか？

**A**: 用途によります：

```
モバイル/Quest:      512×512 または 1024×1024
VRChat 推奨:         1024×1024 から 2048×2048
ハイエンドPC:        2048×2048 から 4096×4096
複数のアニメーションで共有: 最適化のためにできるだけ統一する
```

### Q3: 視線追従が不自然に見えます。どうすればいいですか？

**A**: 以下のパラメータを調整してください：

```
1. _SingleAxisGaze を確認
   └─ キャラクターはY (2) 軸を使用し、その他はAll (0) を使用する

2. _WeakenDistanceGaze を調整
   └─ この値を増やすと遠くの効果が弱まり、より自然になります

3. _ExtraRotX/Y/Z を使用して向きを微調整
   └─ 通常は ±5-15° の小さな調整で十分です

4. ノーマルマップを有効にしてディテール感を高める
   └─ ライティングの反応がよりリアルになります
```

### Q4: GIFからSpriteSheetへの変換における「高パフォーマンス」と「高品質」の違いは何ですか？

**A**:

```
高パフォーマンスモード:
├─ 最大テクスチャ: 2048×2048
├─ 用途: Quest、マルチプレイシーン、パフォーマンス重視
└─ メリット: ロードが速い、メモリ消費が少ない

高品質モード:
├─ 最大テクスチャ: 4096×4096
├─ 用途: デスクトップ、ハイエンドVR
└─ メリット: ディテールが保持され、視覚的な品質が最高

選択の原則:
└─ 視覚的な要求を満たせる最も低い解像度を常に使用する
```

### Q5: ノーマルマップの _NormalStrength はいくつに設定すべきですか？

**A**:

```
負の値 (-1 ~ -5):
└─ 法線を反転、非推奨

0 ~ 1 (推奨):
├─ 0: ノーマル効果なし
├─ 0.5: 微妙なハイライト（自然）
└─ 1.0: 標準的なハイライト（推奨）

1 ~ 5 (强化):
├─ 2-3: 明らかな凹凸感
└─ 4-5: 極端に誇張された効果
```

### Q6: VRChatでちらついて見えます。どうすればいいですか？

**A**: これは透明度のアーティファクトです。`_FixTransp = ON` を有効にしてください。

または：
```
1. GIFのAlphaチャンネルを確認
   └─ エッジが半透明のノイズになっていないか確認

2. ShaderのBlend Modeを調整
   └─ シーンによってはブレンドモードの変更が必要かもしれません

3. カメラの深度設定を確認
   └─ ZTest = LEqual になっているか確認
```

### Q7: Draw Callが多すぎます。どのように最適化すればいいですか？

**A**: Material Managerを使用します：

```
1. Tools → @Luna → Gaze Shader Material Manager を開く
2. [すべて自動最適化] をクリック
3. システムが同じテクスチャに対して共有マテリアルインスタンスを作成します
4. シーン内の重複したマテリアルを自動的に置換します

結果: Draw Callが大幅に減少します
```

### Q8: Inspectorでパラメータを変更しても反応しませんか？

**A**: 以下の点を確認してください：

```
1. Shaderが完全にロードされていない
   └─ しばらく待ってエディタを再起動する

2. マテリアルが正しく適用されていない
   └─ Rendererで再度適用する

3. 以前の設定がキャッシュされている
   └─ Library/ShaderCache フォルダを削除する

4. Play Mode 中にパラメータを変更した
   └─ Play Modeが終了すると変更は失われます
   └─ 編集モード（Edit Mode）で変更する必要があります
```

---

## 技術的な詳細

### Shader アーキテクチャ

```
Gaze Shader コア構造:

Vertex Shader:
├─ ワールド座標とカメラ情報の取得
├─ 視線回転行列の計算
├─ オブジェクトシードの生成（ランダム性に使用）
├─ スケール/回転バリエーションの適用
└─ 変換された頂点の出力

Fragment Shader:
├─ 現在のフレームインデックスの計算
│  ├─ 時間とFPSに基づく
│  ├─ PlayModeロジックをサポート
│  └─ 距離速度制御の考慮
├─ テクスチャのサンプリング
│  ├─ TextureArrayバージョン: UNITY_SAMPLE_TEX2DARRAY
│  └─ SpriteSheetバージョン: UVを計算してサンプリング
├─ ライティングの計算（オプション）
│  ├─ ノーマルマップのサンプリング
│  ├─ スペキュラ反射の計算
│  └─ Light Volume 統合
└─ 最終カラーの出力
```

### 主要アルゴリズム

#### 1. 視線追従 (Gaze Tracking)

```glsl
// 簡易バージョン
float3 cameraDir = normalize(cameraPos - worldPos);
float3x3 rotMatrix = RotateTowardsCamera(cameraDir);
float3 finalPos = mul(rotMatrix, vertexPos);
```

**特徴**:
- ジンバルロックを避けるため、オイラー角ではなく回転行列を使用
- 多軸追従の選択をサポート
- 距離減衰の滑らかなトランジション

#### 2. 距離速度制御

```glsl
float distance = length(worldPos - cameraPos);
float speedMultiplier;

if (SpeedChangeMode == 1) {
    // Accelerate: 近いほど速い
    speedMultiplier = 1.0 + (1 - dist / maxDist) * rate;
} else if (SpeedChangeMode == 2) {
    // Decelerate: 近いほど遅い
    speedMultiplier = 1.0 - (1 - dist / maxDist) * rate;
}

float adjustedFPS = fps * speedMultiplier;
```

#### 3. フレーム計算 (Frame Calculation)

```glsl
float elapsedTime = time - startTime;
float frameIndex = floor(elapsedTime * adjustedFPS) % totalFrames;

// 異なるPlayModeをサポート
if (playMode == ONCE) {
    frameIndex = min(frameIndex, totalFrames - 1);
} else if (playMode == RANDOM) {
    // ランダムなセグメントを計算
    frameIndex = RandomSegment(seed);
} else if (playMode == MANUAL) {
    frameIndex = manualFrame;
}
```

#### 4. SpriteSheet UV 計算

```glsl
int column = int(frameIndex) % columns;
int row = int(frameIndex) / columns;

float u = (column + uv.x) / columns;
float v = (row + uv.y) / rows;

float2 spriteSheetUV = float2(u, v);
```

### パフォーマンスの考慮事項

```
TextureArray バージョン:
├─ メリット: 最高のパフォーマンス、最適なメモリ利用
├─ デメリット: TextureArrayがすべてのプラットフォームでサポートされているとは限らない
└─ パフォーマンス: 最適 (100% 基準)

SpriteSheet バージョン:
├─ メリット: 互換性が高い、変換が簡単
├─ デメリット: 1枚の大きなテクスチャがより多くのメモリを消費する可能性がある
└─ パフォーマンス: 非常に良い (95% 相対)

Cutout バージョン:
├─ メリット: パフォーマンスが最も高い（Alphaブレンドなし）
├─ デメリット: 不透明な画像のみサポート
└─ パフォーマンス: 最適 (105% 相対、最速)
```

### 互換性

```
プラットフォームの互換性:

TextureArray (Gaze Gif):
├─ Windows/Mac: ✅ 完全サポート
├─ WebGL: ❌ サポートなし
├─ Android/Quest: ⚠️ 限定的サポート
└─ iOS: ❌ サポートなし

SpriteSheet (推奨):
├─ Windows/Mac: ✅ 完全サポート
├─ WebGL: ✅ 完全サポート
├─ Android/Quest: ✅ 完全サポート
└─ iOS: ✅ 完全サポート

Cutout:
├─ すべてのプラットフォーム: ✅ 完全サポート
└─ 最高の互換性
```

---

## 付録

### クイックリファレンス

#### パラメータのプリセット

**デフォルト設定（初心者推奨）**:
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

**高パフォーマンス設定**:
```
SpriteSheet 解像度: 512×512
FPS: 12
LightingEffect: OFF
UseNormalMap: OFF
Gaze: Y軸 (単軸は全方向よりパフォーマンスが良い)
```

**高品質設定**:
```
SpriteSheet 解像度: 2048×2048 または TextureArray
FPS: 24
LightingEffect: ON
UseNormalMap: ON
NormalStrength: 1.0
SpecularSharpness: 30
```

### ファイルの場所クイックリファレンス

```
Assets/@Luna/gaze gif shader/
├─ Editor/
│  ├─ AuthorInfoDrawer.cs
│  ├─ GazeGifShaderGUI.cs
│  ├─ GazeShaderLocalization.cs
│  ├─ GifToSpriteSheetConverter.cs
│  ├─ GifToTextureArrayConverter.cs
│  ├─ MaterialInstanceManager.cs
│  └─ NormalMapGenerator.cs
│
├─ Shader/
│  ├─ Gaze Gif.shader (TextureArray版)
│  ├─ Gaze GIF SpriteSheet.shader (推奨)
│  ├─ Gaze GIF SpriteSheet Cutout.shader (不透明版)
│  └─ LightVolumes.cginc (VRC 光照体積)
│
├─ Scripts/
│  └─ UdonGazeGifTrigger.cs (VRC Udon スクリプト)
│
├─ logo/
│  ├─ s booth.png
│  ├─ s x.png
│  ├─ s b.png
│  ├─ @咸鱼子Luna.png
│  └─ Luna_ko.png
│
├─ Gaze_Gif_Shader_Summary_cn.md 
├─ Gaze_Gif_Shader_Summary_en.md
├─ Gaze_Gif_Shader_Summary_ja.md (このドキュメント)
└─ README.md (プロジェクト説明)


```

### ソーシャルメディア

- **Booth**: https://xianyuzi-luna.booth.pm/
- **X (Twitter)**: https://x.com/lunabxgg
- **Bilibili**: https://space.bilibili.com/3546752913247086

---

## 更新履歴

### v1.0 (2026-03-03)

**初期リリースバージョン、以下の内容を含む**:

- ✅ 3つのShaderバージョン（TextureArray、SpriteSheet、Cutout）
- ✅ 完全なEditorツールスイート
- ✅ GIF変換ツール（TextureArray および SpriteSheet）
- ✅ ノーマルマップジェネレーター
- ✅ マテリアルマネージャー
- ✅ VRC Light Volume 統合
- ✅ UdonSharp サポートスクリプト
- ✅ 多言語ローカリゼーション（EN/CN/JA）
- ✅ 完全なドキュメントとサンプル

---

### 🛒 商用利用ライセンス (Commercial License)

このGitHubリポジトリでオープンソースとして公開されているコンテンツは、**個人の無料・非商用利用**（例：プライベートなVRChat Avatarでの使用、または無料のVRChat Worldの公開）にのみ提供されます。

### ⚠️ 商用利用の制限

本シェーダーを直接的または間接的に、いかなる商業的な営利行為に使用することも**固く禁じます**。これには以下が含まれますが、これらに限定されません：

- 商用3Dモデルの販売
- 有料のAvatarカスタマイズ依頼
- 有料のシーン制作
- その他の商業的営利目的

### 💳 商用ライセンスの購入

商用作品や有料プロジェクトでこのシステムを使用する場合は、**BOOTH** にて商用利用ライセンスを購入してください：

**👉 [BOOTHでの商用ライセンス購入はこちらをクリック](https://xianyuzi-luna.booth.pm/items/7555039) 👈**

---

**Gaze Shaderをご利用いただきありがとうございます！ご質問がある場合は、シェーダーインターフェース右下の作者ロゴ--@咸鱼子Luna--のソーシャルメディアを通じて作者にご連絡ください。**