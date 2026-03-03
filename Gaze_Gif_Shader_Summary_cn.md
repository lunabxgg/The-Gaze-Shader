# The Gaze Shader - 完整说明与使用手册

**作者**: @咸鱼子Luna  
**项目**: Gaze Gif Shader 凝视追踪系统  
**版本**: 1.0  
**更新日期**: 2026-03-03

---

## 📖 目录

1. [项目概述](#项目概述)
2. [功能特性](#功能特性)
3. [三种 Shader 对比](#三种-shader-对比)
4. [快速开始](#快速开始)
5. [Shader 参数详解](#shader-参数详解)
6. [Editor 工具使用](#editor-工具使用)
7. [工作流程指南](#工作流程指南)
8. [VRC 集成说明](#vrc-集成说明)
9. [常见问题](#常见问题)
10. [技术细节](#技术细节)

---

## 项目概述

**Gaze Shader** 是一个专为 VRChat 和 Unity 设计的高级动画渲染系统，主要用于实现**凝视追踪型 GIF 动画**效果。该系统能够：

- 🎬 **实时加载和播放 GIF 动画**，支持灵活的帧率控制
- 👁️ **自动追踪摄像机**，使动画对象始终面向观察者
- 📏 **距离感应控制**，根据与摄像机的距离动态调整播放速度
- 💡 **高级光照效果**，包括法线贴图、镜面反射和 VRC 光照体积集成
- 🎭 **多种播放模式**，支持循环、单次、随机和手动帧控制
- 🔀 **随机变化支持**，提供缩放、旋转等参数的随机化选项

---

## 功能特性

### 核心功能

| 功能 | 描述 |
|------|------|
| **GIF 动画播放** | 自动从 GIF 文件提取帧序列并播放 |
| **凝视追踪** | 对象自动旋转面向摄像机（广告牌效果） |
| **距离感应** | 根据到摄像机的距离调整动画速度 |
| **播放模式** | 循环/单次/随机/手动四种模式 |
| **法线贴图** | 支持生成和使用法线贴图实现高光效果 |
| **光照集成** | 与 VRC 光照体积（Light Volumes）集成 |

### 高级特性

| 特性 | 说明 |
|------|------|
| **起始帧随机化** | 每个对象可随机选择起始帧 |
| **旋转变化** | 支持 X/Y/Z 轴的随机旋转变化 |
| **缩放变化** | 对象大小可随机变化 |
| **多轴凝视** | 支持全向、X轴、Y轴、Z轴单轴追踪 |
| **距离衰减** | 远距离时减弱凝视效果 |
| **Alpha 修复** | 修复透明度伪影问题 |

---

## 三种 Shader 对比

### 1️⃣ **Gaze Gif** (@Luna/Gaze Gif)

- **纹理格式**: Texture2DArray（推荐）
- **用途**: 需要精准控制帧序列且内存充足的情况
- **优点**:
  - 最高的性能效率
  - 支持最多的帧数
  - 每帧占用内存少
- **缺点**:
  - 需要将 GIF 转换为 TextureArray
  - 某些平台可能不支持 TextureArray
- **推荐场景**: 高质量 VRChat 世界、桌面应用、VR 应用

### 2️⃣ **Gaze GIF SpriteSheet** (@Luna/Gaze GIF SpriteSheet)

- **纹理格式**: 2D SpriteSheet（网格布局）
- **用途**: 通用场景，兼容性最好
- **优点**:
  - **兼容性最强**，支持所有平台
  - 转换简单快速
  - 易于调整列数、行数和帧数
- **缺点**:
  - 单张大纹理可能占用较多内存
  - 性能略低于 TextureArray
- **推荐场景**: VRChat 世界、Quest 平台、一般生产环境

### 3️⃣ **Gaze GIF SpriteSheet Cutout** (@Luna/Gaze GIF SpriteSheet Cutout)

- **纹理格式**: 2D SpriteSheet（不透明版本）
- **用途**: 完全不透明的 GIF，需要最高性能
- **优点**:
  - **最高性能**
  - 支持深度写入
  - 不需要 Alpha 混合计算
- **缺点**:
  - **只适用于完全不透明图像**
  - 无法显示半透明效果
- **推荐场景**: 不透明的 GIF 动画、性能敏感的移动平台

---

## 快速开始

### 步骤 1: 准备 GIF 文件

1. 准备您的 GIF 文件（支持透明背景）
2. 将 GIF 放入 `Assets/` 目录
3. 在 Inspector 中设置 GIF 导入为 **Texture** 类型

### 步骤 2: 创建材质

#### 方式 A：使用 SpriteSheet（推荐新手）

1. **创建材质**: 右键 → Create → Material
2. **设置 Shader**: 选择 `@Luna/Gaze GIF SpriteSheet`
3. **在 Inspector 中**:
   - 拖入 GIF 文件到 "GIF Source" 字段
   - 点击 **"生成精灵图"** 按钮
   - Shader 自动填充 MainTex、Columns、Rows、TotalFrames

#### 方式 B：使用 TextureArray（高级用户）

1. **创建材质**: 右键 → Create → Material
2. **设置 Shader**: 选择 `@Luna/Gaze Gif`
3. **转换 GIF**:
   - 在 Inspector 中找到 Tool 菜单
   - 选择 "GIF to Texture Array"
   - 设置分辨率并生成

### 步骤 3: 配置参数

```
基础设置（必填）：
├─ FPS (Base FPS): 设置动画帧率，典型值 10-24
├─ Play Mode: 选择播放模式（Loop/Once/Random/Manual）
└─ Color: 调整颜色

凝视追踪：
├─ Gaze: 启用/禁用追踪
├─ Single Axis Gaze: 选择追踪轴向
└─ Weaken Distance Gaze: 远距离时减弱追踪

距离速度控制：
├─ Speed Change Mode: 选择速度变化模式
├─ Speed Change Rate: 调整变化强度
└─ Max Distance: 设置影响距离范围
```

### 步骤 4: 应用到场景

1. 创建 Quad、Plane 或 3D 模型
2. 在 Renderer 上应用创建的材质
3. 在 Play Mode 中预览效果

---

## Shader 参数详解

### 📦 纹理部分 (Textures)

#### Gaze Gif.shader

```
_Textures (Texture Array)
  ├─ 说明: GIF 转换得到的 Texture2DArray
  ├─ 默认: black
  └─ 用途: 存储所有动画帧

_NormalMapArray (2DArray, 可选)
  ├─ 说明: 对应的法线贴图数组
  ├─ 默认: bump
  └─ 用途: 实现镜面反射和高光效果
```

#### Gaze GIF SpriteSheet.shader

```
_MainTex (2D)
  ├─ 说明: SpriteSheet 拼合图
  ├─ 默认: white
  └─ 用途: 包含所有动画帧的网格

_Columns & _Rows (IntRange)
  ├─ 说明: SpriteSheet 的列数和行数
  ├─ 范围: 1-64
  └─ 例子: 4 列 × 4 行 = 16 帧

_TotalFrames (IntRange)
  ├─ 说明: 有效帧总数
  ├─ 范围: 1-4096
  └─ 说明: 可小于 行数×列数 以使用部分帧
```

### 🎬 动画控制 (Animation Control)

```
_fps (IntRange, 默认: 12)
  ├─ 范围: 1-60
  └─ 说明: 动画播放帧率
  
_PlayMode (Enum, 默认: Loop)
  ├─ Loop (0): 循环播放
  ├─ Once (1): 播放一次后停止
  ├─ Random (2): 随机循环播放不同片段
  └─ Manual (3): 手动指定帧

_ManualFrame (IntRange, 默认: 1)
  ├─ 范围: 1-100
  ├─ 用途: 当 PlayMode=Manual 时指定显示的帧号
  └─ 说明: 值为 1 表示第一帧

_StartFrameRandomization (Float, 默认: 0)
  ├─ 范围: 0-1
  ├─ 说明: 每个对象起始帧随机化程度
  └─ 例子: 0.5 = 每个对象随机选择 50% 的起始帧位置
```

### 📏 凝视追踪 (Gaze Control)

```
_Gaze (Toggle, 默认: ON)
  ├─ 开启: 对象面向摄像机
  └─ 关闭: 对象保持原始朝向

_SingleAxisGaze (Enum, 默认: All)
  ├─ All (0): 全向追踪（最逼真）
  ├─ X (1): 仅在 X 轴追踪
  ├─ Y (2): 仅在 Y 轴追踪（常用于竖站的人物）
  └─ Z (3): 仅在 Z 轴追踪

_WeakenDistanceGaze (Float, 默认: 0)
  ├─ 范围: 0-1
  ├─ 说明: 距离摄像机更远时减弱追踪效果
  └─ 例子: 0.5 = 最远距离处追踪强度减半

_ExtraRotX/Y/Z (Float, 默认: 0)
  ├─ 范围: -180 ~ 180°
  ├─ 说明: 额外的旋转偏移（叠加在凝视旋转上）
  └─ 用途: 调整凝视方向或修正对象朝向
```

### 📉 距离速度控制 (Distance Speed Control)

```
_SpeedChangeMode (Enum, 默认: Uniform)
  ├─ Uniform (0): 距离不影响速度
  ├─ Accelerate (1): 越近速度越快
  └─ Decelerate (2): 越近速度越慢

_SpeedChangeRate (Float, 默认: 1)
  ├─ 范围: 1-10
  ├─ 说明: 距离对速度的影响强度
  └─ 示例: 值越大，速度变化越剧烈

_MaxDistance (Float, 默认: 15)
  ├─ 范围: 5-50 米
  ├─ 说明: 距离速度控制的参考距离
  └─ 说明: 超过此距离后不再继续加速/减速

_SpeedFromZero (Toggle, 默认: OFF)
  ├─ 开启: 最近距离时速度可以为 0
  └─ 关闭: 最近距离时保持基础 FPS
```

### 🎨 颜色和缩放 (Color & Scale)

```
_Color (Color, 默认: White)
  ├─ 说明: 对纹理进行着色
  └─ Alpha: 影响整体透明度

_ScaleX / _ScaleY (Float, 默认: 1)
  ├─ 说明: UV 纹理缩放
  └─ 用途: 放大/缩小显示的纹理部分

_UVx / _UVy (Float, 默认: 0)
  ├─ 说明: UV 偏移
  └─ 用途: 平移纹理显示位置

_Brightness (Float, 默认: 1.0)
  ├─ 范围: 0-5
  └─ 说明: 调整整体亮度
```

### 🎭 随机变化 (Random Variation)

```
_ScaleVariation (Float, 默认: 1)
  ├─ 范围: 1-2
  ├─ 说明: 对象缩放的随机变化幅度
  └─ 例子: 1.5 = 缩放可在 1x-1.5x 之间随机

_RandomRotXVariation (Float, 默认: 0)
  ├─ 范围: 0-1
  ├─ 说明: X 轴旋转随机化程度
  └─ 1.0 = 可随机旋转 ±180°

_RandomRotYVariation (Float, 默认: 0)
  ├─ 范围: 0-1
  └─ 说明: Y 轴旋转随机化程度

_RandomRotZVariation (Float, 默认: 0)
  ├─ 范围: 0-1
  └─ 说明: Z 轴旋转随机化程度
```

### 💡 光照效果 (Lighting Effect)

```
_LightingEffect (Toggle, 默认: ON)
  ├─ 开启: 启用光照计算
  └─ 关闭: 仅显示纹理本身

_Brightness (Float, 默认: 1.0)
  ├─ 范围: 0-5
  └─ 说明: 调整光照亮度

_UseNormalMap (Toggle, 默认: OFF)
  ├─ 说明: 启用法线贴图支持
  └─ 依赖: 需要配合 _NormalMapArray 或 _NormalMap

_NormalStrength (Float, 默认: 1)
  ├─ 范围: -5 ~ 5
  ├─ 说明: 法线贴图强度
  └─ 负值反转法线方向

_SpecularSharpness (Float, 默认: 20)
  ├─ 范围: 1-100
  ├─ 说明: 镜面反射的锐度
  └─ 值越高越锐利集中

_SpecularBrightness (Float, 默认: 0.5)
  ├─ 范围: 0-1
  └─ 说明: 镜面反射的亮度
```

### 🌈 VRC 集成 (VRC Features)

```
_UseLightVolume (Toggle, 默认: ON)
  ├─ 说明: 启用 VRC Light Volume 支持
  └─ 用途: 在 VRC 世界中被光照体积影响

_LightVolumeIntensity (Float, 默认: 1.0)
  ├─ 范围: 0-2
  └─ 说明: 光照体积影响的强度
```

### ☑️ 显示修复 (Display Fix)

```
_BackfaceCulling (Toggle, 默认: ON)
  ├─ 说明: 启用背面剔除
  └─ 关闭可显示对象的背面

_FixTransp (Toggle, 默认: OFF)
  ├─ 说明: 修复透明度伪影
  └─ 如果看到边缘闪烁，启用此选项
```

---

## Editor 工具使用

### 🎛️ Gaze Gif Shader GUI

这是 Shader 的自定义编辑器面板。

#### 主要功能区域

**1. 作者信息区**
- 显示 咸鱼子Luna 的社交媒体链接
- Booth、X (Twitter)、Bilibili

**2. Header 区**
- Material Manager 按钮：打开材质管理器
- 快速 Shader 切换（Gaze Gif / SpriteSheet / Cutout）
- 即时编辑现有材质

**3. 资源区 (Resource Section)**

对于 SpriteSheet 版本：
```
[生成精灵图] 按钮
  ├─ 输入: GIF 文件
  ├─ 处理: 自动转换为 SpriteSheet
  └─ 输出: MainTex、Columns、Rows、TotalFrames
```

对于 TextureArray 版本：
```
Tool 菜单
  ├─ GIF to Texture Array: 转换 GIF
  └─ GIF to SpriteSheet: 转换为 SpriteSheet
```

**4. 效果区 (Effect Section)**

- Play Mode 选择
- FPS 调整
- 起始帧随机化
- 速度变化模式配置

**5. 固定变化区 (Fixed Variation)**

- Extra Rotation (X/Y/Z)
- Scale、UV Offset 等

**6. 随机变化区 (Random Variation)**

- Scale Variation
- Random Rotation Variation

**7. 高级区 (Advanced)**

- 法线贴图设置
- 镜面反射参数
- 背光剔除选项

---

### 📁 Material Instance Manager

打开: `Tools → @Luna → Gaze Shader Material Manager`

#### 功能

```
左侧面板：
├─ 语言选择 (EN/CN/JA)
├─ 快速操作按钮
│  ├─ [自动优化全部] - 批量优化场景中的材质
│  └─ [刷新扫描] - 重新扫描场景材质
└─ 材质分组显示
   ├─ 按纹理分组
   └─ 未分配纹理的材质

右侧操作：
├─ [创建共享实例] - 为相同纹理创建共享材质
├─ [优化场景] - 替换为共享材质实例
└─ [清理未使用] - 删除场景中未使用的材质
```

#### 工作流程

```
1. 打开 Material Manager
2. 点击 [刷新扫描] 扫描所有材质
3. 查看材质分组和优化建议
4. 手动或点击 [自动优化全部] 进行优化
5. 关闭窗口验证结果
```

---

### 🎨 Normal Map Generator

自动生成法线贴图。

#### 使用步骤

1. **在材质中启用**:
   - 设置 `_UseNormalMap` = ON

2. **自动生成**:
   - Inspector 中点击 **[生成法线贴图]** 按钮
   - 系统自动根据 Texture 生成对应的法线贴图

3. **调整参数**:
   - `_NormalStrength`: 法线强度（范围 -5~5）
   - `_SpecularSharpness`: 镜面反射锐度（范围 1-100）
   - `_SpecularBrightness`: 镜面反射亮度（范围 0-1）

#### 法线贴图原理

法线贴图通过计算相邻像素的亮度差，生成 3D 法线方向，用于：
- 实现凹凸视觉效果
- 计算镜面反射
- 增强细节感

---

### 🔄 GIF 转换工具

#### GIF to SpriteSheet Converter

**特点**:
- ✅ 快速转换
- ✅ 兼容性最强
- ✅ 支持高性能和高质量模式

**流程**:
```
1. 在材质 Inspector 中选择 GIF
2. 点击 [生成精灵图]
3. 选择质量模式:
   - 高性能: 单张 ≤2048×2048
   - 高质量: 单张 ≤4096×4096
4. 自动计算网格布局（行数×列数）
5. 生成 Sprite Sheet 并应用
```

**质量对比**:
| 模式 | 最大尺寸 | 性能 | 用途 |
|------|---------|------|------|
| 高性能 | 2048×2048 | 最优 | 移动、VRChat 多人 |
| 高质量 | 4096×4096 | 好 | 桌面、高端平台 |

#### GIF to Texture Array Converter

**特点**:
- ✅ 性能最优
- ✅ 帧数无限制
- ❌ 兼容性受限

**流程**:
```
1. 在材质中拖入 GIF
2. 选择分辨率 (256-2048)
3. 点击 [转换]
4. 系统自动提取所有帧
5. 生成 Texture2DArray 文件
```

**分辨率选择指南**:
```
256×256:   超小、高性能
512×512:   推荐（平衡方案）
1024×1024: 高质量、高消耗
2048×2048: 最高质量、最高消耗
```

---

## 工作流程指南

### 工作流 1: 快速原型制作

```
第 1 步: 准备资源
  └─ 准备 GIF 文件 (推荐不超过 256×256)

第 2 步: 创建材质
  ├─ Create → Material
  └─ Shader = @Luna/Gaze GIF SpriteSheet

第 3 步: 转换 GIF
  ├─ 拖入 GIF 到 "GIF Source"
  └─ 点击 [生成精灵图]

第 4 步: 配置基础参数
  ├─ FPS = 12-24
  ├─ Color = 适当调整
  └─ Gaze = ON

第 5 步: 应用到场景
  ├─ 创建 Quad
  └─ 应用材质即可预览
```

### 工作流 2: VRChat 世界优化

```
第 1 步: 资源规划
  ├─ 评估需要多少个 GIF 动画
  └─ 分类：小型、中型、大型

第 2 步: 批量转换（SpriteSheet）
  ├─ 使用高性能模式
  ├─ 保活 4096×4096 以内
  └─ 为相同大小的 GIF 使用统一分辨率

第 3 步: 创建共享材质
  ├─ 打开 Material Manager
  ├─ 点击 [自动优化全部]
  └─ 为相同纹理创建共享实例

第 4 步: Light Volume 配置
  ├─ 启用 _UseLightVolume
  ├─ 根据环境设置强度 (0.8-1.2)
  └─ 测试光照效果

第 5 步: 性能测试
  ├─ 在 Quest 设备上测试
  ├─ 监控 Draw Call 和内存
  └─ 必要时调整分辨率
```

### 工作流 3: 高级视觉效果

```
第 1 步: 基础配置
  ├─ 选择 Gaze GIF SpriteSheet
  └─ 转换并应用 GIF

第 2 步: 凝视追踪微调
  ├─ SingleAxisGaze: 根据对象选择轴向
  ├─ WeakenDistanceGaze: 调整远距离效果
  └─ ExtraRot: 修正朝向

第 3 步: 启用法线贴图
  ├─ 设置 _UseNormalMap = ON
  ├─ 点击 [生成法线贴图]
  └─ 调整 _NormalStrength (1-2 较自然)

第 4 步: 镜面反射配置
  ├─ _SpecularSharpness = 20-50
  ├─ _SpecularBrightness = 0.3-0.7
  └─ 在光照下观察效果

第 5 步: 距离速度控制
  ├─ SpeedChangeMode: 选择 Accelerate 或 Decelerate
  ├─ SpeedChangeRate: 1-3（推荐）
  ├─ MaxDistance: 10-20 米
  └─ 测试不同距离的动画速度

第 6 步: 随机变化
  ├─ ScaleVariation: 1.2-1.5
  ├─ RandomRotVariation: 0.1-0.3（微调）
  └─ 创建多个副本观察随机效果
```

---

## VRC 集成说明

### VRC Light Volume 支持

**什么是 Light Volume？**

VRC Light Volume 是 VRChat 世界中用于为特定区域添加光照的工具。启用此功能后，Gaze Shader 会自动响应 Light Volume 的光照。

**启用步骤**:

1. **在 Shader 中启用**:
   ```
   _UseLightVolume = ON
   _LightVolumeIntensity = 1.0 (默认)
   ```

2. **在 VRC 世界中**:
   - 确保世界中已放置 Light Volume
   - Shader 自动检测并响应

3. **强度调整**:
   ```
   0.0: 完全不受 Light Volume 影响
   1.0: 正常影响（推荐）
   2.0: 加强影响（明显光照变化）
   ```

### UdonSharp 脚本集成

文件: [UdonGazeGifTrigger.cs](./Scripts/UdonGazeGifTrigger.cs)

**功能**: 同步凝视动画播放时间

**使用方式**:

1. 在包含 Gaze Shader 材质的 GameObject 上添加此脚本
2. 脚本自动在 OnEnable 时获取当前时间
3. 将时间同步到 Shader 的 `_StartTime` 参数
4. 确保所有玩家看到的是同步的动画

**工作原理**:

```csharp
// 当游戏对象启用时自动执行
OnEnable() {
    // 获取材质
    meshRenderer = GetComponent<Renderer>();
    
    // 设置启动时间为当前游戏时间
    meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
    
    // Shader 使用此时间计算当前应显示的帧
}
```

### 在 VRChat 中的最佳实践

```
1. 网络问题
   ├─ 避免频繁改变材质参数
   └─ 使用 AnimatorController 预设动画参数

2. 性能优化
   ├─ 场景中单个 Gaze Shader 不超过 50 个
   ├─ 合理使用 SpriteSheet（推荐）
   └─ 启用 LOD 系统（远处降低分辨率）

3. 跨平台兼容性
   ├─ 优先使用 SpriteSheet 而非 TextureArray
   ├─ 测试 Quest 和 PC 平台
   └─ 监控 Draw Call 数量

4. 视觉效果
   ├─ Light Volume：启用以获得环境光影响
   ├─ 法线贴图：仅在高端设备启用
   └─ 距离控制：根据场景空间调整 MaxDistance
```

---

## 常见问题

### Q1: 怎样选择 Shader 版本？

**A**: 按优先级：

1. **首选 SpriteSheet 版本** (`@Luna/Gaze GIF SpriteSheet`)
   - 兼容性最强
   - 适合 99% 的场景

2. **选择 TextureArray 版本** (`@Luna/Gaze Gif`)
   - 条件：需要极多帧数 (>256)
   - 条件：内存充足
   - 条件：确认平台支持

3. **选择 Cutout 版本** (`@Luna/Gaze GIF SpriteSheet Cutout`)
   - 条件：图像完全不透明
   - 条件：需要最高性能

### Q2: 转换后 SpriteSheet 的分辨率多少合适？

**A**: 取决于用途：

```
移动/Quest:      512×512 或 1024×1024
VRChat 推荐:     1024×1024 到 2048×2048
高端PC:          2048×2048 到 4096×4096
多个动画共享:    尽量保持一致以利于优化
```

### Q3: 凝视追踪看起来不自然，怎么办？

**A**: 调整以下参数：

```
1. 检查 _SingleAxisGaze
   └─ 人物应该用 Y (2) 轴，其他用 All (0)

2. 调整 _WeakenDistanceGaze
   └─ 增加此值会使远处效果变弱，更自然

3. 使用 _ExtraRotX/Y/Z 微调朝向
   └─ 小幅调整 ±5-15° 通常足够

4. 启用法线贴图增加细节感
   └─ 会让光照反应更逼真
```

### Q4: GIF 转 SpriteSheet 的"高性能"和"高质量"有什么区别？

**A**:

```
高性能模式:
├─ 最大纹理: 2048×2048
├─ 用途: Quest、多人场景、性能敏感
└─ 优点: 加载快、内存少

高质量模式:
├─ 最大纹理: 4096×4096
├─ 用途: 桌面、高端VR
└─ 优点: 细节保留、视觉质量最高

选择原则:
└─ 始终使用最低能满足视觉需求的分辨率
```

### Q5: 法线贴图的 _NormalStrength 应该设多少？

**A**:

```
负值 (-1 ~ -5):
└─ 反转法线，不推荐

0 ~ 1 (推荐):
├─ 0: 没有法线效果
├─ 0.5: 微妙高光（自然）
└─ 1.0: 标准高光（推荐）

1 ~ 5 (强化):
├─ 2-3: 明显凹凸感
└─ 4-5: 极端夸张效果
```

### Q6: 在 VRChat 中看起来闪烁，怎么办？

**A**: 这是透明度伪影，启用 `_FixTransp = ON`

或者：
```
1. 检查 GIF 的 Alpha 通道
   └─ 确保边缘不是半透明的噪点

2. 调整 Shader 的 Blend Mode
   └─ 某些场景可能需要修改混合模式

3. 检查相机深度设置
   └─ 确保 ZTest = LEqual
```

### Q7: Draw Call 太多，怎么优化？

**A**: 使用 Material Manager：

```
1. 打开 Tools → @Luna → Gaze Shader Material Manager
2. 点击 [自动优化全部]
3. 系统会为相同纹理创建共享材质实例
4. 自动替换场景中重复的材质

结果: Draw Call 显著减少
```

### Q8: 在 Inspector 中改参数后没反应？

**A**: 检查以下几点：

```
1. Shader 未完全加载
   └─ 等待片刻重启编辑器

2. 材质未被正确应用
   └─ 在 Renderer 中重新应用

3. 之前设置有缓存
   └─ 删除 Library/ShaderCache 文件夹

4. 在 Play Mode 中修改参数
   └─ Play Mode 结束后改动会丢失
   └─ 应该在编辑模式下修改
```

---

## 技术细节

### Shader 架构

```
Gaze Shader 核心结构:

Vertex Shader:
├─ 获取世界坐标和摄像机信息
├─ 计算凝视旋转矩阵
├─ 生成对象种子（用于随机性）
├─ 应用缩放/旋转变化
└─ 输出变换后的顶点

Fragment Shader:
├─ 计算当前帧索引
│  ├─ 基于时间和 FPS
│  ├─ 支持 PlayMode 逻辑
│  └─ 考虑距离速度控制
├─ 采样纹理
│  ├─ TextureArray 版本: UNITY_SAMPLE_TEX2DARRAY
│  └─ SpriteSheet 版本: 计算 UV 并采样
├─ 计算光照（可选）
│  ├─ 法线贴图采样
│  ├─ 镜面反射计算
│  └─ Light Volume 集成
└─ 输出最终颜色
```

### 关键算法

#### 1. 凝视追踪 (Gaze Tracking)

```glsl
// 简化版本
float3 cameraDir = normalize(cameraPos - worldPos);
float3x3 rotMatrix = RotateTowardsCamera(cameraDir);
float3 finalPos = mul(rotMatrix, vertexPos);
```

**特点**:
- 使用旋转矩阵而非欧拉角，避免万向锁
- 支持多轴追踪选择
- 距离衰减平滑过渡

#### 2. 距离速度控制

```glsl
float distance = length(worldPos - cameraPos);
float speedMultiplier;

if (SpeedChangeMode == 1) {
    // Accelerate: 越近越快
    speedMultiplier = 1.0 + (1 - dist / maxDist) * rate;
} else if (SpeedChangeMode == 2) {
    // Decelerate: 越近越慢
    speedMultiplier = 1.0 - (1 - dist / maxDist) * rate;
}

float adjustedFPS = fps * speedMultiplier;
```

#### 3. 帧计算 (Frame Calculation)

```glsl
float elapsedTime = time - startTime;
float frameIndex = floor(elapsedTime * adjustedFPS) % totalFrames;

// 支持不同 PlayMode
if (playMode == ONCE) {
    frameIndex = min(frameIndex, totalFrames - 1);
} else if (playMode == RANDOM) {
    // 计算随机片段
    frameIndex = RandomSegment(seed);
} else if (playMode == MANUAL) {
    frameIndex = manualFrame;
}
```

#### 4. SpriteSheet UV 计算

```glsl
int column = int(frameIndex) % columns;
int row = int(frameIndex) / columns;

float u = (column + uv.x) / columns;
float v = (row + uv.y) / rows;

float2 spriteSheetUV = float2(u, v);
```

### 性能考量

```
TextureArray 版本:
├─ 优点: 最高性能、最优内存利用
├─ 缺点: TextureArray 可能不被所有平台支持
└─ 性能: 最优 (100% 基准)

SpriteSheet 版本:
├─ 优点: 兼容性强、转换简单
├─ 缺点: 单张大纹理可能占用更多内存
└─ 性能: 很好 (95% 相对)

Cutout 版本:
├─ 优点: 性能最高（无 Alpha 混合）
├─ 缺点: 只支持不透明图像
└─ 性能: 最优 (105% 相对，最快)
```

### 兼容性

```
平台兼容性:

TextureArray (Gaze Gif):
├─ Windows/Mac: ✅ 完整支持
├─ WebGL: ❌ 不支持
├─ Android/Quest: ⚠️ 有限支持
└─ iOS: ❌ 不支持

SpriteSheet (推荐):
├─ Windows/Mac: ✅ 完整支持
├─ WebGL: ✅ 完整支持
├─ Android/Quest: ✅ 完整支持
└─ iOS: ✅ 完整支持

Cutout:
├─ 所有平台: ✅ 完整支持
└─ 最高兼容性
```

---

## 附录

### 快速参考

#### 参数预设

**默认设置（推荐新手）**:
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

**高性能设置**:
```
SpriteSheet 分辨率: 512×512
FPS: 12
LightingEffect: OFF
UseNormalMap: OFF
Gaze: Y轴 (单轴比全向性能好)
```

**高质量设置**:
```
SpriteSheet 分辨率: 2048×2048 或 TextureArray
FPS: 24
LightingEffect: ON
UseNormalMap: ON
NormalStrength: 1.0
SpecularSharpness: 30
```

### 文件位置速查

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
│  ├─ Gaze Gif.shader (TextureArray 版)
│  ├─ Gaze GIF SpriteSheet.shader (推荐)
│  ├─ Gaze GIF SpriteSheet Cutout.shader (不透明版)
│  └─ LightVolumes.cginc (VRC 光体积)
│
├─ Scripts/
│  └─ UdonGazeGifTrigger.cs (VRC Udon 脚本)
│
├─ logo/
│  ├─ s booth.png
│  ├─ s x.png
│  ├─ s b.png
│  ├─ @咸鱼子Luna.png
│  └─ Luna_ko.png
│
├─ Gaze_Gif_Shader_Summary_cn.md (本文档)
├─ Gaze_Gif_Shader_Summary_en.md
├─ Gaze_Gif_Shader_Summary_ja.md
└─ README.md (项目说明)


```

### 社交媒体

- **Booth**: https://xianyuzi-luna.booth.pm/
- **X (Twitter)**: https://x.com/lunabxgg
- **Bilibili**: https://space.bilibili.com/3546752913247086

---

## 更新日志

### v1.0 (2026-03-03)

**初始发布版本，包含以下内容**:

- ✅ 三种 Shader 版本（TextureArray、SpriteSheet、Cutout）
- ✅ 完整的 Editor 工具套件
- ✅ GIF 转换工具（TextureArray 和 SpriteSheet）
- ✅ 法线贴图生成器
- ✅ 材质管理器
- ✅ VRC Light Volume 集成
- ✅ UdonSharp 支持脚本
- ✅ 多语言本地化（EN/CN/JA）
- ✅ 完整的文档和示例

---

### 🛒 商业使用授权 (Commercial License)

本 GitHub 仓库开源的内容仅供**个人免费非商业使用**（例如：用于私有 VRChat Avatar，或发布免费的 VRChat World）。

### ⚠️ 商业使用限制

**严禁**将本着色器直接或间接用于任何商业营利行为，包含但不限于：

- 商用 3D 模型售卖
- 付费 Avatar 委托定制
- 付费场景搭建
- 其他商业营利用途

### 💳 商用许可证购买

若需在商用作品或付费项目中使用本系统，请前往 **BOOTH** 购买商业使用授权：

**👉 [点击此处前往 BOOTH 购买商用许可证](https://xianyuzi-luna.booth.pm/items/7555039) 👈**

---

**感谢使用 Gaze Shader！如有问题，请通过着色器界面右下角作者LOGO--@咸鱼子Luna--的社交媒体渠道联系作者。**
