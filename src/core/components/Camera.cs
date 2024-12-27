using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class Camera : Component, ISizable
{
    public Vector2 Size => Engine.Graphics.ScreenToViewVector(Resolution);
    
    public Vector2 Resolution { get; set; } = new Vector2(1920, 1080);
    public float AspectRatio => Resolution.X / Resolution.Y;
    
    public RenderTarget2D RenderTarget { get; private set; }
    
    public Color BackgroundColor { get; set; } = Color.CornflowerBlue;    

    public Camera(Entity entity) : base(entity)
    {
        CreateRenderTarget();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateRenderTarget();
    }

    private void CreateRenderTarget()
    {
        RenderTarget = new RenderTarget2D(Engine.GraphicsDevice, (int)Resolution.X, (int)Resolution.Y);
    }

    private void UpdateRenderTarget()
    {
        if (RenderTarget.Bounds.Size.ToVector2() != Resolution)
        {
            CreateRenderTarget();
            // TODO: Size won't be recalculated if PPU is changed.
            Entity.Transform.RecalculateSize();
        }
    }
}