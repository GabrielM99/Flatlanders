using System;
using System.Collections.Generic;
using Flatlanders.Core.Graphics;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class Camera : Component, ISizable
{
    // TODO: Recalculate size and recreate render targets when changing resolution.
    public Vector2 Resolution { get; set; } = new Vector2(1920, 1080);
    public float AspectRatio => Resolution.X / Resolution.Y;

    public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

    public Vector2 ViewSize
    {
        get
        {
            Vector2 windowSize = Engine.RenderManager.WindowSize;
            return (windowSize.X / AspectRatio <= windowSize.Y) ? new Vector2(windowSize.X, windowSize.X / AspectRatio) : new Vector2(windowSize.Y * AspectRatio, windowSize.Y);
        }
    }
    public Vector2 ViewScale => ViewSize / Resolution;

    private Dictionary<TransformSpace, RenderTarget2D> RenderTargetBySpace { get; }

    public Camera(Entity entity) : base(entity)
    {
        RenderTargetBySpace = [];
        CreateRenderTargets();
    }

    public Vector2 GetSize(TransformSpace space)
    {
        return space == TransformSpace.World ? Engine.RenderManager.WindowToWorldVector(Resolution) : Engine.RenderManager.WindowToScreenVector(Resolution);
    }

    public RenderTarget2D GetRenderTarget(TransformSpace transformSpace)
    {
        return RenderTargetBySpace[transformSpace];
    }

    public Vector2 WindowToWorldPosition(Vector2 windowPosition)
    {
        RenderManager renderManager = Engine.RenderManager;

        Vector2 windowSize = renderManager.WindowSize;
        Vector2 offset = new((windowSize.X - ViewSize.X) * 0.5f, (windowSize.Y - ViewSize.Y) * 0.5f);
        Vector2 normalized = (windowPosition - ViewSize * 0.5f - offset) / ViewSize;

        return normalized * renderManager.WindowToWorldVector(Resolution) + Entity.Position;
    }

    public Vector2 WorldToWindowPosition(Vector2 worldPosition)
    {
        RenderManager renderManager = Engine.RenderManager;

        Vector2 windowSize = renderManager.WindowSize;
        Vector2 offset = new((windowSize.X - ViewSize.X) * 0.5f, (windowSize.Y - ViewSize.Y) * 0.5f);
        Vector2 normalized = (worldPosition - Entity.Position) / renderManager.WindowToWorldVector(Resolution);

        return (normalized * ViewSize) + offset + ViewSize * 0.5f;
    }

    private void CreateRenderTargets()
    {
        foreach (TransformSpace transformSpace in Enum.GetValues(typeof(TransformSpace)))
        {
            RenderTargetBySpace[transformSpace] = new RenderTarget2D(Engine.GraphicsDevice, (int)Resolution.X, (int)Resolution.Y);
        }
    }
}