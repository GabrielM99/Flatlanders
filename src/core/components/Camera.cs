using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class Camera : Component
{
    public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

    public Vector2 Resolution { get; set; } = new Vector2(1920, 1080);
    public RenderTarget2D Viewport { get; private set; }

    public Camera(Entity entity) : base(entity)
    {
        CreateViewport();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateViewport();
    }

    public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        return screenPosition / 100f; 
    }

    private void CreateViewport()
    {
        Viewport = new RenderTarget2D(Engine.GraphicsDevice, (int)Resolution.X, (int)Resolution.Y);
    }

    private void UpdateViewport()
    {
        if (Viewport != null && Viewport.Bounds.Size.ToVector2() != Resolution)
        {
            CreateViewport();
        }
    }
}