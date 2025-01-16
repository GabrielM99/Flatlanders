using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public class Graphics : DrawableGameComponent
{
    private readonly struct DrawRequest(Transform transform, IDrawer drawer, Color color, sbyte layer, Vector2 sortingOrigin)
    {
        public Transform Transform { get; } = transform;
        public IDrawer Drawer { get; } = drawer;
        public Color Color { get; } = color;
        public sbyte Layer { get; } = layer;
        public Vector2 SortingOrigin { get; } = sortingOrigin;
    }

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

    public Vector2 SortingAxis { get; set; }

    private GraphicsDeviceManager GraphicsDeviceManager { get; }
    private SpriteBatch SpriteBatch { get; set; }

    private Dictionary<TransformSpace, List<DrawRequest>> DrawerRequestsBySpace { get; }

    public Graphics(Game game) : base(game)
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(game);
        DrawerRequestsBySpace = [];
        CreateDrawerSpaces();
    }

    private static float CalculateLayerDepth(sbyte renderLayer, sbyte sortingLayer, sbyte orderLayer)
    {
        // The last 8 bits [0..7] have no effect, thus are never used.
        int packedLayerDepth = renderLayer << 24 | sortingLayer << 16 | orderLayer << 8;
        // Remaps into [0..1] range.
        return ((float)packedLayerDepth - int.MinValue) / ((float)int.MaxValue - int.MinValue);
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

    public void Draw(ITransform transform, IDrawer drawer, Color color, sbyte layer, Vector2 sortingOrigin = default)
    {
        DrawerRequestsBySpace[transform.Space].Add(new DrawRequest(Transform.Copy(transform), drawer, color, layer, sortingOrigin));
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

    public Vector2 WorldToViewVector(Vector2 worldVector)
    {
        return worldVector * PixelsPerUnit;
    }

    private void CreateDrawerSpaces()
    {
        foreach (TransformSpace space in Enum.GetValues(typeof(TransformSpace)))
        {
            DrawerRequestsBySpace.Add(space, []);
        }
    }

    private void ClearDrawers()
    {
        foreach (List<DrawRequest> drawerRequests in DrawerRequestsBySpace.Values)
        {
            drawerRequests.Clear();
        }
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

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend);

        // This is used to maintain the order of consecutive draw calls in a layer.
        sbyte orderLayer = 0;
        sbyte lastLayer = 0;

        foreach (DrawRequest drawRequest in DrawerRequestsBySpace[TransformSpace.World])
        {
            if (drawRequest.Layer != lastLayer)
            {
                orderLayer = 0;
                lastLayer = drawRequest.Layer;
            }

            Transform transform = drawRequest.Transform;

            Vector2 sortingPosition = transform.Position + drawRequest.SortingOrigin;
            Vector2 screenPosition = WorldToScreenVector(sortingPosition);
            Vector2 sortingScreenPosition = Vector2.Multiply(SortingAxis, Vector2.Divide(screenPosition, WindowSize));

            transform.Position = WorldToViewVector(transform.Position);
            transform.Size = WorldToViewVector(transform.Size);

            sbyte sortingLayer = (sbyte)Math.Clamp(sortingScreenPosition.Length() * 127, sbyte.MinValue, sbyte.MaxValue);

            drawRequest.Drawer.Draw(SpriteBatch, transform, drawRequest.Color, CalculateLayerDepth(drawRequest.Layer, sortingLayer, orderLayer));
            orderLayer++;
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

        foreach (DrawRequest drawRequest in DrawerRequestsBySpace[TransformSpace.Screen])
        {
            Transform transform = drawRequest.Transform;
            transform.Position += Vector2.Multiply(ITransform.GetAnchorPosition(transform.Root.Anchor), ActiveCamera.Resolution / PixelsPerUnitScale) * 0.5f;
            drawRequest.Drawer.Draw(SpriteBatch, transform, drawRequest.Color, CalculateLayerDepth(drawRequest.Layer, 0, 0));
        }

        SpriteBatch.End();
    }
}