using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Flatlanders.Core.Graphics.Drawers;
using Flatlanders.Core.Graphics.Lighting;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Graphics;

public partial class RenderManager : DrawableGameComponent
{
    private readonly struct DrawRequest(Transform transform, IDrawer drawer, Color color, sbyte layer, Vector2 sortingOrigin, sbyte order)
    {
        public Transform Transform { get; } = transform;
        public IDrawer Drawer { get; } = drawer;
        public Color Color { get; } = color;
        public sbyte Layer { get; } = layer;
        public sbyte Order { get; } = order;
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

    public Color AmbientLightColor { get; set; } = Color.White;

    public Penumbra.PenumbraComponent Lighting { get; }

    private GraphicsDeviceManager GraphicsDeviceManager { get; }
    private SpriteBatch SpriteBatch { get; set; }

    private Dictionary<TransformSpace, List<DrawRequest>> DrawerRequestsBySpace { get; }

    private Engine Engine { get; }

    public RenderManager(Engine engine) : base(engine)
    {
        Engine = engine;
        GraphicsDeviceManager = new GraphicsDeviceManager(engine);
        Lighting = new Penumbra.PenumbraComponent(engine);
        DrawerRequestsBySpace = [];
        CreateDrawerSpaces();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public override void Initialize()
    {
        Lighting.Initialize();
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        ClearDrawers();
        Lighting.AmbientColor = AmbientLightColor;
    }

    public override void Draw(GameTime gameTime)
    {
        if (ActiveCamera == null)
        {
            GraphicsDevice.Clear(Color.Black);
            return;
        }

        // Gets the render target for each space.
        RenderTarget2D worldRenderTarget = ActiveCamera.GetRenderTarget(TransformSpace.World);
        RenderTarget2D screenRenderTarget = ActiveCamera.GetRenderTarget(TransformSpace.Screen);

        // Draw all space's render targets.
        GraphicsDevice.SetRenderTarget(worldRenderTarget);
        GraphicsDevice.Clear(ActiveCamera.BackgroundColor);

        DrawWorldSpace();

        GraphicsDevice.SetRenderTarget(null);

        GraphicsDevice.SetRenderTarget(screenRenderTarget);
        GraphicsDevice.Clear(Color.Transparent);

        DrawScreenSpace();

        GraphicsDevice.SetRenderTarget(null);

        // Configure the lighting transform matrix. 
        // Lighting works with window coordinates to maintain the high quality of shadows.
        Vector2 cameraPosition = WorldToWindowVector(ActiveCamera.Entity.Position);
        Matrix lightingTransformMatrix = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f) * Matrix.CreateScale(ViewSize.X / ActiveCamera.Resolution.X, ViewSize.Y / ActiveCamera.Resolution.Y, 1f) * Matrix.CreateTranslation(new Vector3(WindowSize * 0.5f, 0f));

        Lighting.Transform = lightingTransformMatrix;
        Lighting.BeginDraw();

        // The letterboxed rectangle that represents the game view.
        Rectangle viewRectangle = new(((WindowSize - ViewSize) * 0.5f).ToPoint(), ViewSize.ToPoint());

        // Draw the world space render target.
        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default);
        SpriteBatch.Draw(worldRenderTarget, viewRectangle, Color.White);
        SpriteBatch.End();

        // Draw the lighting.
        Lighting.Draw(gameTime);

        // Draw the screen space render target.
        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default);
        SpriteBatch.Draw(screenRenderTarget, viewRectangle, Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    public void Draw(ITransform transform, IDrawer drawer, Color color, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        DrawerRequestsBySpace[transform.Space].Add(new DrawRequest(Transform.Copy(transform), drawer, color, layer, sortingOrigin, order));
    }

    public Vector2 WindowToScreenVector(Vector2 windowVector)
    {
        return windowVector / PixelsPerUnitScale;
    }

    public Vector2 WindowToWorldVector(Vector2 windowVector)
    {
        return ScreenToWorldVector(WindowToScreenVector(windowVector));
    }

    public Vector2 ScreenToWindowVector(Vector2 screenVector)
    {
        return screenVector * PixelsPerUnitScale;
    }

    public Vector2 ScreenToWorldVector(Vector2 screenVector)
    {
        return screenVector / PixelsPerUnit;
    }

    public Vector2 WorldToScreenVector(Vector2 worldVector)
    {
        return worldVector * PixelsPerUnit;
    }

    public Vector2 WorldToWindowVector(Vector2 worldVector)
    {
        return ScreenToWindowVector(WorldToScreenVector(worldVector));
    }

    public Vector2 WindowToWorldPosition(Vector2 windowPosition)
    {
        if (ActiveCamera == null)
        {
            return Vector2.Zero;
        }

        Vector2 offset = new((WindowSize.X - ViewSize.X) * 0.5f, (WindowSize.Y - ViewSize.Y) * 0.5f);
        Vector2 normalized = (windowPosition - ViewSize * 0.5f - offset) / ViewSize;

        return normalized * WindowToWorldVector(ActiveCamera.Resolution) + ActiveCamera.Entity.Position;
    }

    public Vector2 WorldToWindowPosition(Vector2 worldPosition)
    {
        if (ActiveCamera == null)
        {
            return Vector2.Zero;
        }

        Vector2 offset = new((WindowSize.X - ViewSize.X) * 0.5f, (WindowSize.Y - ViewSize.Y) * 0.5f);
        Vector2 normalized = (worldPosition - ActiveCamera.Entity.Position) / WindowToWorldVector(ActiveCamera.Resolution);

        return (normalized * ViewSize) + offset + ViewSize * 0.5f;
    }

    public void AddLight(Light light)
    {
        Lighting.Lights.Add(light.Data);
    }

    public void RemoveLight(Light light)
    {
        Lighting.Lights.Remove(light.Data);
    }

    public LightOccluder CreateLightOccluder(Vector2 position, params Vector2[] points)
    {
        LightOccluder lightOccluder = new(Engine, position, points);
        Lighting.Hulls.Add(lightOccluder.Data);
        return lightOccluder;
    }

    public void DestroyLightOccluder(LightOccluder lightOccluder)
    {
        Lighting.Hulls.Remove(lightOccluder.Data);
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

    private void DrawWorldSpace()
    {
        Entity cameraEntity = ActiveCamera.Entity;
        Vector2 cameraPosition = WorldToScreenVector(cameraEntity.Position);

        Matrix transformMatrix =
            Matrix.CreateRotationZ(cameraEntity.Rotation) *
            Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f) *
            Matrix.CreateScale(PixelsPerUnitScale, PixelsPerUnitScale, 1f) *
            Matrix.CreateTranslation(new Vector3(ActiveCamera.Resolution * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend);

        foreach (DrawRequest drawRequest in DrawerRequestsBySpace[TransformSpace.World])
        {
            Transform transform = drawRequest.Transform;

            Vector2 sortingPosition = transform.Position + drawRequest.SortingOrigin;
            Vector2 screenPosition = WorldToWindowPosition(sortingPosition);
            Vector2 sortingScreenPosition = Vector2.Multiply(SortingAxis, Vector2.Divide(screenPosition, WindowSize));

            transform.Position = WorldToScreenVector(transform.Position);
            transform.Size = WorldToScreenVector(transform.Size);

            sbyte sortingLayer = (sbyte)(Math.Clamp(sortingScreenPosition.Y, -1f, 1f) * 127);

            drawRequest.Drawer.Draw(SpriteBatch, transform, drawRequest.Color, CalculateLayerDepth(drawRequest.Layer, sortingLayer, drawRequest.Order));
        }

        SpriteBatch.End();
    }

    private void DrawScreenSpace()
    {
        Matrix transformMatrix =
            Matrix.CreateScale(PixelsPerUnitScale, PixelsPerUnitScale, 1f) *
            Matrix.CreateTranslation(new Vector3(ActiveCamera.Resolution * 0.5f, 0f));

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

        foreach (DrawRequest drawRequest in DrawerRequestsBySpace[TransformSpace.Screen])
        {
            Transform transform = drawRequest.Transform;
            ITransform root = transform.Root;

            transform.Position += Vector2.Multiply(ITransform.GetAnchorPosition(root == null ? TransformAnchor.Center : root.Anchor), WindowToScreenVector(ActiveCamera.Resolution)) * 0.5f;

            drawRequest.Drawer.Draw(SpriteBatch, transform, drawRequest.Color, CalculateLayerDepth(drawRequest.Layer, 0, 0));
        }

        SpriteBatch.End();
    }

    private static float CalculateLayerDepth(sbyte renderLayer, sbyte sortingLayer, sbyte orderLayer)
    {
        // The last 8 bits [0..7] have no effect, thus are never used.
        int packedLayerDepth = renderLayer << 24 | ((byte)(sortingLayer + 128)) << 16 | ((byte)(orderLayer + 128)) << 8;
        // Remaps into [0..1] range.
        return (float)GameMath.Remap(packedLayerDepth, int.MinValue, int.MaxValue, 0f, 1f);
    }
}