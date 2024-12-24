using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class Camera : Component
{
    public Vector2 Resolution { get; set; } = new Vector2(1920, 1080);
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
        }

        Size = Engine.Graphics.ScreenToViewVector(Resolution);
    }
}