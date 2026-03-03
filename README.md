# 👁️ The Gaze Shader - Dynamic Gaze & Animation Shader System

## 📝 Introduction
**The Gaze Shader** is a 2D animation rendering and gaze-tracking system tailored for Unity and VRChat creators. It not only enables your GIFs or Sprite Sheets to play vividly in 3D space but also automatically tracks the player's perspective, creating an interactive sensation of "being watched." Combined with a built-in format conversion tool, one-click normal map generation, and perfect support for VRChat Light Volumes, it empowers you to effortlessly implement highly expressive dynamic elements in your scenes.

## ✨ Feature Overview
* **Built-in Efficient Converter:** Say goodbye to cumbersome external software. Convert GIFs into high-quality "Texture Arrays" or highly performant "Sprite Sheets" with just one click.
* **Smart Gaze Tracking:** Supports omnidirectional tracking and single-axis locking, complete with distance-sensing capabilities. Easily create posters, UIs, playing animations, directional signs, or the oppressive close-range gaze needed for horror games.
* **True Randomization for Multiple Objects:** Break through the rendering limitations of sharing a single material. Assign random seeds with one click, giving a massive number of identical objects completely independent playback effects and random variations.
* **Physical Lighting Integration:** Seamlessly integrates with VRC Light Volumes and real-time lighting. The built-in one-click normal map generator gives 2D animations a realistic 3D volumetric feel and specular reflections.
* **Cross-Platform Scene Adaptation:** Whether it's a dynamic accessory on a VRChat Avatar or an interactive object in a World, it provides targeted playback and optimization solutions.

---

## 📖 User Manual

To help you get started quickly, the layout of this manual perfectly matches the functional areas in the Material Inspector panel. Please read along while checking the UI.
*If you need a more detailed manual, please read `Gaze_Gif_Shader_Summary.md` included in the downloaded files.*

### 1. Resource Section
This is your first stop for importing animation resources.
* **GIF Conversion Tool:** The panel comes with a built-in one-click conversion feature. You can drop a GIF file directly into it to convert it into a higher-quality "Texture2DArray" or a "Sprite Sheet" for better performance and broader applicability.
* **Direct Sprite Sheet Import:** If you prefer not to use the GIF conversion tool, or if you already have a ready-made Sprite Sheet, you can drag the image directly into the slot. Just manually set the correct **Columns**, **Rows**, and **Total Frames** below, and the animation will run perfectly.

### 2. Effect Section
Controls the animation playback logic and the core "gaze" tracking behavior.
* **Animation Settings:** * **Play Mode - Once:** Perfect for creating trigger-based visual effects. If you are a **VRChat World creator**, please attach the provided activation component (`UdonGazeGifTrigger.asset`) to the item so it plays automatically and only once when it appears. If you are a **VRChat Avatar creator**, make good use of the "Manual" Play Mode to control the progress of a single playback by recording an animation.
    * **Distance Speed Control:** The playback speed of the animation can progressively accelerate/decelerate as the player approaches or moves away, increasing interactive feedback.
* **Gaze Effect:**
    * **Single Axis Gaze:** Restricts the image to rotate only around a specific axis. The **Y-axis** is the most commonly used setting, perfect for hanging ceiling signs or poster displays.
    * **Weaken Distance Gaze:** The object remains static when the player is far away and only gradually turns to gaze at the player as they get closer. **Highly recommended for horror scene creation!** *Note: When using this feature, pay close attention to the original front/back orientation of the object's mesh, otherwise, a bizarre reverse rotation might occur when approaching.*

### 3. Fixed Variations
Used for correcting and fine-tuning the base transformation of the object.
* **Quick Orientation:** If you find the image is facing the wrong way or disappears after enabling the gaze effect, it is usually due to the mesh's initial placement orientation. Creating a "3D Object - Quad" via the right-click menu in the Unity Hierarchy provides the most compatible model for the shader's default settings. Selecting options marked with a **★** from the dropdown menu can usually solve most orientation issues with one click.

### 4. Random Variations
Adds a sense of variation to repeatedly used elements in the scene, breaking the monotony of copy-pasting.
* **Randomize Multi-Objects:** When you have multiple 3D objects sharing the same material (to save DrawCalls) but want their playback progress and movements to be completely independent, **please be sure to click this button**. It will physically assign imperceptibly subtle scale differences to grant exclusive random seeds, thereby perfectly activating the "Random Variations" and "Start Frame Randomization" features.

### 5. Advanced Section
* **UV & Display Fix:** Supports quick horizontal/vertical flipping. If you encounter depth sorting errors when transparent objects overlap, you can check "Fix Artifacts" to resolve the rendering conflict.

### 6. Lighting Effect Section
Allows your 2D paper animations to blend perfectly into 3D environments.
* **Normals and Highlights:** The panel features a built-in **Generate Normal Map** button to create bump details from the current texture with one click. Combined with specular sharpness/brightness adjustments, the texture quality is instantly elevated.
* **VRC Light Volumes:** Perfectly adapted to VRChat's Light Volumes feature. Once enabled, the animation will receive real-time lighting, ambient lighting, and volumetric lighting from the scene. *Tip: If you need to show lighting gradients on a GIF with transparency changes, please do not set the GameObject to Static during baking to avoid unnatural baked shadows.*

---

## 🛠️ Shader Variants

This system provides the following rendering pipeline variants to suit different needs:
1. **Gaze Gif (Array Version):** Based on `Texture2DArray`, suitable for long animation sequences.
2. **Gaze GIF SpriteSheet (Transparent Version):** Based on the `Transparent` queue, supports Alpha blending, suitable for glowing gradients, smoke, and other translucent effects.
3. **Gaze GIF SpriteSheet Cutout (Cutout Version):** Based on `AlphaTest` culling and depth writing (`ZWrite On`). **Recommended for pixel art, UI icons, and other hard-edged images.** It avoids layer penetration issues with translucent objects, supports correct shadow casting, and has lower rendering overhead.

---

## 🛒 Commercial License
The open-source content in this GitHub repository is for personal, free, non-commercial use only (e.g., used for private VRChat Avatars, or publishing free VRChat Worlds).

⚠️ **It is strictly prohibited to use this shader directly or indirectly for any commercial, profit-making activities** (including but not limited to: selling commercial 3D models, paid Avatar commissions, or paid scene construction).

If you need to use this system in commercial works or paid projects, please go to BOOTH to purchase a commercial license:
👉 [Click here to purchase the commercial license on BOOTH](https://xianyuzi-luna.booth.pm/items/7555039) 👈

Thank you for using the Gaze Shader! If you have any questions, please contact the author via the social media channels accessible through the logo in the bottom right corner of the shader interface.