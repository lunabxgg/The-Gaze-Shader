# The Gaze Shader - Complete Guide & User Manual

**Author**: @Xianyuzi Luna (@咸鱼子Luna)  
**Project**: Gaze Gif Shader Tracking System  
**Version**: 1.0  
**Update Date**: 2026-03-03

---

## 📖 Table of Contents

1. [Project Overview](#project-overview)
2. [Features](#features)
3. [Comparison of Three Shaders](#comparison-of-three-shaders)
4. [Quick Start](#quick-start)
5. [Shader Parameters Explained](#shader-parameters-explained)
6. [Editor Tools Usage](#editor-tools-usage)
7. [Workflow Guide](#workflow-guide)
8. [VRC Integration Guide](#vrc-integration-guide)
9. [FAQ](#faq)
10. [Technical Details](#technical-details)

---

## Project Overview

**Gaze Shader** is an advanced animation rendering system specifically designed for VRChat and Unity, primarily used to achieve **gaze-tracking GIF animation** effects. This system can:

- 🎬 **Load and play GIF animations in real-time**, supporting flexible framerate control
- 👁️ **Automatically track the camera**, keeping the animated object always facing the observer
- 📏 **Distance-sensing control**, dynamically adjusting playback speed based on the distance to the camera
- 💡 **Advanced lighting effects**, including normal maps, specular reflection, and VRC Light Volume integration
- 🎭 **Multiple play modes**, supporting Loop, Once, Random, and Manual frame control
- 🔀 **Random variation support**, providing randomization options for scaling, rotation, and other parameters

---

## Features

### Core Features

| Feature | Description |
|------|------|
| **GIF Animation Playback** | Automatically extracts and plays frame sequences from GIF files |
| **Gaze Tracking** | Objects automatically rotate to face the camera (Billboard effect) |
| **Distance Sensing** | Adjusts animation speed based on the distance to the camera |
| **Play Modes** | Four modes available: Loop / Once / Random / Manual |
| **Normal Maps** | Supports generating and using normal maps for specular highlights |
| **Lighting Integration** | Integrates with VRC Light Volumes |

### Advanced Features

| Feature | Description |
|------|------|
| **Start Frame Randomization** | Randomly selects a starting frame for each object |
| **Rotation Variation** | Supports random rotation variations on the X/Y/Z axes |
| **Scale Variation** | Object size can vary randomly |
| **Multi-axis Gaze** | Supports omnidirectional, X-axis, Y-axis, and Z-axis single-axis tracking |
| **Distance Attenuation** | Weakens the gaze effect at long distances |
| **Alpha Fix** | Fixes transparency artifacts |

---

## Comparison of Three Shaders

### 1️⃣ **Gaze Gif** (@Luna/Gaze Gif)

- **Texture Format**: Texture2DArray (Recommended)
- **Use Case**: When precise control over frame sequences is needed and memory is sufficient.
- **Pros**:
  - Highest performance efficiency
  - Supports the maximum number of frames
  - Low memory footprint per frame
- **Cons**:
  - Requires converting GIFs to TextureArray
  - TextureArray may not be supported on some platforms
- **Recommended Scenarios**: High-quality VRChat worlds, desktop applications, VR applications

### 2️⃣ **Gaze GIF SpriteSheet** (@Luna/Gaze GIF SpriteSheet)

- **Texture Format**: 2D SpriteSheet (Grid layout)
- **Use Case**: General scenarios, best compatibility.
- **Pros**:
  - **Strongest compatibility**, supports all platforms
  - Quick and easy conversion
  - Easy to adjust columns, rows, and frame counts
- **Cons**:
  - A single large texture might consume more memory
  - Performance is slightly lower than TextureArray
- **Recommended Scenarios**: VRChat worlds, Quest platform, general production environments

### 3️⃣ **Gaze GIF SpriteSheet Cutout** (@Luna/Gaze GIF SpriteSheet Cutout)

- **Texture Format**: 2D SpriteSheet (Opaque version)
- **Use Case**: Completely opaque GIFs requiring maximum performance.
- **Pros**:
  - **Maximum performance**
  - Supports depth writing
  - No Alpha blending calculations required
- **Cons**:
  - **Only applicable to completely opaque images**
  - Cannot display translucent effects
- **Recommended Scenarios**: Opaque GIF animations, performance-sensitive mobile platforms

---

## Quick Start

### Step 1: Prepare the GIF File

1. Prepare your GIF file (transparent backgrounds are supported)
2. Place the GIF into the `Assets/` directory
3. Set the GIF import type to **Texture** in the Inspector

### Step 2: Create Material

#### Method A: Using SpriteSheet (Recommended for Beginners)

1. **Create Material**: Right-click → Create → Material
2. **Set Shader**: Select `@Luna/Gaze GIF SpriteSheet`
3. **In the Inspector**:
   - Drag and drop the GIF file into the "GIF Source" field
   - Click the **"Generate SpriteSheet" (生成精灵图)** button
   - The shader will automatically fill in MainTex, Columns, Rows, and TotalFrames

#### Method B: Using TextureArray (Advanced Users)

1. **Create Material**: Right-click → Create → Material
2. **Set Shader**: Select `@Luna/Gaze Gif`
3. **Convert GIF**:
   - Find the Tool menu in the Inspector
   - Select "GIF to Texture Array"
   - Set the resolution and generate

### Step 3: Configure Parameters

```
Basic Settings (Required):
├─ FPS (Base FPS): Set animation framerate, typical values 10-24
├─ Play Mode: Select play mode (Loop/Once/Random/Manual)
└─ Color: Adjust color tint

Gaze Tracking:
├─ Gaze: Enable/Disable tracking
├─ Single Axis Gaze: Select tracking axis
└─ Weaken Distance Gaze: Weaken tracking at far distances

Distance Speed Control:
├─ Speed Change Mode: Select speed change mode
├─ Speed Change Rate: Adjust intensity of the change
└─ Max Distance: Set the affected distance range
```

### Step 4: Apply to Scene

1. Create a Quad, Plane, or 3D model
2. Apply the created material to the Renderer
3. Preview the effect in Play Mode

---

## Shader Parameters Explained

### 📦 Textures

#### Gaze Gif.shader

```
_Textures (Texture Array)
  ├─ Description: Texture2DArray obtained from GIF conversion
  ├─ Default: black
  └─ Usage: Stores all animation frames

_NormalMapArray (2DArray, Optional)
  ├─ Description: Corresponding normal map array
  ├─ Default: bump
  └─ Usage: Achieves specular and highlight effects
```

#### Gaze GIF SpriteSheet.shader

```
_MainTex (2D)
  ├─ Description: Combined SpriteSheet texture
  ├─ Default: white
  └─ Usage: Grid containing all animation frames

_Columns & _Rows (IntRange)
  ├─ Description: Number of columns and rows in the SpriteSheet
  ├─ Range: 1-64
  └─ Example: 4 Columns × 4 Rows = 16 frames

_TotalFrames (IntRange)
  ├─ Description: Total valid frames
  ├─ Range: 1-4096
  └─ Description: Can be less than Rows × Columns to use partial frames
```

### 🎬 Animation Control

```
_fps (IntRange, Default: 12)
  ├─ Range: 1-60
  └─ Description: Animation playback framerate
  
_PlayMode (Enum, Default: Loop)
  ├─ Loop (0): Play continuously in a loop
  ├─ Once (1): Play once and stop
  ├─ Random (2): Randomly loop different segments
  └─ Manual (3): Manually specify a frame

_ManualFrame (IntRange, Default: 1)
  ├─ Range: 1-100
  ├─ Usage: Specifies the frame number to display when PlayMode=Manual
  └─ Description: A value of 1 means the first frame

_StartFrameRandomization (Float, Default: 0)
  ├─ Range: 0-1
  ├─ Description: Degree of start frame randomization per object
  └─ Example: 0.5 = Each object randomly selects 50% of the starting frame positions
```

### 📏 Gaze Control

```
_Gaze (Toggle, Default: ON)
  ├─ ON: Object faces the camera
  └─ OFF: Object maintains its original orientation

_SingleAxisGaze (Enum, Default: All)
  ├─ All (0): Omnidirectional tracking (most realistic)
  ├─ X (1): Track only on the X-axis
  ├─ Y (2): Track only on the Y-axis (often used for standing characters)
  └─ Z (3): Track only on the Z-axis

_WeakenDistanceGaze (Float, Default: 0)
  ├─ Range: 0-1
  ├─ Description: Weakens the tracking effect when further from the camera
  └─ Example: 0.5 = Tracking intensity halved at the maximum distance

_ExtraRotX/Y/Z (Float, Default: 0)
  ├─ Range: -180 ~ 180°
  ├─ Description: Extra rotational offset (superimposed on gaze rotation)
  └─ Usage: Adjusts gaze direction or corrects object orientation
```

### 📉 Distance Speed Control

```
_SpeedChangeMode (Enum, Default: Uniform)
  ├─ Uniform (0): Distance does not affect speed
  ├─ Accelerate (1): Faster when closer
  └─ Decelerate (2): Slower when closer

_SpeedChangeRate (Float, Default: 1)
  ├─ Range: 1-10
  ├─ Description: Intensity of distance's effect on speed
  └─ Example: Larger values cause more drastic speed changes

_MaxDistance (Float, Default: 15)
  ├─ Range: 5-50 Meters
  ├─ Description: Reference distance for speed control
  └─ Description: Speed stops accelerating/decelerating beyond this distance

_SpeedFromZero (Toggle, Default: OFF)
  ├─ ON: Speed can reach 0 at the closest distance
  └─ OFF: Maintains base FPS at the closest distance
```

### 🎨 Color & Scale

```
_Color (Color, Default: White)
  ├─ Description: Tints the texture
  └─ Alpha: Affects overall transparency

_ScaleX / _ScaleY (Float, Default: 1)
  ├─ Description: UV texture scale
  └─ Usage: Zooms in/out of the displayed texture portion

_UVx / _UVy (Float, Default: 0)
  ├─ Description: UV offset
  └─ Usage: Pans the texture display position

_Brightness (Float, Default: 1.0)
  ├─ Range: 0-5
  └─ Description: Adjusts overall brightness
```

### 🎭 Random Variation

```
_ScaleVariation (Float, Default: 1)
  ├─ Range: 1-2
  ├─ Description: Amplitude of random scale variation for objects
  └─ Example: 1.5 = Scale can randomize between 1x and 1.5x

_RandomRotXVariation (Float, Default: 0)
  ├─ Range: 0-1
  ├─ Description: Degree of X-axis rotation randomization
  └─ 1.0 = Can randomly rotate ±180°

_RandomRotYVariation (Float, Default: 0)
  ├─ Range: 0-1
  └─ Description: Degree of Y-axis rotation randomization

_RandomRotZVariation (Float, Default: 0)
  ├─ Range: 0-1
  └─ Description: Degree of Z-axis rotation randomization
```

### 💡 Lighting Effect

```
_LightingEffect (Toggle, Default: ON)
  ├─ ON: Enables lighting calculations
  └─ OFF: Displays only the texture itself

_Brightness (Float, Default: 1.0)
  ├─ Range: 0-5
  └─ Description: Adjusts lighting brightness

_UseNormalMap (Toggle, Default: OFF)
  ├─ Description: Enables normal map support
  └─ Dependency: Requires _NormalMapArray or _NormalMap to be set

_NormalStrength (Float, Default: 1)
  ├─ Range: -5 ~ 5
  ├─ Description: Normal map strength
  └─ Negative values invert normal direction

_SpecularSharpness (Float, Default: 20)
  ├─ Range: 1-100
  ├─ Description: Sharpness of the specular reflection
  └─ Higher values mean sharper, more concentrated highlights

_SpecularBrightness (Float, Default: 0.5)
  ├─ Range: 0-1
  └─ Description: Brightness of the specular reflection
```

### 🌈 VRC Features

```
_UseLightVolume (Toggle, Default: ON)
  ├─ Description: Enables VRC Light Volume support
  └─ Usage: Allows object to be affected by Light Volumes in VRC worlds

_LightVolumeIntensity (Float, Default: 1.0)
  ├─ Range: 0-2
  └─ Description: Intensity of the Light Volume's effect
```

### ☑️ Display Fix

```
_BackfaceCulling (Toggle, Default: ON)
  ├─ Description: Enables backface culling
  └─ Turning this OFF displays the back of the object

_FixTransp (Toggle, Default: OFF)
  ├─ Description: Fixes transparency artifacts
  └─ Enable this if you see flickering edges
```

---

## Editor Tools Usage

### 🎛️ Gaze Gif Shader GUI

This is the custom editor panel for the Shader.

#### Main Functional Areas

**1. Author Information Area**
- Displays Luna's social media links
- Booth, X (Twitter), Bilibili

**2. Header Area**
- Material Manager Button: Opens the material manager
- Quick Shader Switching (Gaze Gif / SpriteSheet / Cutout)
- Instant editing of existing materials

**3. Resource Section**

For the SpriteSheet version:
```
[Generate SpriteSheet] Button
  ├─ Input: GIF file
  ├─ Process: Automatically converts to SpriteSheet
  └─ Output: MainTex, Columns, Rows, TotalFrames
```

For the TextureArray version:
```
Tool Menu
  ├─ GIF to Texture Array: Converts GIF
  └─ GIF to SpriteSheet: Converts to SpriteSheet
```

**4. Effect Section**

- Play Mode selection
- FPS adjustment
- Start frame randomization
- Speed change mode configuration

**5. Fixed Variation**

- Extra Rotation (X/Y/Z)
- Scale, UV Offset, etc.

**6. Random Variation**

- Scale Variation
- Random Rotation Variation

**7. Advanced Section**

- Normal map settings
- Specular reflection parameters
- Backface culling options

---

### 📁 Material Instance Manager

Open: `Tools → @Luna → Gaze Shader Material Manager`

#### Features

```
Left Panel:
├─ Language Selection (EN/CN/JA)
├─ Quick Action Buttons
│  ├─ [Auto Optimize All] - Batch optimizes materials in the scene
│  └─ [Refresh Scan] - Rescans scene materials
└─ Material Group Display
   ├─ Grouped by texture
   └─ Materials with no texture assigned

Right Panel Operations:
├─ [Create Shared Instance] - Creates shared materials for identical textures
├─ [Optimize Scene] - Replaces with shared material instances
└─ [Clean Unused] - Deletes unused materials in the scene
```

#### Workflow

```
1. Open Material Manager
2. Click [Refresh Scan] to scan all materials
3. Review material groups and optimization suggestions
4. Manually optimize or click [Auto Optimize All]
5. Close the window and verify results
```

---

### 🎨 Normal Map Generator

Automatically generates normal maps.

#### Usage Steps

1. **Enable in Material**:
   - Set `_UseNormalMap` = ON

2. **Auto Generate**:
   - Click the **[Generate Normal Map]** button in the Inspector
   - The system will automatically generate a corresponding normal map based on the Texture

3. **Adjust Parameters**:
   - `_NormalStrength`: Normal strength (Range -5~5)
   - `_SpecularSharpness`: Specular sharpness (Range 1-100)
   - `_SpecularBrightness`: Specular brightness (Range 0-1)

#### Normal Map Principle

Normal maps calculate the brightness difference of adjacent pixels to generate 3D normal directions, used for:
- Achieving bumpy visual effects
- Calculating specular reflections
- Enhancing the sense of detail

---

### 🔄 GIF Converter Tools

#### GIF to SpriteSheet Converter

**Features**:
- ✅ Fast conversion
- ✅ Strongest compatibility
- ✅ Supports high-performance and high-quality modes

**Process**:
```
1. Select GIF in the material Inspector
2. Click [Generate SpriteSheet]
3. Select quality mode:
   - High Performance: Single sheet ≤2048×2048
   - High Quality: Single sheet ≤4096×4096
4. Automatically calculates grid layout (Rows × Columns)
5. Generates and applies the Sprite Sheet
```

**Quality Comparison**:
| Mode | Max Size | Performance | Use Case |
|------|---------|------|------|
| High Perf | 2048×2048 | Optimal | Mobile, VRChat Multiplayer |
| High Quality | 4096×4096 | Good | Desktop, High-end Platforms |

#### GIF to Texture Array Converter

**Features**:
- ✅ Optimal performance
- ✅ Unlimited frames
- ❌ Limited compatibility

**Process**:
```
1. Drag and drop GIF into material
2. Select resolution (256-2048)
3. Click [Convert]
4. System automatically extracts all frames
5. Generates Texture2DArray file
```

**Resolution Selection Guide**:
```
256×256:   Ultra-small, high performance
512×512:   Recommended (Balanced approach)
1024×1024: High quality, high consumption
2048×2048: Highest quality, highest consumption
```

---

## Workflow Guide

### Workflow 1: Rapid Prototyping

```
Step 1: Prepare Resources
  └─ Prepare GIF file (Recommended ≤ 256×256)

Step 2: Create Material
  ├─ Create → Material
  └─ Shader = @Luna/Gaze GIF SpriteSheet

Step 3: Convert GIF
  ├─ Drag GIF into "GIF Source"
  └─ Click [Generate SpriteSheet]

Step 4: Configure Basic Parameters
  ├─ FPS = 12-24
  ├─ Color = Adjust appropriately
  └─ Gaze = ON

Step 5: Apply to Scene
  ├─ Create Quad
  └─ Apply material to preview
```

### Workflow 2: VRChat World Optimization

```
Step 1: Resource Planning
  ├─ Evaluate how many GIF animations are needed
  └─ Categorize: Small, Medium, Large

Step 2: Batch Conversion (SpriteSheet)
  ├─ Use High Performance mode
  ├─ Keep within 4096×4096 limits
  └─ Use uniform resolution for GIFs of the same size

Step 3: Create Shared Materials
  ├─ Open Material Manager
  ├─ Click [Auto Optimize All]
  └─ Create shared instances for identical textures

Step 4: Light Volume Configuration
  ├─ Enable _UseLightVolume
  ├─ Set intensity based on environment (0.8-1.2)
  └─ Test lighting effects

Step 5: Performance Testing
  ├─ Test on Quest devices
  ├─ Monitor Draw Calls and Memory
  └─ Adjust resolution if necessary
```

### Workflow 3: Advanced Visual Effects

```
Step 1: Basic Configuration
  ├─ Select Gaze GIF SpriteSheet
  └─ Convert and apply GIF

Step 2: Fine-tune Gaze Tracking
  ├─ SingleAxisGaze: Choose axis based on object
  ├─ WeakenDistanceGaze: Adjust distance effect
  └─ ExtraRot: Correct orientation

Step 3: Enable Normal Maps
  ├─ Set _UseNormalMap = ON
  ├─ Click [Generate Normal Map]
  └─ Adjust _NormalStrength (1-2 is natural)

Step 4: Specular Reflection Configuration
  ├─ _SpecularSharpness = 20-50
  ├─ _SpecularBrightness = 0.3-0.7
  └─ Observe effects under lighting

Step 5: Distance Speed Control
  ├─ SpeedChangeMode: Choose Accelerate or Decelerate
  ├─ SpeedChangeRate: 1-3 (Recommended)
  ├─ MaxDistance: 10-20 Meters
  └─ Test animation speed at different distances

Step 6: Random Variation
  ├─ ScaleVariation: 1.2-1.5
  ├─ RandomRotVariation: 0.1-0.3 (Fine-tuning)
  └─ Create multiple copies to observe random effects
```

---

## VRC Integration Guide

### VRC Light Volume Support

**What is a Light Volume?**

VRC Light Volume is a tool in VRChat worlds used to add lighting to specific areas. When this feature is enabled, the Gaze Shader will automatically respond to the Light Volume's illumination.

**How to Enable**:

1. **Enable in Shader**:
   ```
   _UseLightVolume = ON
   _LightVolumeIntensity = 1.0 (Default)
   ```

2. **In VRC World**:
   - Ensure a Light Volume has been placed in the world
   - The Shader will automatically detect and respond to it

3. **Intensity Adjustment**:
   ```
   0.0: Completely unaffected by Light Volume
   1.0: Normal effect (Recommended)
   2.0: Enhanced effect (Obvious lighting changes)
   ```

### UdonSharp Script Integration

File: [UdonGazeGifTrigger.cs](UdonGazeGifTrigger.cs)

**Function**: Synchronizes gaze animation playback time across clients.

**Usage**:

1. Add this script to the GameObject containing the Gaze Shader material
2. The script automatically fetches the current time during OnEnable
3. Syncs the time to the Shader's `_StartTime` parameter
4. Ensures all players see a synchronized animation

**How it works**:

```csharp
// Executes automatically when the GameObject is enabled
OnEnable() {
    // Get the Renderer
    meshRenderer = GetComponent<Renderer>();
    
    // Set start time to current game time
    meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
    
    // The Shader uses this time to calculate the current frame to display
}
```

### Best Practices in VRChat

```
1. Network Issues
   ├─ Avoid changing material parameters frequently
   └─ Use AnimatorController to preset animation parameters

2. Performance Optimization
   ├─ Keep individual Gaze Shaders under 50 per scene
   ├─ Use SpriteSheets reasonably (Recommended)
   └─ Enable LOD systems (Lower resolution from afar)

3. Cross-Platform Compatibility
   ├─ Prioritize SpriteSheet over TextureArray
   ├─ Test on both Quest and PC platforms
   └─ Monitor Draw Call counts

4. Visual Effects
   ├─ Light Volume: Enable for ambient lighting influence
   ├─ Normal Maps: Enable only on high-end devices
   └─ Distance Control: Adjust MaxDistance based on scene space
```

---

## FAQ

### Q1: How do I choose the right Shader version?

**A**: In order of priority:

1. **First Choice: SpriteSheet Version** (`@Luna/Gaze GIF SpriteSheet`)
   - Strongest compatibility
   - Suitable for 99% of scenes

2. **Second Choice: TextureArray Version** (`@Luna/Gaze Gif`)
   - Condition: Requires a massive number of frames (>256)
   - Condition: Abundant memory available
   - Condition: Confirmed platform support

3. **Third Choice: Cutout Version** (`@Luna/Gaze GIF SpriteSheet Cutout`)
   - Condition: Image is completely opaque
   - Condition: Requires maximum performance

### Q2: What is the appropriate resolution for the SpriteSheet after conversion?

**A**: It depends on the usage:

```
Mobile/Quest:         512×512 or 1024×1024
VRChat Recommended:   1024×1024 to 2048×2048
High-end PC:          2048×2048 to 4096×4096
Shared across animations: Try to keep it consistent for optimization
```

### Q3: The gaze tracking looks unnatural, what should I do?

**A**: Adjust the following parameters:

```
1. Check _SingleAxisGaze
   └─ Characters should use Y (2) axis, others use All (0)

2. Adjust _WeakenDistanceGaze
   └─ Increasing this value weakens the effect from afar, making it more natural

3. Use _ExtraRotX/Y/Z for fine-tuning
   └─ Small adjustments of ±5-15° are usually sufficient

4. Enable Normal Maps for added detail
   └─ Makes lighting reactions more realistic
```

### Q4: What's the difference between "High Performance" and "High Quality" when converting GIF to SpriteSheet?

**A**:

```
High Performance Mode:
├─ Max Texture: 2048×2048
├─ Use Cases: Quest, Multiplayer scenes, Performance-sensitive
└─ Pros: Fast loading, low memory footprint

High Quality Mode:
├─ Max Texture: 4096×4096
├─ Use Cases: Desktop, High-end VR
└─ Pros: Detail preservation, highest visual quality

Selection Principle:
└─ Always use the lowest resolution that meets your visual needs
```

### Q5: What should the _NormalStrength for normal maps be set to?

**A**:

```
Negative values (-1 ~ -5):
└─ Inverts normals, not recommended

0 ~ 1 (Recommended):
├─ 0: No normal effect
├─ 0.5: Subtle highlight (Natural)
└─ 1.0: Standard highlight (Recommended)

1 ~ 5 (Enhanced):
├─ 2-3: Noticeable bumpiness
└─ 4-5: Extreme, exaggerated effects
```

### Q6: It looks flickering in VRChat, what should I do?

**A**: These are transparency artifacts. Turn on `_FixTransp = ON`

Alternatively:
```
1. Check the GIF's Alpha channel
   └─ Ensure edges don't have translucent noise

2. Adjust the Shader's Blend Mode
   └─ Some scenes might require modifying the blend mode

3. Check camera depth settings
   └─ Ensure ZTest = LEqual
```

### Q7: Too many Draw Calls, how do I optimize?

**A**: Use the Material Manager:

```
1. Open Tools → @Luna → Gaze Shader Material Manager
2. Click [Auto Optimize All]
3. The system will create shared material instances for identical textures
4. Automatically replaces duplicate materials in the scene

Result: Draw Calls will be significantly reduced
```

### Q8: I changed parameters in the Inspector but nothing happened?

**A**: Check the following:

```
1. Shader not fully loaded
   └─ Wait a moment and restart the editor

2. Material not properly applied
   └─ Reapply it in the Renderer

3. Previous settings are cached
   └─ Delete the Library/ShaderCache folder

4. Modified parameters during Play Mode
   └─ Changes are lost when Play Mode ends
   └─ You should modify them in Edit Mode
```

---

## Technical Details

### Shader Architecture

```
Gaze Shader Core Structure:

Vertex Shader:
├─ Fetch world coordinates and camera information
├─ Calculate gaze rotation matrix
├─ Generate object seed (for randomness)
├─ Apply scaling/rotation variations
└─ Output transformed vertices

Fragment Shader:
├─ Calculate current frame index
│  ├─ Based on time and FPS
│  ├─ Supports PlayMode logic
│  └─ Considers distance speed control
├─ Sample Texture
│  ├─ TextureArray Version: UNITY_SAMPLE_TEX2DARRAY
│  └─ SpriteSheet Version: Calculate UVs and sample
├─ Calculate Lighting (Optional)
│  ├─ Normal map sampling
│  ├─ Specular reflection calculation
│  └─ Light Volume integration
└─ Output final color
```

### Key Algorithms

#### 1. Gaze Tracking

```glsl
// Simplified Version
float3 cameraDir = normalize(cameraPos - worldPos);
float3x3 rotMatrix = RotateTowardsCamera(cameraDir);
float3 finalPos = mul(rotMatrix, vertexPos);
```

**Features**:
- Uses rotation matrices instead of Euler angles to avoid gimbal lock
- Supports multi-axis tracking selection
- Smooth transition for distance attenuation

#### 2. Distance Speed Control

```glsl
float distance = length(worldPos - cameraPos);
float speedMultiplier;

if (SpeedChangeMode == 1) {
    // Accelerate: Faster when closer
    speedMultiplier = 1.0 + (1 - dist / maxDist) * rate;
} else if (SpeedChangeMode == 2) {
    // Decelerate: Slower when closer
    speedMultiplier = 1.0 - (1 - dist / maxDist) * rate;
}

float adjustedFPS = fps * speedMultiplier;
```

#### 3. Frame Calculation

```glsl
float elapsedTime = time - startTime;
float frameIndex = floor(elapsedTime * adjustedFPS) % totalFrames;

// Support for different PlayModes
if (playMode == ONCE) {
    frameIndex = min(frameIndex, totalFrames - 1);
} else if (playMode == RANDOM) {
    // Calculate random segment
    frameIndex = RandomSegment(seed);
} else if (playMode == MANUAL) {
    frameIndex = manualFrame;
}
```

#### 4. SpriteSheet UV Calculation

```glsl
int column = int(frameIndex) % columns;
int row = int(frameIndex) / columns;

float u = (column + uv.x) / columns;
float v = (row + uv.y) / rows;

float2 spriteSheetUV = float2(u, v);
```

### Performance Considerations

```
TextureArray Version:
├─ Pros: Maximum performance, optimal memory usage
├─ Cons: TextureArray might not be supported on all platforms
└─ Performance: Optimal (100% Baseline)

SpriteSheet Version:
├─ Pros: Strong compatibility, easy conversion
├─ Cons: Single large texture might use more memory
└─ Performance: Very Good (95% Relative)

Cutout Version:
├─ Pros: Maximum performance (No Alpha blending)
├─ Cons: Only supports opaque images
└─ Performance: Optimal (105% Relative, Fastest)
```

### Compatibility

```
Platform Compatibility:

TextureArray (Gaze Gif):
├─ Windows/Mac: ✅ Fully Supported
├─ WebGL: ❌ Not Supported
├─ Android/Quest: ⚠️ Limited Support
└─ iOS: ❌ Not Supported

SpriteSheet (Recommended):
├─ Windows/Mac: ✅ Fully Supported
├─ WebGL: ✅ Fully Supported
├─ Android/Quest: ✅ Fully Supported
└─ iOS: ✅ Fully Supported

Cutout:
├─ All Platforms: ✅ Fully Supported
└─ Maximum Compatibility
```

---

## Appendix

### Quick Reference

#### Parameter Presets

**Default Settings (Recommended for Beginners)**:
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

**High Performance Settings**:
```
SpriteSheet Resolution: 512×512
FPS: 12
LightingEffect: OFF
UseNormalMap: OFF
Gaze: Y Axis (Single axis performs better than omnidirectional)
```

**High Quality Settings**:
```
SpriteSheet Resolution: 2048×2048 or TextureArray
FPS: 24
LightingEffect: ON
UseNormalMap: ON
NormalStrength: 1.0
SpecularSharpness: 30
```

### File Locations Guide

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
├─ Shader/
│  ├─ Gaze Gif.shader (TextureArray Version)
│  ├─ Gaze GIF SpriteSheet.shader (Recommended)
│  ├─ Gaze GIF SpriteSheet Cutout.shader (Opaque Version)
│  └─ LightVolumes.cginc (VRC Light Volume)
│
├─ Scripts/
│  └─ UdonGazeGifTrigger.cs (VRC Udon Script)
│
├─ logo/
│  ├─ s booth.png
│  ├─ s x.png
│  ├─ s b.png
│  ├─ @咸鱼子Luna.png
│  └─ Luna_ko.png
│
├─ Gaze_Gif_Shader_Summary_cn.md 
├─ Gaze_Gif_Shader_Summary_en.md (This Document)
├─ Gaze_Gif_Shader_Summary_ja.md
└─ README.md (Project Info)
```

### Social Media

- **Booth**: https://xianyuzi-luna.booth.pm/
- **X (Twitter)**: https://x.com/lunabxgg
- **Bilibili**: https://space.bilibili.com/3546752913247086

---

## Changelog

### v1.0 (2026-03-03)

**Initial Release, includes the following**:

- ✅ Three Shader Versions (TextureArray, SpriteSheet, Cutout)
- ✅ Complete Editor Tools Suite
- ✅ GIF Converter Tools (TextureArray and SpriteSheet)
- ✅ Normal Map Generator
- ✅ Material Manager
- ✅ VRC Light Volume Integration
- ✅ UdonSharp Support Script
- ✅ Multi-language Localization (EN/CN/JA)
- ✅ Complete Documentation and Examples

---

### 🛒 Commercial License

The open-source content in this GitHub repository is strictly for **personal, free, non-commercial use** (e.g., used in private VRChat Avatars, or published free VRChat Worlds).

### ⚠️ Commercial Use Restrictions

It is **strictly prohibited** to use this shader directly or indirectly for any commercial profit-making activities, including but not limited to:

- Selling commercial 3D models
- Paid Avatar commission customization
- Paid scene building
- Other commercial or profit-making purposes

### 💳 Purchasing a Commercial License

If you need to use this system in commercial works or paid projects, please visit **BOOTH** to purchase a commercial license:

**👉 [Click here to go to BOOTH and purchase a commercial license](https://xianyuzi-luna.booth.pm/items/7555039) 👈**

---

**Thank you for using the Gaze Shader! If you have any questions, please contact the author via the social media channels found on the author's logo -- @咸鱼子Luna -- in the bottom right corner of the shader interface.**