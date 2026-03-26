using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CopyColorRendererFeature : ScriptableRendererFeature
{
    class CopyColorPass : ScriptableRenderPass
    {
        RTHandle destination;

        public CopyColorPass(RTHandle dest)
        {
            destination = dest;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques + 1;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, destination);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public static RTHandle grabTexture;
    CopyColorPass pass;

    public override void Create()
    {
        grabTexture = RTHandles.Alloc("_GrabTexture", name: "_GrabTexture");
        pass = new CopyColorPass(grabTexture);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}