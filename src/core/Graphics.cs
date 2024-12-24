using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public class Graphics : DrawableGameComponent
{
    public int ReferencePixelsPerUnit { get; set; } = 100;
    public int PixelsPerUnit { get; set; } = 100;
    public float PixelsPerUnitScale => (float)ReferencePixelsPerUnit / PixelsPerUnit;

    public Camera ActiveCamera { get; set; }

    public bool IsFullscreen { get => GraphicsDeviceManager.IsFullScreen; set => GraphicsDeviceManager.IsFullScreen = value; }
    public Vector2 WindowSize
    {
        get => GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2();

        set
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = (int)value.X;
            GraphicsDeviceManager.PreferredBackBufferHeight = (int)value.Y;
        }
    }
    // TODO: Other aspect ratios aren't currently supported.
    public float AspectRatio { get; set; } = 16f / 9f;
    public Vector2 ViewportSize => (WindowSize.X / AspectRatio <= WindowSize.Y) ? new Vector2(WindowSize.X, WindowSize.X / AspectRatio) : new Vector2(WindowSize.Y * AspectRatio, WindowSize.Y);

    private GraphicsDeviceManager GraphicsDeviceManager { get; }
    private SpriteBatch SpriteBatch { get; set; }

    private Dictionary<TransformSpace, List<IDrawer>> DrawersBySpace { get; }

    public Graphics(Game game) : base(game)
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(game);
        DrawersBySpace = new Dictionary<TransformSpace, List<IDrawer>>();

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
        ClearDrawers();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        DrawCamera();
    }

    public Vector2 ScreenToViewVector(Vector2 screenVector)
    {
        return screenVector / PixelsPerUnitScale;
    }

    public Vector2 ScreenToWorldVector(Vector2 screenVector)
    {
        if (ActiveCamera == null)
        {
            return Vector2.Zero;
        }

        Vector2 offset = new((WindowSize.X - ViewportSize.X) * 0.5f, (WindowSize.Y - ViewportSize.Y) * 0.5f);
        Vector2 normalized = (screenVector - ViewportSize * 0.5f - offset) / ViewportSize;

        return normalized * (ActiveCamera.Resolution / ReferencePixelsPerUnit) + ActiveCamera.Entity.Transform.Position;
    }

    public Vector2 ViewToScreenVector(Vector2 viewVector)
    {
        return viewVector * PixelsPerUnitScale;
    }

    public Vector2 ViewToWorldVector(Vector2 viewVector)
    {
        return viewVector / PixelsPerUnit;
    }

    public Vector2 WorldToScreenVector(Vector2 worldVector)
    {
        if (ActiveCamera == null)
        {
            return Vector2.Zero;
        }

        Vector2 offset = new((WindowSize.X - ViewportSize.X) * 0.5f, (WindowSize.Y - ViewportSize.Y) * 0.5f);
        Vector2 normalized = (worldVector - ActiveCamera.Entity.Transform.Position) / (ActiveCamera.Resolution / ReferencePixelsPerUnit);

        return (normalized * ViewportSize) + offset + ViewportSize * 0.5f;
    }

    private Vector2 WorldToViewVector(Vector2 worldVector)
    {
        return worldVector * PixelsPerUnit;
    }

    public void DrawSprite(Sprite sprite, Transform transform, Vector2 offset, Color color, SpriteEffects effects, short layer)
    {
        Draw(transform.Space, new TextureDrawer
        {
            Texture = sprite.Texture,
            Transform = transform,
            SourceRectangle = sprite.Rectangle,
            Color = color,
            Origin = sprite.Origin - Vector2.Multiply(offset, sprite.Rectangle.Size.ToVector2()),
            Effects = effects,
            Layer = layer,
        });
    }

    public void DrawSprite(Sprite sprite, Transform transform, Color color, SpriteEffects effects, short layer)
    {
        DrawSprite(sprite, transform, Vector2.Zero, color, effects, layer);
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

    private void ClearDrawers()
    {
        foreach (TransformSpace space in DrawersBySpace.Keys)
        {
            DrawersBySpace[space].Clear();
        }
    }

    private void Draw(TransformSpace space, IDrawer drawer)
    {
        DrawersBySpace[space].Add(drawer);
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
        GraphicsDevice.Clear(ActiveCamera.BackgroundColor);

        DrawWorldSpace();
        DrawScreenSpace();

        GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawView()
    {
        GraphicsDevice.Clear(Color.Black);
        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
        SpriteBatch.Draw(ActiveCamera.Viewport, new Rectangle(((WindowSize - ViewportSize) * 0.5f).ToPoint(), ViewportSize.ToPoint()), Color.White);
        SpriteBatch.End();
    }

    private void DrawWorldSpace()
    {
        Transform cameraTransform = ActiveCamera.Entity.Transform;
        Vector2 cameraPosition = WorldToViewVector(cameraTransform.Position);

        Matrix transformMatrix = Matrix.CreateRotationZ(cameraTransform.Rotation) * Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f) * Matrix.CreateScale(PixelsPerUnitScale, PixelsPerUnitScale, 1f) * Matrix.CreateTranslation(new Vector3(ActiveCamera.Resolution * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (IDrawer drawer in DrawersBySpace[TransformSpace.World])
        {            
            RectangleF bounds = drawer.Transform.Bounds;
            bounds.Position = WorldToViewVector(bounds.Position);
            bounds.Size = WorldToViewVector(bounds.Size);
            drawer.Draw(SpriteBatch, bounds, CalculateLayerDepth(drawer.Layer));
        }

        SpriteBatch.End();
    }

    private void DrawScreenSpace()
    {
        Matrix transformMatrix = Matrix.CreateScale(PixelsPerUnitScale, PixelsPerUnitScale, 1f) * Matrix.CreateTranslation(new Vector3(ActiveCamera.Resolution * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (IDrawer drawer in DrawersBySpace[TransformSpace.Screen])
        {
            Transform transform = drawer.Transform;
            RectangleF bounds = transform.Bounds;
            bounds.Position += Vector2.Multiply(transform.Root.AnchorPosition, ActiveCamera.Resolution / PixelsPerUnitScale) * 0.5f;
            drawer.Draw(SpriteBatch, bounds, CalculateLayerDepth(drawer.Layer));
        }

        SpriteBatch.End();
    }
}