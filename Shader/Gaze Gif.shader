Shader "@Luna/Gaze Gif"
{
    // ========================================
    // Gaze Gif 着色器 - 凝视追踪GIF渲染系统
    // ========================================
    // 功能概述：
    // 1. 支持2D数组纹理动画序列播放（GIF、Sprite Sheet等）
    // 2. 动态凝视跟踪 - 自动朝向摄像机的广告牌效果
    // 3. 距离感应播放速度变化（渐进式加速/减速）
    // 4. 可选法线贴图支持（镜面反射、高光效果）
    // 5. VRC光照体积集成
    // 6. 多轴凝视模式（全向、X轴、Y轴、Z轴）
    // 7. 随机起始帧与播放模式（循环、单次、随机、手动）
    // ========================================
    
    Properties
    {
        [HideInInspector] _StartTime ("Start Time", Float) = 0
        [Header(Textures)]
        [NoScaleOffset]_Textures ("Texture Array", 2DArray) = "black" {}
        
        [Header(GIF Conversion)]
        [MainTexture] _GifTexture ("GIF Source", 2D) = "black" {}
        
        _Color("Color", Color) = (1,1,1,1)
        [IntRange] _fps ("Base FPS(Speed)", Range(1, 60)) = 12
        _StartFrameRandomization ("Start Frame Randomization", Range(0,1)) = 0
        
        [Enum(Loop, 0, Once, 1, Random, 2, Manual, 3)] _PlayMode ("Play Mode", Float) = 0
        [IntRange]_ManualFrame ("Manual Frame", Range(1, 100)) = 1
        
        [Header(Distance Speed Control)]
        [Enum(Uniform, 0, Accelerate, 1, Decelerate, 2)] _SpeedChangeMode ("Speed Change Mode", Float) = 0
        _SpeedChangeRate ("Speed Change Rate", Range(1, 10)) = 1
        _MaxDistance ("Max Distance", Range(5, 50)) = 15
        [Toggle]_SpeedFromZero ("Speed From Zero", Float) = 0
        
        [Toggle(_GAZE_ON)]_Gaze ("Gaze", Float) = 1
        [Enum(All, 0, X, 1, Y, 2, Z, 3)] _SingleAxisGaze ("Single Axis Gaze", Float) = 0
        _WeakenDistanceGaze ("Weaken Distance Gaze", Range(0,1)) = 0
        
        _ExtraRotX ("Extra X Rotation", Range(-180,180)) = 0
        _ExtraRotY ("Extra Y Rotation", Range(-180,180)) = 0
        _ExtraRotZ ("Extra Z Rotation", Range(-180,180)) = 0
        
        _ScaleVariation ("Scale Variation", Range(1,2)) = 1
        _RandomRotXVariation ("Random X Rotation Variation", Range(0,1)) = 0
        _RandomRotYVariation ("Random Y Rotation Variation", Range(0,1)) = 0
        _RandomRotZVariation ("Random Z Rotation Variation", Range(0,1)) = 0
        
        _UVx ("UV X Offset", Float) = 0
        _UVy ("UV Y Offset", Float) = 0
        _ScaleX ("X Scale", Float) = 1
        _ScaleY ("Y Scale", Float) = 1

        [Toggle(_NORMAL_MAP)]_UseNormalMap ("Use Normal Map", Float) = 0
        [NoScaleOffset]_NormalMapArray ("Normal Map Array", 2DArray) = "bump" {}
        _NormalStrength ("Normal Strength", Range(-5, 5)) = 1
        _SpecularSharpness ("Specular Sharpness", Range(1, 100)) = 20
        _SpecularBrightness ("Specular Brightness", Range(0, 1)) = 0.5

        [Toggle(_BACKFACE_CULLING)]_BackfaceCulling ("Backface Culling", Float) = 1
        [Toggle(_LIGHTING_EFFECT)]_LightingEffect ("Lighting Effect", Float) = 1
        [Toggle(_FIX_TRANSPARENCY)]_FixTransp ("Fix Artifacts", Float) = 0
        
        _Brightness ("Brightness", Range(0, 5)) = 1.0
       
        _LightVolumeIntensity ("Light Volume Intensity", Range(0, 2)) = 1.0
        [Toggle(_USE_LIGHT_VOLUME)]_UseLightVolume ("Use Light Volume", Float) = 1
    }

    SubShader
    {
            Tags
            { 
                "RenderType"="Transparent"
                "Queue"="Transparent+500"
                "DisableBatching"="True"
                "ForceNoShadowCasting"="True"
                "IgnoreProjector"="True"
            }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual

            CGINCLUDE

            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "./LightVolumes.cginc"

            UNITY_DECLARE_TEX2DARRAY(_Textures);
            UNITY_DECLARE_TEX2DARRAY(_NormalMapArray);
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            // ========== 数据结构定义 ==========
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 projPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                #if defined(_GAZE_ON)
                float gazeIntensity : TEXCOORD3;
                #endif
                float objectRandomSeed : TEXCOORD4;
                float3 worldNormal : TEXCOORD5;
                float3 viewDir : TEXCOORD6;
                float distanceToCamera : TEXCOORD7;
                #if defined(_NORMAL_MAP)
                float3 worldTangent : TEXCOORD8;
                float3 worldBitangent : TEXCOORD9;
                #endif
                UNITY_FOG_COORDS(10)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 _Color;
            int _fps;
            float _StartFrameRandomization;
            float _UVx, _UVy;
            float _ScaleX, _ScaleY;
            float _ExtraRotX, _ExtraRotY, _ExtraRotZ;
            float _WeakenDistanceGaze;
            float _ScaleVariation;
            float _RandomRotXVariation;
            float _RandomRotYVariation;
            float _RandomRotZVariation;
            
            float _PlayMode;
            float _ManualFrame;
            float _SingleAxisGaze;
            float _SpeedChangeMode;
            float _SpeedChangeRate;
            float _MaxDistance;
            float _SpeedFromZero;
            float _StartTime;

            float _UseNormalMap;
            float _NormalStrength;
            float _SpecularSharpness;  
            float _SpecularBrightness; 

            float _LightVolumeIntensity;
            float _UseLightVolume;
            float _Brightness;

            // ========== 工具函数 ==========
            
            float3x3 RotationMatrix(float3 angles)
            {
                float3 rad = radians(angles);
                float3 sinA, cosA;
                sincos(rad, sinA, cosA);
                
                return float3x3(
                    cosA.y*cosA.z,  -cosA.x*sinA.z + sinA.x*sinA.y*cosA.z,  sinA.x*sinA.z + cosA.x*sinA.y*cosA.z,
                    cosA.y*sinA.z,  cosA.x*cosA.z + sinA.x*sinA.y*sinA.z,   -sinA.x*cosA.z + cosA.x*sinA.y*sinA.z,
                    -sinA.y,        sinA.x*cosA.y,                          cosA.x*cosA.y
                );
            }

            float GenerateObjectSeed(float3 worldPos)
            {
                return frac(sin(dot(worldPos, float3(12.9898, 78.233, 45.543))) * 43758.5453);
            }

            float RandomFromSeed(float seed, float offset)
            {
                return frac(sin(dot(float2(seed, offset), float2(12.9898, 78.233))) * 43758.5453);
            }

            float3 SampleNormalMap(float2 uv, uint frame, float3 worldNormal, float3 worldTangent, float3 worldBitangent)
            {
                #ifdef _NORMAL_MAP
                float4 texNormal = UNITY_SAMPLE_TEX2DARRAY(_NormalMapArray, float3(uv, frame));
                
                #if !defined(UNITY_COLORSPACE_GAMMA)
                texNormal.rgb = pow(abs(texNormal.rgb), 1.0 / 2.2);
                #endif
                
                float3 tangentNormal;
                tangentNormal.xy = texNormal.xy * 2.0 - 1.0;
                tangentNormal.xy *= _NormalStrength;
                
                tangentNormal.z = sqrt(max(0.001, 1.0 - dot(tangentNormal.xy, tangentNormal.xy)));
                tangentNormal = normalize(tangentNormal);
                
                float3 worldNormalFromMap = tangentNormal.x * normalize(worldTangent) + 
                                            tangentNormal.y * normalize(worldBitangent) + 
                                            tangentNormal.z * normalize(worldNormal);
                return normalize(worldNormalFromMap);
                #else
                return worldNormal;
                #endif
            }

            float3 CalculateLighting(float3 worldNormal, float3 albedo, float3 worldPos, float3 viewDir)
            {
                float3 ambient = ShadeSH9(float4(worldNormal, 1));
                float3 volumeSpecular = float3(0, 0, 0); 
                
                #if defined(_USE_LIGHT_VOLUME)
                if (_UseLightVolume > 0.5 && LightVolumesEnabled() > 0.5)
                {
                    float3 L0, L1r, L1g, L1b;
                    LightVolumeSH(worldPos, L0, L1r, L1g, L1b);
                    float3 lightVolumeAmbient = LightVolumeEvaluate(worldNormal, L0, L1r, L1g, L1b);
                    
                    ambient = lerp(ambient, lightVolumeAmbient, _LightVolumeIntensity);
                    #if defined(_NORMAL_MAP)
                    float smoothness = saturate(_SpecularSharpness / 100.0);
                    volumeSpecular = LightVolumeSpecular(albedo, smoothness, 0.0, worldNormal, viewDir, L0, L1r, L1g, L1b);
                    volumeSpecular *= _SpecularBrightness * _LightVolumeIntensity;
                    #endif
                }
                #endif

                ambient *= _Brightness;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float atten = 1.0;
                
                if (_WorldSpaceLightPos0.w != 0.0)
                {
                    lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
                    float dist = distance(_WorldSpaceLightPos0.xyz, worldPos);
                    atten = 1.0 / (1.0 + dist * dist);
                }
                
                float NdotL = max(0, dot(worldNormal, lightDir));
                float3 diffuse = _LightColor0.rgb * NdotL * atten;
                
                float3 halfDir = normalize(lightDir + viewDir);
                float specular = pow(max(0, dot(worldNormal, halfDir)), _SpecularSharpness) * _SpecularBrightness;
                float3 specularColor = _LightColor0.rgb * specular * atten;
                
                float3 finalColor = albedo * (ambient + diffuse) + specularColor + volumeSpecular;
                return finalColor;
            }

            // ========== 顶点着色器 (修复法线与光照对齐问题) ==========
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 center = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                
                // 缩放值作为随机种子
                // 1. 提取无视位移和旋转的纯粹缩放值
                float3 exactScale = float3(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22)
                );
                
                // 2. 放大 100000 倍并四舍五入
                float3 roundedScale = round(exactScale * 100000.0);
                
                // 3. 混沌打乱
                float chaoticSeed = frac(sin(dot(roundedScale, float3(12.9898, 78.233, 37.719))) * 43758.5453);
                chaoticSeed = frac(sin(chaoticSeed * 13.37) * 43758.5453); 
                
                // 4. 结合 GPU Instancing ID 增加随机维度
                #ifdef UNITY_INSTANCING_ENABLED
                float instVal = (float)v.instanceID;
                #else
                float instVal = 0.0;
                #endif
                
                float baseSeed = frac(sin(dot(float2(chaoticSeed, instVal + 1.0), float2(12.9898, 78.233))) * 43758.5453);
                o.objectRandomSeed = baseSeed;
                
                float3 camPos = _WorldSpaceCameraPos;
                o.distanceToCamera = length(camPos - center);
                
                float scaleRandom = RandomFromSeed(baseSeed, 1.0);
                float scaleFactor = 1.0;
                if (_ScaleVariation > 1.0)
                {
                    float minScale = max(2.0 - _ScaleVariation, 0.5);
                    float maxScale = min(_ScaleVariation, 2.0);
                    scaleFactor = lerp(minScale, maxScale, scaleRandom);
                }
                
                float rotXRandom = RandomFromSeed(baseSeed, 2.0);
                float rotYRandom = RandomFromSeed(baseSeed, 3.0);
                float rotZRandom = RandomFromSeed(baseSeed, 4.0);
                float randomRotX = _ExtraRotX + (rotXRandom - 0.5) * 2.0 * _RandomRotXVariation * 180.0;
                float randomRotY = _ExtraRotY + (rotYRandom - 0.5) * 2.0 * _RandomRotYVariation * 180.0;
                float randomRotZ = _ExtraRotZ + (rotZRandom - 0.5) * 2.0 * _RandomRotZVariation * 180.0;

                float3x3 userRotMat = RotationMatrix(float3(randomRotX, randomRotY, randomRotZ));

                #ifdef _GAZE_ON
                float distanceToCamera = length(camPos - center);
                float gazeIntensity = 1.0 - (_WeakenDistanceGaze * saturate(distanceToCamera / 10.0));
                o.gazeIntensity = gazeIntensity;

                float3 viewDir = normalize(camPos - center);
                float3 up = float3(0,1,0);
                viewDir += dot(viewDir, up) > 0.999 ? float3(0.001,0,0.001) : 0;
                float3 right = normalize(cross(up, viewDir));
                up = normalize(cross(viewDir, right));

                float3 objScale = float3(
                    length(unity_ObjectToWorld._m00_m01_m02),
                    length(unity_ObjectToWorld._m10_m11_m12),
                    length(unity_ObjectToWorld._m20_m21_m22)
                );

                float3 originalPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                int axisMode = (int)_SingleAxisGaze;
                
                float3 bRight = right;
                float3 bUp = up;
                float3 bForward = viewDir;
                float3x3 combinedRot = mul(RotationMatrix(float3(0.0, -180.0, 0.0)), userRotMat);

                if (axisMode == 1) { 
                    bForward = normalize(float3(0, viewDir.y, viewDir.z));
                    bRight = normalize(cross(float3(0,1,0), bForward));
                    bUp = normalize(cross(bForward, bRight));
                } else if (axisMode == 2) { 
                    bForward = normalize(float3(viewDir.x, 0, viewDir.z));
                    bRight = normalize(cross(float3(0,1,0), bForward));
                    bUp = normalize(cross(bForward, bRight));
                } else if (axisMode == 3) { 
                    bForward = float3(0,0,1);
                    bRight = normalize(cross(float3(0,1,0), bForward));
                    bUp = normalize(cross(bForward, bRight));
                    float3x3 initialFix = float3x3(-1, 0, 0, 0, 1, 0, 0, 0, -1);
                    combinedRot = mul(initialFix, combinedRot);
                }
                
                float3 localVert = mul(combinedRot, v.vertex.xyz * scaleFactor);
                float3 billboardPos = center + bRight * (localVert.x * objScale.x) + bUp * (localVert.y * objScale.y) + bForward * (localVert.z * objScale.z);
                
                o.worldPos = lerp(originalPos, billboardPos, gazeIntensity);
                o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.0));
                
                float3 localNormal = mul(combinedRot, v.normal);
                float3 finalNormal = bRight * localNormal.x + bUp * localNormal.y + bForward * localNormal.z;
                o.worldNormal = normalize(lerp(normalize(mul((float3x3)unity_ObjectToWorld, v.normal)), finalNormal, gazeIntensity));

                #if defined(_NORMAL_MAP)
                float3 t = length(v.tangent.xyz) > 0.1 ? v.tangent.xyz : float3(1,0,0);
                float tw = v.tangent.w != 0.0 ? v.tangent.w : -1.0;
                float3 localTangent = mul(combinedRot, t);
                float3 finalTangent = bRight * localTangent.x + bUp * localTangent.y + bForward * localTangent.z;
                o.worldTangent = normalize(lerp(normalize(mul((float3x3)unity_ObjectToWorld, t)), finalTangent, gazeIntensity));
                o.worldBitangent = normalize(cross(o.worldNormal, o.worldTangent) * tw * unity_WorldTransformParams.w);
                #endif

                #else
                float3 rotatedVert = mul(userRotMat, v.vertex.xyz * scaleFactor);
                o.pos = UnityObjectToClipPos(float4(rotatedVert, 1.0));
                o.worldPos = mul(unity_ObjectToWorld, float4(rotatedVert, 1.0)).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                
                #if defined(_NORMAL_MAP)
                float3 t = length(v.tangent.xyz) > 0.1 ? v.tangent.xyz : float3(1,0,0);
                float tw = v.tangent.w != 0.0 ? v.tangent.w : -1.0;
                o.worldTangent = normalize(mul((float3x3)unity_ObjectToWorld, t));
                o.worldBitangent = normalize(cross(o.worldNormal, o.worldTangent) * tw * unity_WorldTransformParams.w);
                #endif
                #endif

                o.viewDir = normalize(camPos - o.worldPos);
                o.uv = v.uv;
                o.projPos = ComputeScreenPos(o.pos);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }
            // ========== 片元着色器 ==========
            fixed4 fragCommon (v2f i, bool isBackface)
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                #ifdef _FIX_TRANSPARENCY
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                clip(sceneZ - i.projPos.z + 0.0001);
                #endif

                float2 uv = (i.uv * float2(1/_ScaleX, 1/_ScaleY)) + float2(_UVx, _UVy);
                float width, height, layers;
                _Textures.GetDimensions(width, height, layers);
                
                float time = max(0.0f, _Time.y - _StartTime);
                float effectiveFPS = _fps;
                
                float speedMultiplier = 1.0;
                if (_SpeedChangeMode > 0.5)
                {
                    float distance = i.distanceToCamera;
                    const int segmentCount = 10;
                    float maxDistance = _MaxDistance;
                    
                    if (distance >= maxDistance)
                    {
                        if (_SpeedFromZero > 0.5)
                        {
                            speedMultiplier = (_SpeedChangeMode < 1.5) ? 0.0 : _SpeedChangeRate;
                        }
                        else
                        {
                            speedMultiplier = 1.0;
                        }
                    }
                    else
                    {
                        float normalizedDistance = saturate(distance / maxDistance);
                        int segmentIndex = int(normalizedDistance * segmentCount);
                        segmentIndex = min(segmentIndex, segmentCount - 1);
                        float segmentFactor = float(segmentIndex) / float(segmentCount - 1);
                        if (_SpeedFromZero > 0.5)
                        {
                            if (_SpeedChangeMode < 1.5)
                            {
                                speedMultiplier = lerp(0.0, _SpeedChangeRate, 1.0 - segmentFactor);
                            }
                            else
                            {
                                speedMultiplier = lerp(_SpeedChangeRate, 0.0, 1.0 - segmentFactor);
                            }
                        }
                        else
                        {
                            if (_SpeedChangeMode < 1.5)
                            {
                                speedMultiplier = lerp(_SpeedChangeRate, 1.0, segmentFactor);
                            }
                            else
                            {
                                speedMultiplier = lerp(1.0 / _SpeedChangeRate, 1.0, segmentFactor);
                            }
                        }
                        
                        speedMultiplier = clamp(speedMultiplier, 0.0, 5.0);
                    }
                }

                float adjustedTime = max(0.0f, _Time.y - _StartTime) * speedMultiplier;
                effectiveFPS = _fps;
                
                uint frame;
                if (_PlayMode > 2.5)
                {
                    uint baseFrameForRandom = (uint)fmod(_fps * time, layers);
                    uint maxRandomFrame = (uint)(_StartFrameRandomization * layers);
                    uint randomStartFrame = (uint)(i.objectRandomSeed * maxRandomFrame);
                    frame = ((uint)(_ManualFrame - 1) + randomStartFrame) % (uint)layers;
                }
                else
                {
                    uint baseFrame;
                    if (_PlayMode < 0.5)
                    {
                        baseFrame = (uint)fmod(effectiveFPS * adjustedTime, layers);
                    }
                    else if (_PlayMode < 1.5)
                    {
                        baseFrame = min((uint)(effectiveFPS * adjustedTime), (uint)layers - 1);
                    }
                    else
                    {
                        float randomInterval = RandomFromSeed(i.objectRandomSeed, 10.0) * 3.0 + 0.5;
                        uint intervalFrames = (uint)(randomInterval * effectiveFPS);
                        uint totalCycleFrames = layers + intervalFrames;
                        uint cycleFrame = (uint)fmod(effectiveFPS * adjustedTime, totalCycleFrames);
                        baseFrame = cycleFrame < layers ? cycleFrame : layers - 1;
                    }
                    
                    uint maxRandomFrame = (uint)(_StartFrameRandomization * layers);
                    uint randomStartFrame = (uint)(i.objectRandomSeed * maxRandomFrame);
                    frame = (baseFrame + randomStartFrame) % (uint)layers;
                }
                
                fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_Textures, float3(uv, frame));
                col.rgb *= _Color.rgb;
                col.a = saturate(col.a * _Color.a);
                
                #if defined(_NORMAL_MAP)
                float3 finalNormal = SampleNormalMap(uv, frame, i.worldNormal, i.worldTangent, i.worldBitangent);
                #else
                float3 finalNormal = normalize(i.worldNormal);
                #endif
                
                #ifdef _LIGHTING_EFFECT
                col.rgb = CalculateLighting(finalNormal, col.rgb, i.worldPos, normalize(i.viewDir));
                #else
                float3 ambient = ShadeSH9(float4(finalNormal, 1));
                col.rgb *= ambient;
                #endif

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG

            // ========== Pass 1: FORWARD_BACK ==========
            Pass
            {
                Name "FORWARD_BACK"
                Tags { "LightMode" = "ForwardBase" }
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                Cull Front
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag_back
                #pragma multi_compile_fwdbase
                #pragma multi_compile_fog
                #pragma multi_compile_instancing
                #pragma require 2darray
                
                #pragma shader_feature _GAZE_ON
                #pragma shader_feature _NORMAL_MAP
                #pragma shader_feature _LIGHTING_EFFECT
                #pragma shader_feature _FIX_TRANSPARENCY
                #pragma shader_feature _BACKFACE_CULLING
                #pragma shader_feature _USE_LIGHT_VOLUME
                #pragma multi_compile _ LIGHTVOLUMES_AVAILABLE

                fixed4 frag_back (v2f i) : SV_Target
                {
                    #ifdef _BACKFACE_CULLING
                        discard;
                        return fixed4(0, 0, 0, 0);
                    #else
                        return fragCommon(i, true);
                    #endif
                }
                ENDCG
            }

            // ========== Pass 2: FORWARD_FRONT ==========
            Pass
            {
                Name "FORWARD_FRONT"
                Tags { "LightMode" = "ForwardBase" }
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                Cull Back
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag_front
                #pragma multi_compile_fwdbase
                #pragma multi_compile_fog
                #pragma multi_compile_instancing
                #pragma require 2darray
                
                #pragma shader_feature _GAZE_ON
                #pragma shader_feature _NORMAL_MAP
                #pragma shader_feature _LIGHTING_EFFECT
                #pragma shader_feature _FIX_TRANSPARENCY
                #pragma shader_feature _USE_LIGHT_VOLUME
                #pragma multi_compile _ LIGHTVOLUMES_AVAILABLE

                fixed4 frag_front (v2f i) : SV_Target
                {
                    return fragCommon(i, false);
                }
                ENDCG
            }

            // ========== Pass 3: FORWARD_ADD (同步同步动画与改善衰减) ==========
            Pass
            {
                Name "FORWARD_ADD"
                Tags { "LightMode" = "ForwardAdd" }
                Blend SrcAlpha One
                ZWrite Off
                Cull Back
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag_add
                #pragma multi_compile_fwdadd
                #pragma multi_compile_fog
                #pragma multi_compile_instancing
                #pragma require 2darray
                
                #pragma shader_feature _GAZE_ON
                #pragma shader_feature _NORMAL_MAP
                #pragma shader_feature _LIGHTING_EFFECT
                #pragma shader_feature _FIX_TRANSPARENCY

                fixed4 frag_add (v2f i) : SV_Target
                {
                    #ifdef _LIGHTING_EFFECT
                    float2 uv = (i.uv * float2(1/_ScaleX, 1/_ScaleY)) + float2(_UVx, _UVy);
                    float width, height, layers;
                    _Textures.GetDimensions(width, height, layers);
                    
                    // --- 强行同步基层的动画算法 ---
                    float speedMultiplier = 1.0;
                    if (_SpeedChangeMode > 0.5)
                    {
                        float distance = i.distanceToCamera;
                        const int segmentCount = 10;
                        float maxDistance = _MaxDistance;
                        
                        if (distance >= maxDistance) {
                            if (_SpeedFromZero > 0.5) speedMultiplier = (_SpeedChangeMode < 1.5) ? 0.0 : _SpeedChangeRate;
                            else speedMultiplier = 1.0;
                        } else {
                            float normalizedDistance = saturate(distance / maxDistance);
                            int segmentIndex = int(normalizedDistance * segmentCount);
                            segmentIndex = min(segmentIndex, segmentCount - 1);
                            float segmentFactor = float(segmentIndex) / float(segmentCount - 1);
                            if (_SpeedFromZero > 0.5) {
                                if (_SpeedChangeMode < 1.5) speedMultiplier = lerp(0.0, _SpeedChangeRate, 1.0 - segmentFactor);
                                else speedMultiplier = lerp(_SpeedChangeRate, 0.0, 1.0 - segmentFactor);
                            } else {
                                if (_SpeedChangeMode < 1.5) speedMultiplier = lerp(_SpeedChangeRate, 1.0, segmentFactor);
                                else speedMultiplier = lerp(1.0 / _SpeedChangeRate, 1.0, segmentFactor);
                            }
                            speedMultiplier = clamp(speedMultiplier, 0.0, 5.0);
                        }
                    }

                    float time = max(0.0f, _Time.y - _StartTime);
                    float adjustedTime = max(0.0f, _Time.y - _StartTime) * speedMultiplier;
                    float effectiveFPS = _fps;
                    
                    uint frame;
                    if (_PlayMode > 2.5) {
                        uint maxRandomFrame = (uint)(_StartFrameRandomization * layers);
                        uint randomStartFrame = (uint)(i.objectRandomSeed * maxRandomFrame);
                        frame = ((uint)(_ManualFrame - 1) + randomStartFrame) % (uint)layers;
                    } else {
                        uint baseFrame;
                        if (_PlayMode < 0.5) baseFrame = (uint)fmod(effectiveFPS * adjustedTime, layers);
                        else if (_PlayMode < 1.5) baseFrame = min((uint)(effectiveFPS * adjustedTime), (uint)layers - 1);
                        else {
                            float randomInterval = RandomFromSeed(i.objectRandomSeed, 10.0) * 3.0 + 0.5;
                            uint intervalFrames = (uint)(randomInterval * effectiveFPS);
                            uint totalCycleFrames = layers + intervalFrames;
                            uint cycleFrame = (uint)fmod(effectiveFPS * adjustedTime, totalCycleFrames);
                            baseFrame = cycleFrame < layers ? cycleFrame : layers - 1;
                        }
                        uint maxRandomFrame = (uint)(_StartFrameRandomization * layers);
                        uint randomStartFrame = (uint)(i.objectRandomSeed * maxRandomFrame);
                        frame = (baseFrame + randomStartFrame) % (uint)layers;
                    }

                    fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_Textures, float3(uv, frame));
                    col.rgb *= _Color.rgb;
                    col.a = saturate(col.a * _Color.a);

                    // --- 使用平滑物理衰减公式 ---
                    float3 lightVec = _WorldSpaceLightPos0.xyz - i.worldPos;
                    float lightDist = length(lightVec);
                    float3 lightDir = lightVec / max(lightDist, 0.0001);
                    float atten = 1.0;
                    if (_WorldSpaceLightPos0.w != 0.0)
                    {
                        float range = 1.0 / sqrt(_WorldSpaceLightPos0.w); 
                        float distNorm = saturate(lightDist / range);
                        atten = 1.0 / (1.0 + 25.0 * distNorm * distNorm);
                        atten *= saturate(1.0 - distNorm) * saturate(1.0 - distNorm); // 在光照边缘实现极度柔和的融合过渡
                    }
                    
                    #if defined(_NORMAL_MAP)
                    float3 finalNormal = SampleNormalMap(uv, frame, i.worldNormal, i.worldTangent, i.worldBitangent);
                    #else
                    float3 finalNormal = normalize(i.worldNormal);
                    #endif
                    
                    float NdotL = max(0, dot(finalNormal, lightDir));
                    float3 diffuse = _LightColor0.rgb * NdotL * atten;
                    
                    col.rgb *= diffuse;
                    #else
                    fixed4 col = fixed4(0,0,0,0);
                    #endif

                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }

        FallBack "Transparent/Diffuse"
        CustomEditor "GazeGifShaderGUI"
}