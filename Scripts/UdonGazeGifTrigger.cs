// 只有当项目里安装了 UdonSharp (World SDK) 时，才会编译以下代码
#if UDONSHARP

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UdonGazeGifTrigger : UdonSharpBehaviour
{
    private Renderer meshRenderer;

    // 当这个物体被勾选显示（激活）的那一瞬间，会执行这个函数
    private void OnEnable()
    {
        // 如果还没有获取到 Renderer，就获取一下
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<Renderer>();
        }

        // 把当前地图已经运行的时间，塞给 Shader
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
        }
    }
}

#endif // 结束UdonSharp隔离