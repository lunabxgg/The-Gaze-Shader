# The Gaze Shader - Complete Documentation & User Manual (English)

**Author**: @Xianyuzi Luna  
**Project**: Gaze Gif Shader gaze‑tracking system  
**Version**: 1.0  
**Last Updated**: 2026‑03‑03

---

## 📖 Table of Contents

1. [Project Overview](#project-overview)
2. [Features](#features)
3. [Shader Versions Comparison](#shader-versions-comparison)
4. [Quick Start](#quick-start)
5. [Shader Parameter Reference](#shader-parameter-reference)
6. [Editor Tools](#editor-tools)
7. [Workflows](#workflows)
8. [VRC Integration](#vrc-integration)
9. [FAQ](#faq)
10. [Technical Details](#technical-details)

---

## Project Overview

The **Gaze Shader** is an advanced animation rendering system tailored for Unity and VRChat, designed to display **gaze‑tracking GIF animations**. It can:

- 🎬 Play GIF animations in real time with flexible frame rate control
- 👁️ Automatically rotate objects to face the camera (billboard effect)
- 📏 Adjust playback speed based on distance to the camera
- 💡 Support normal maps, specular highlights, and VRC light volumes
- 🎭 Offer multiple play modes: loop, once, random, manual
- 🔀 Provide randomization options for scale and rotation

---

## Features

### Core Capabilities

| Feature | Description |
|--------|-------------|
| **GIF Playback** | Extracts frames from a GIF and plays them sequentially |
| **Gaze Tracking** | Object constantly faces the camera |
| **Distance Speed** | Playback speed changes with viewer distance |
| **Play Modes** | Loop / Once / Random / Manual |
| **Normal Mapping** | Generate/use normal maps for highlights |
| **Light Volume** | Integrates with VRChat Light Volumes |

### Advanced Options

| Option | Description |
|--------|-------------|
| **Start Frame Randomization** | Random start frame per object |
| **Rotation Variation** | Random rotation on X/Y/Z axes |
| **Scale Variation** | Random size variation |
| **Axis‑limited Gaze** | All/X/Y/Z axis only tracking |
| **Distance attenuation** | Fade gaze effect over distance |
| **Alpha fix** | Correct transparency artifacts |

---

## Shader Versions Comparison

### 1️⃣ **Gaze Gif** (`@Luna/Gaze Gif`)

- **Texture format**: Texture2DArray (preferred)
- **Use case**: precise control with plenty of memory
- **Pros**: best performance, unlimited frames, low memory per frame
- **Cons**: GIF must be converted to an array; not supported on all platforms
- **Recommended**: desktop/VR apps, high‑quality VRChat worlds

### 2️⃣ **Gaze GIF SpriteSheet** (`@Luna/Gaze GIF SpriteSheet`)

- **Texture format**: 2D spritesheet grid
- **Use case**: general purpose, highest compatibility
- **Pros**: works everywhere, easy conversion, adjustable rows/cols
- **Cons**: single large texture may use more memory, slightly slower
- **Recommended**: VRChat worlds, Quest, general production

### 3️⃣ **Gaze GIF SpriteSheet Cutout** (`@Luna/Gaze GIF SpriteSheet Cutout`)

- **Texture format**: 2D spritesheet (opaque only)
- **Use case**: fully opaque GIFs requiring maximum speed
- **Pros**: fastest performance, depth write, no alpha blending
- **Cons**: does not support semi‑transparent images
- **Recommended**: opaque GIF animations, mobile/Quest

---

## Quick Start

### Step 1: Prepare GIF

1. Obtain a GIF file (transparency supported).
2. Place it in the `Assets/` folder.
3. Set import type to **Texture** in the Inspector.

### Step 2: Create Material

#### Option A – SpriteSheet (beginner)

1. Right‑click → Create → Material.
2. Select shader `@Luna/Gaze GIF SpriteSheet`.
3. In Inspector:
   - Drag GIF into "GIF Source".
   - Click **Generate Sprite Sheet**.
   - Shader fills MainTex, Columns, Rows, TotalFrames.

#### Option B – TextureArray (advanced)

1. Create a material as above.
2. Choose shader `@Luna/Gaze Gif`.
3. Use the Tools menu:
   - Select *GIF to Texture Array*.
   - Pick a size and generate.

### Step 3: Configure Parameters

```
Basic:
├─ FPS: animation frame rate (10‑24 typical)
├─ Play Mode: Loop/Once/Random/Manual
└─ Color: tint

Gaze:
├─ Gaze: enable/disable tracking
├─ Single Axis Gaze: choose axis
└─ Weaken Distance Gaze: fade with distance

Distance Speed:
├─ Speed Change Mode
├─ Speed Change Rate
└─ Max Distance
```

### Step 4: Apply to Scene

1. Create a Quad/Plane/mesh.
2. Assign the material to its Renderer.
3. Play the scene to preview.

---

## Shader Parameter Reference

### 📦 Textures

#### Gaze Gif.shader

```
_Textures (Texture Array)
  ├─ Description: generated from GIF
  ├─ Default: black
  └─ Purpose: stores animation frames

_NormalMapArray (2DArray, optional)
  ├─ Description: matching normal maps
  ├─ Default: bump
  └─ Purpose: specular highlights
```

#### Gaze GIF SpriteSheet.shader

```
_MainTex (2D)
  ├─ Description: spritesheet image
  ├─ Default: white
  └─ Purpose: holds all frames

_Columns & _Rows (IntRange)
  ├─ Description: grid size
  ├─ Range: 1‑64
  └─ Example: 4×4 = 16 frames

_TotalFrames (IntRange)
  ├─ Description: actual frame count
  ├─ Range: 1‑4096
  └─ Can be less than rows×cols
```

### 🎬 Animation Control

```
_fps (IntRange, default 12)
  ├─ Range: 1‑60
  └─ Frame rate

_PlayMode (Enum, default Loop)
  ├─ Loop (0)
  ├─ Once (1)
  ├─ Random (2)
  └─ Manual (3)

_ManualFrame (IntRange, default 1)
  ├─ Range: 1‑100
  ├─ Used when PlayMode=Manual
  └─ 1 = first frame

_StartFrameRandomization (Float, default 0)
  ├─ Range: 0‑1
  └─ Random start offset per object

### 📏 Gaze Control

```
_Gaze (Toggle, default ON)
  ├─ Enables camera‑facing behavior
  └─ Off leaves original orientation

_SingleAxisGaze (Enum, default All)
  ├─ All (0): full tracking
  ├─ X (1)
  ├─ Y (2)
  └─ Z (3)

_WeakenDistanceGaze (Float, default 0)
  ├─ Range 0‑1
  └─ Attenuate tracking at distance

_ExtraRotX/Y/Z (Float, default 0)
  ├─ Range ‑180 to 180°
  └─ Additional orientation offset
```

### 📉 Distance Speed Control

```
_SpeedChangeMode (Enum, default Uniform)
  ├─ Uniform (0): no distance effect
  ├─ Accelerate (1): faster when close
  └─ Decelerate (2): slower when close

_SpeedChangeRate (Float, default 1)
  ├─ Range 1‑10
  └─ Strength of distance effect

_MaxDistance (Float, default 15)
  ├─ Range 5‑50m
  └─ Reference distance for speed changes

_SpeedFromZero (Toggle, default OFF)
  ├─ On: speed can reach zero at close range
  └─ Off: minimum base FPS maintained
```

### 🎨 Color & Scale

```
_Color (Color, default White)
  ├─ Tints the texture
  └─ Alpha adjusts overall transparency

_ScaleX / _ScaleY (Float, default 1)
  ├─ UV texture scaling
  └─ Enlarges or shrinks displayed area

_UVx / _UVy (Float, default 0)
  ├─ UV offset
  └─ Shifts texture position

_Brightness (Float, default 1.0)
  ├─ Range 0‑5
  └─ Adjust overall brightness
```

### 🎭 Random Variation

```
_ScaleVariation (Float, default 1)
  ├─ Range 1‑2
  └─ Random scale multiplier

_RandomRotXVariation (Float, default 0)
  ├─ Range 0‑1
  └─ Random X rotation multiplier

_RandomRotYVariation (Float, default 0)
  └─ Random Y rotation multiplier

_RandomRotZVariation (Float, default 0)
  └─ Random Z rotation multiplier
```

### 💡 Lighting Effect

```
_LightingEffect (Toggle, default ON)
  ├─ Enables lighting calculations
  └─ Off displays texture only

_Brightness (Float, default 1.0)
  ├─ Range 0‑5
  └─ Adjust lighting brightness

_UseNormalMap (Toggle, default OFF)
  ├─ Enables normal map support
  └─ Requires _NormalMapArray or _NormalMap

_NormalStrength (Float, default 1)
  ├─ Range ‑5 to 5
  └─ Strength of normal map effect

_SpecularSharpness (Float, default 20)
  ├─ Range 1‑100
  └─ Sharpness of specular highlights

_SpecularBrightness (Float, default 0.5)
  ├─ Range 0‑1
  └─ Brightness of specular highlights
```

### 🌈 VRC Features

```
_UseLightVolume (Toggle, default ON)
  ├─ Enables VRChat Light Volume support
  └─ Object responds to light volumes

_LightVolumeIntensity (Float, default 1.0)
  ├─ Range 0‑2
  └─ Intensity of volume influence
```

### ☑️ Display Fix

```
_BackfaceCulling (Toggle, default ON)
  ├─ Enables backface culling
  └─ Off displays back faces

_FixTransp (Toggle, default OFF)
  ├─ Fix transparency artifacts
  └─ Enable if edges flicker
```

---

## Editor Tools

### 🎛️ Gaze Gif Shader GUI

Custom inspector panel for the shader.

#### Main Sections

1. **Author Info** – social links (Booth, X, Bilibili)
2. **Header** – Material Manager button, shader switcher
3. **Resource Section**
   - SpriteSheet version: [Generate Sprite Sheet]
   - TextureArray version: Tools menu for conversions
4. **Effect Section** – play mode, FPS, random start, speed settings
5. **Fixed Variation** – extra rotation, scale, UV, etc.
6. **Random Variation** – scale/rotation randomness
7. **Advanced** – normal map, specular, culling options

---

### 📁 Material Instance Manager

Open via `Tools → @Luna → Gaze Shader Material Manager`.

#### Capabilities

```
Left panel:
├─ Language selector (EN/CN/JA)
├─ Quick actions
│  ├─ [Auto Optimize All]
│  └─ [Refresh Scan]
└─ Grouped materials by texture or null

Right panel actions:
├─ [Create Shared Instance]
├─ [Optimize Scene]
└─ [Cleanup Unused]
```

#### Workflow

1. Open manager
2. Click [Refresh Scan]
3. Review groups and suggestions
4. Use auto or manual optimization
5. Close when done

---

### 🎨 Normal Map Generator

Generates normal maps automatically.

#### Usage

1. Enable `_UseNormalMap` on the material.
2. Click **[Generate Normal Map]** in the inspector.
3. Adjust parameters:
   - `_NormalStrength` (‑5 to 5)
   - `_SpecularSharpness` (1‑100)
   - `_SpecularBrightness` (0‑1)

#### How it works

Computes brightness differences between neighboring pixels
and generates normals for bump mapping and specular highlights.

---

### 🔄 GIF Conversion Tools

#### GIF to SpriteSheet Converter

- Fast, compatible with all platforms.
- Two quality modes:
  - High performance: max 2048x2048 atlas
  - High quality: max 4096x4096 atlas

**Procedure**:
1. Select GIF in material
2. Click [Generate Sprite Sheet]
3. Choose quality mode
4. System computes optimal grid and generates atlas

| Mode | Max Size | Performance | Use Case |
|------|----------|-------------|----------|
| High perf | 2048² | Best | Mobile/VRChat large scenes |
| High quality | 4096² | Good | Desktop/high‑end |

#### GIF to Texture Array Converter

- Best performance, unlimited frames
- Limited compatibility

**Procedure**:
1. Drag GIF into material
2. Choose resolution (256‑2048)
3. Click [Convert]
4. Generated Texture2DArray is saved

Resolution guide:
```
256: tiny/high perf
512: recommended balance
1024: high quality
2048: highest quality
```

---

## Workflows

### Workflow 1 – Rapid Prototyping

1. Prepare GIF (<256² recommended)
2. Create SpriteSheet material
3. Convert GIF to sheet
4. Set FPS=12‑24, Color, Gaze=ON
5. Apply to quad and preview

### Workflow 2 – VRChat World Optimization

1. Plan resources and categorize
2. Batch convert using high perf mode (<4096)
3. Use Material Manager to share materials
4. Enable `_UseLightVolume` and adjust intensity
5. Test on Quest, monitor draw calls and memory

### Workflow 3 – Advanced Visuals

1. Start with SpriteSheet
2. Fine‑tune gaze parameters and extra rot
3. Enable normal map and set strength
4. Configure specular settings
5. Adjust distance speed control
6. Add random variations and duplicate

---

## VRC Integration

### Light Volume Support

Light Volume allows environment lighting in VRChat.
Enable via `_UseLightVolume = ON` and set intensity.

### UdonSharp Script

File: `UdonGazeGifTrigger.cs`

Used to synchronize animation start time across players.

**Usage**:
1. Add script to object with Gaze material
2. OnEnable sets `_StartTime` to `Time.timeSinceLevelLoad`
3. Shader uses that to compute current frame

Example:
```csharp
private void OnEnable() {
    meshRenderer = GetComponent<Renderer>();
    if (meshRenderer?.material != null)
        meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
}
```

### Best Practices for VRChat

```
- Minimize network updates; use AnimatorController for material params
- Keep fewer than 50 Gaze Shader objects in a scene
- Prefer SpriteSheet for cross‑platform compatibility
- Enable LOD and reduce resolution at distance
- Test on Quest and PC builds
- Activate Light Volume for realistic lighting
- Use normal maps only on high‑end devices
- Set MaxDistance according to scene scale
```

---

## FAQ

### Q1: Which shader version should I choose?

**A**: Priority order:

1. **SpriteSheet** (`@Luna/Gaze GIF SpriteSheet`)
   - Highest compatibility, fits 99% of use cases
2. **TextureArray** (`@Luna/Gaze Gif`)
   - Use when you need huge frame counts or top performance
   - Ensure the target platform supports Texture2DArray
3. **Cutout** (`@Luna/Gaze GIF SpriteSheet Cutout`)
   - Only for fully opaque imagery and maximum speed

### Q2: What spritesheet resolution should I use?

```
Mobile/Quest: 512×512 or 1024×1024
VRChat recommended: 1024×1024 to 2048×2048
High‑end PC: 2048×2048 to 4096×4096
Keep resolutions consistent across multiple assets for better batching
```

### Q3: Gaze tracking looks unnatural—what then?

```
1. Check _SingleAxisGaze setting
   └‑ Y axis is often better for standing characters
2. Raise _WeakenDistanceGaze to soften distant effect
3. Use _ExtraRotX/Y/Z for fine orientation tweaks (±5‑15°)
4. Enable normal mapping to add surface detail
```

### Q4: What’s the difference between high‑perf and high‑quality modes in conversion?

```
High perf:
├‑ Max atlas 2048×2048
├‑ Use on Quest, crowded scenes
└‑ Fast load, low memory

High quality:
├‑ Max atlas 4096×4096
├‑ Use on desktop/high‑end
└‑ Best detail
```

Use the lowest resolution that meets your visual needs.

### Q5: How strong should _NormalStrength be?

```
Negative (-1 to -5): reverse normals (not recommended)
0–1: subtle to standard highlights (1 is default)
2–3: pronounced bump effect
4–5: extreme exaggerated normals
```

### Q6: There’s flickering in VRChat—help?

Enable `_FixTransp = ON` for transparency artifacts.
Alternatively:
```
1. Check GIF alpha channel for noisy edges
2. Adjust shader blend mode if needed
3. Ensure camera uses ZTest LEqual
```

### Q7: Draw calls too high—how to optimize?

Use Material Manager:
```
1. Tools → @Luna → Gaze Shader Material Manager
2. Click [Auto Optimize All]
3. Shared instances for identical textures are created
4. Scene materials are replaced automatically
```
Draw calls drop significantly.

### Q8: Changes in Inspector don’t apply?

```
1. Shader may still be compiling—wait or restart editor
2. Material not actually assigned—re‑apply to renderer
3. Cache issue—delete Library/ShaderCache
4. Don’t modify parameters in Play Mode (changes are discarded)
```

---

## Technical Details

### Shader Architecture

```
Vertex Shader:
├‑ gather world/camera data
├‑ compute gaze rotation matrix
├‑ generate object seed for randomness
├‑ apply scaling/rotation variations
└‑ output transformed vertex

Fragment Shader:
├‑ compute current frame index
│   ├‑ based on time and FPS
│   ├‑ includes play mode logic
│   └‑ factors distance speed control
├‑ sample texture
│   ├‑ TextureArray: UNITY_SAMPLE_TEX2DARRAY
│   └‑ SpriteSheet: UV math
├‑ lighting calculations (optional)
│   ├‑ normal map sampling
│   ├‑ specular reflection
│   └‑ light volume blending
└‑ output final color
```

### Key Algorithms

#### 1. Gaze Tracking

```glsl
float3 cameraDir = normalize(cameraPos - worldPos);
float3x3 rotMatrix = RotateTowardsCamera(cameraDir);
float3 finalPos = mul(rotMatrix, vertexPos);
```

Uses rotation matrices to avoid gimbal lock. Supports axis constraints and distance attenuation.

#### 2. Distance Speed Control

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

#### 3. Frame Calculation

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

#### 4. SpriteSheet UV Computation

```glsl
int column = int(frameIndex) % columns;
int row = int(frameIndex) / columns;
float u = (column + uv.x) / columns;
float v = (row + uv.y) / rows;
float2 spriteSheetUV = float2(u, v);
```

### Performance Considerations

```
TextureArray:
├‑ pros: best perf, memory efficient
├‑ cons: limited platform support
└‑ perf baseline: 100%

SpriteSheet:
├‑ pros: wide compatibility
├‑ cons: larger single texture
└‑ perf ~95%

Cutout:
├‑ pros: fastest (no alpha)
├‑ cons: opaque only
└‑ perf ~105%
```

### Compatibility

```
TextureArray (Gaze Gif):
├‑ Windows/Mac: ✅
├‑ WebGL: ❌
├‑ Android/Quest: ⚠️
└‑ iOS: ❌

SpriteSheet:
├‑ Windows/Mac/WebGL/Android/Quest/iOS: ✅

Cutout:
├‑ All platforms: ✅ (highest compatibility)
```

---

## Appendix

### Quick Presets

**Default (beginner)**:
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

**High‑performance**:
```
SpriteSheet res: 512×512
FPS: 12
LightingEffect: OFF
UseNormalMap: OFF
Gaze: Y axis (single axis is cheaper)
```

**High‑quality**:
```
SpriteSheet res: 2048×2048 or TextureArray
FPS: 24
LightingEffect: ON
UseNormalMap: ON
NormalStrength: 1.0
SpecularSharpness: 30
```

### File Locations

```
Assets/@Luna/gaze gif shader/
├─ Editor/ (all editor scripts + this doc)
├─ Shader/ (three shader files + light volume include)
├─ Scripts/ (UdonGazeGifTrigger.cs)
├─ logo/ (social icons)
└─ README.md
```

### Social

- Booth: https://xianyuzi-luna.booth.pm/
- X (Twitter): https://x.com/lunabxgg
- Bilibili: https://space.bilibili.com/3546752913247086

---

## Changelog

### v1.0 (2026-03-03)

- Initial release with full feature set, editor tools, converters, normal map generator, material manager, VRC integration, Udon support, localization, and documentation.

---

### 🛒 Commercial License

This GitHub repository's open-source content is for **personal, non-commercial use only** (e.g., private VRChat avatars or free VRChat worlds).

### ⚠️ Commercial Restrictions

Commercial use of this shader—direct or indirect—is strictly prohibited. Examples include:

- Paid 3D model sales
- Commissioned avatars for money
- Paid world/scene construction
- Any other revenue-generating activities

### 💳 Buying a Commercial License

If you need to use this system in commercial or paid projects, please purchase a commercial license on **BOOTH**:

**👉 [Purchase Commercial License](https://xianyuzi-luna.booth.pm/items/7555039) 👈**

---

**Thank you for using Gaze Shader! For questions contact the author via the social links on the shader's lower-right logo.**

    meshRenderer = GetComponent<Renderer>();
    if(meshRenderer?.material != null)
        meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
}
```

### Best Practices for VRChat

```
- Minimize network updates; use AnimatorController
- Keep <50 Gaze Shader objects per scene
- Prefer SpriteSheet for compatibility
- Enable LOD for distant objects
- Test on Quest & PC platforms
- Light Volume: enabled for realistic lighting
- Normal maps: only on high‑end devices
- Adjust MaxDistance based on scene size
```

---

## FAQ

### Q1: Which shader version should I choose?
... (continue translating remaining FAQ entries)

_StartFrameRandomization (Float, default 0)
  ├─ Range: 0‑1
  └─ Random start offset per object
```

### 📏 Gaze Control

```
_Gaze (Toggle, default ON)
  ├─ Enables camera‑facing behavior
  └─ Off leaves original orientation

_SingleAxisGaze (Enum, default All)
  ├─ All (0): full tracking
  ├─ X (1)
  ├─ Y (2)
  └─ Z (3)

_WeakenDistanceGaze (Float, default 0)
  ├─ Range 0‑1
  └─ Attenuate tracking at distance

_ExtraRotX/Y/Z (Float, default 0)
  ├─ Range ‑180 to 180°
  └─ Additional orientation offset
```

(Continue translating rest of sections similarly...)