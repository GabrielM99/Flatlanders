using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public class Graphics : DrawableGameComponent
{
    private const int ReferencePixelsPerUnit = 100;

    public int PixelsPerUnit { get; set; } = 100;
    private float UnitScale => (float)ReferencePixelsPerUnit / PixelsPerUnit;
    // TODO: Other aspect ratios aren't currently supported.

    public float AspectRatio { get; set; } = 16f / 9f;

    public Camera ActiveCamera { get; set; }

    public bool IsFullscreen { get => GraphicsDeviceManager.IsFullScreen; set => GraphicsDeviceManager.IsFullScreen = value; }
    public Vector2 Resolution
    {
        get => new(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight);

        set
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = (int)value.X;
            GraphicsDeviceManager.PreferredBackBufferHeight = (int)value.Y;
        }
    }

    private GraphicsDeviceManager GraphicsDeviceManager { get; }
    private SpriteBatch SpriteBatch { get; set; }

    private Dictionary<TransformSpace, List<IDrawer>> DrawersBySpace { get; }

    public Graphics(Game game) : base(game)
    {
        GraphicsDeviceManager = new(game);
        DrawersBySpace = new();

        CreateDrawerSpaces();
    }

    private static float CalculateLayerDepth(short layer)
    {
        return ((float)layer / short.MaxValue + 1f) * 0.5f;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // TODO: Extract to a method.
        foreach (TransformSpace space in DrawersBySpace.Keys)
        {
            DrawersBySpace[space].Clear();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        DrawCamera();
    }

    public void DrawSprite(Sprite sprite, Transform transform, Color color, SpriteEffects effects, short layer)
    {
        Draw(transform.Space, new TextureDrawer
        {
            Texture = sprite.Texture,
            Transform = transform,
            SourceRectangle = sprite.Rectangle,
            Color = color,
            Origin = sprite.Origin,
            Effects = effects,
            Layer = layer,
        });
    }

    public void DrawText(SpriteFont font, string text, Transform transform, Color color, SpriteEffects effects, short layer)
    {
        Draw(transform.Space, new TextDrawer
        {
            Text = text,
            Font = font,
            Transform = transform,
            Color = color,
            Effects = effects,
            Layer = layer
        });
    }

    public void DrawRectangle(Transform transform, Color color, short layer)
    {
        Draw(transform.Space, new RectangleDrawer
        {
            Transform = transform,
            Color = color,
            Layer = layer
        });
    }

    private void CreateDrawerSpaces()
    {
        foreach (TransformSpace space in Enum.GetValues(typeof(TransformSpace)))
        {
            DrawersBySpace.Add(space, new List<IDrawer>());
        }
    }

    private void Draw(TransformSpace space, IDrawer drawer)
    {
        DrawersBySpace[space].Add(drawer);
    }

    private Vector2 CalculateWorldPosition(Vector2 position)
    {
        position.X *= PixelsPerUnit;
        position.Y *= PixelsPerUnit;
        return position;
    }

    private RectangleF CalculateWorldBounds(RectangleF bounds)
    {
        // We ignore the size when scaling.
        bounds.Position = CalculateWorldPosition(bounds.Position + (Vector2)bounds.Size * 0.5f) - bounds.Size * 0.5f;
        return bounds;
    }

    private RectangleF CalculateScreenBounds(RectangleF bounds, Vector2 rootAnchorPosition)
    {
        // Roots will be anchored on the viewport itself.
        bounds.Position += Vector2.Multiply(rootAnchorPosition, ActiveCamera.ViewportSize / UnitScale) * 0.5f;
        return bounds;
    }

    private void DrawCamera()
    {
        if (ActiveCamera == null)
        {
            GraphicsDevice.Clear(Color.Black);
            return;
        }

        DrawSpaces();
        DrawView();
    }

    private void DrawSpaces()
    {
        GraphicsDevice.SetRenderTarget(ActiveCamera.Viewport);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        DrawWorldSpace();
        DrawScreenSpace();

        GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawView()
    {
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);

        Vector2 windowSize = GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2();
        Vector2 viewportSize = (windowSize.X / AspectRatio <= windowSize.Y) ? new Vector2(windowSize.X, windowSize.X / AspectRatio) : new Vector2(windowSize.Y * AspectRatio, windowSize.Y);

        SpriteBatch.Draw(ActiveCamera.Viewport, new Rectangle(((windowSize - viewportSize) * 0.5f).ToPoint(), viewportSize.ToPoint()), Color.White);

        SpriteBatch.End();
    }

    private void DrawWorldSpace()
    {
        Transform cameraTransform = ActiveCamera.Entity.Transform;
        Vector2 cameraPosition = CalculateWorldPosition(cameraTransform.Position);

        Matrix transformMatrix = Matrix.CreateRotationZ(cameraTransform.Rotation) * Matrix.CreateTranslation(cameraPosition.X, cameraPosition.Y, 0f) * Matrix.CreateScale(UnitScale, UnitScale, 1f) * Matrix.CreateTranslation(new Vector3(ActiveCamera.ViewportSize * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (IDrawer drawer in DrawersBySpace[TransformSpace.World])
        {
            drawer.Draw(SpriteBatch, CalculateWorldBounds(drawer.Transform.Bounds), CalculateLayerDepth(drawer.Layer));
        }

        SpriteBatch.End();
    }

    private void DrawScreenSpace()
    {
        Matrix transformMatrix = Matrix.CreateScale(UnitScale, UnitScale, 1f) * Matrix.CreateTranslation(new Vector3(ActiveCamera.ViewportSize * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (IDrawer drawer in DrawersBySpace[TransformSpace.Screen])
        {
            Transform transform = drawer.Transform;
            drawer.Draw(SpriteBatch, CalculateScreenBounds(transform.Bounds, transform.Root.AnchorPosition), CalculateLayerDepth(drawer.Layer));
        }

        SpriteBatch.End();
    }
}