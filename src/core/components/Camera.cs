using System;
using System.Collections.Generic;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class Camera : Component, ISizable
{
    public Vector2 Size => Engine.RenderManager.ScreenToViewVector(Resolution);
    
    // TODO: Recalculate size and recreate render targets when changing resolution.
    public Vector2 Resolution { get; set; } = new Vector2(1920, 1080);
    public float AspectRatio => Resolution.X / Resolution.Y;

    public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

    private Dictionary<TransformSpace, RenderTarget2D> RenderTargetBySpace { get; }

    public Camera(Entity entity) : base(entity)
    {
        RenderTargetBySpace = [];
        CreateRenderTargets();
    }

    public RenderTarget2D GetRenderTarget(TransformSpace transformSpace)
    {
        return RenderTargetBySpace[transformSpace];
    }

    private void CreateRenderTargets()
    {
        foreach (TransformSpace transformSpace in Enum.GetValues(typeof(TransformSpace)))
        {
            RenderTargetBySpace[transformSpace] = new RenderTarget2D(Engine.GraphicsDevice, (int)Resolution.X, (int)Resolution.Y);
        }
    }
}