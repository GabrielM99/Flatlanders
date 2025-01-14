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
    public Vector2 ViewSize
    {
        get
        {
            float aspectRatio = ActiveCamera == null ? 1f : ActiveCamera.AspectRatio;
            return (WindowSize.X / aspectRatio <= WindowSize.Y) ? new Vector2(WindowSize.X, WindowSize.X / aspectRatio) : new Vector2(WindowSize.Y * aspectRatio, WindowSize.Y);
        }
    }

    public Vector2 SortAxis { get; set; }

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

    public Vector2 ViewToScreenVector(Vector2 screenVector)
    {
        return screenVector * PixelsPerUnitScale;
    }

    public Vector2 ScreenToWorldVector(Vector2 screenVector)
    {
        if (ActiveCamera == null)
        {
            return Vector2.Zero;
        }

        Vector2 offset = new((WindowSize.X - ViewSize.X) * 0.5f, (WindowSize.Y - ViewSize.Y) * 0.5f);
        Vector2 normalized = (screenVector - ViewSize * 0.5f - offset) / ViewSize;

        return normalized * (ActiveCamera.Resolution / ReferencePixelsPerUnit) + ActiveCamera.Entity.Node.Position;
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

        Vector2 offset = new((WindowSize.X - ViewSize.X) * 0.5f, (WindowSize.Y - ViewSize.Y) * 0.5f);
        Vector2 normalized = (worldVector - ActiveCamera.Entity.Node.Position) / (ActiveCamera.Resolution / ReferencePixelsPerUnit);

        return (normalized * ViewSize) + offset + ViewSize * 0.5f;
    }

    private Vector2 WorldToViewVector(Vector2 worldVector)
    {
        return worldVector * PixelsPerUnit;
    }

    public void DrawSprite(Sprite sprite, ITransform transform, Color color, SpriteEffects effects, short layer)
    {
        if(sprite == null)
        {
            return;
        }
        
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

    public void DrawText(SpriteFont font, string text, ITransform transform, Color color, SpriteEffects effects, short layer)
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

    public void DrawRectangle(ITransform transform, Color color, short layer)
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
        GraphicsDevice.SetRenderTarget(ActiveCamera.RenderTarget);
        GraphicsDevice.Clear(ActiveCamera.BackgroundColor);

        DrawWorldSpace();
        DrawScreenSpace();

        GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawView()
    {
        GraphicsDevice.Clear(Color.Black);
        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
        SpriteBatch.Draw(ActiveCamera.RenderTarget, new Rectangle(((WindowSize - ViewSize) * 0.5f).ToPoint(), ViewSize.ToPoint()), Color.White);
        SpriteBatch.End();
    }

    private void DrawWorldSpace()
    {
        Node cameraTransform = ActiveCamera.Entity.Node;
        Vector2 cameraPosition = WorldToViewVector(cameraTransform.Position);

        Matrix transformMatrix = Matrix.CreateRotationZ(cameraTransform.Rotation) * Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f) * Matrix.CreateScale(PixelsPerUnitScale, PixelsPerUnitScale, 1f) * Matrix.CreateTranslation(new Vector3(ActiveCamera.Resolution * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (IDrawer drawer in DrawersBySpace[TransformSpace.World])
        {
            RectangleF bounds = drawer.Transform.Bounds;
            Vector2 screenPosition = WorldToScreenVector(bounds.Position);

            float layerDepth = CalculateLayerDepth(drawer.Layer);
            float layerDepthSortOffset = Remap(
                (SortAxis.X * screenPosition.X / WindowSize.X) +
                (SortAxis.Y * screenPosition.Y / WindowSize.Y),
                0f, 1f, 0f, int.MaxValue / short.MaxValue / (float)int.MaxValue);

            layerDepth = MathHelper.Clamp(layerDepth + layerDepthSortOffset, 0f, 1f);

            bounds.Position = WorldToViewVector(bounds.Position);
            bounds.Size = WorldToViewVector(bounds.Size);

            drawer.Draw(SpriteBatch, bounds, layerDepth);
        }

        SpriteBatch.End();
    }

    public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    private void DrawScreenSpace()
    {
        Matrix transformMatrix = Matrix.CreateScale(PixelsPerUnitScale, PixelsPerUnitScale, 1f) * Matrix.CreateTranslation(new Vector3(ActiveCamera.Resolution * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (IDrawer drawer in DrawersBySpace[TransformSpace.Screen])
        {
            ITransform transform = drawer.Transform;
            RectangleF bounds = transform.Bounds;
            bounds.Position += Vector2.Multiply(transform.Root.AnchorPosition, ActiveCamera.Resolution / PixelsPerUnitScale) * 0.5f;
            drawer.Draw(SpriteBatch, bounds, CalculateLayerDepth(drawer.Layer));
        }

        SpriteBatch.End();
    }
}