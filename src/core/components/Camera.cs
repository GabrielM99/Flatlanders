using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class Camera : Component
{
    public Vector2 ViewportSize { get; set; } = new Vector2(1920, 1080);
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

    private void CreateViewport()
    {
        Viewport = new RenderTarget2D(Engine.GraphicsDevice, (int)ViewportSize.X, (int)ViewportSize.Y);
    }

    private void UpdateViewport()
    {
        if (Viewport != null && Viewport.Bounds.Size.ToVector2() != ViewportSize)
        {
            CreateViewport();
        }
    }
}