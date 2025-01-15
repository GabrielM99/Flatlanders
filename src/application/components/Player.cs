using System;
using Flatlanders.Application.Databases;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Flatlanders.Core.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Application.Components;

public class Player : Component
{
    public Tilemap tilemap;
    public Tile rockTile;
    public TextRenderer debugTextRenderer;

    public SpriteRenderer HairSpriteRenderer { get; set; }
    public SpriteRenderer EyebrowsSpriteRenderer { get; set; }
    public SpriteRenderer EyesBackSpriteRenderer { get; set; }
    public SpriteRenderer EyesSpriteRenderer { get; set; }
    public SpriteRenderer HeadSpriteRenderer { get; set; }
    public SpriteRenderer ChestSpriteRenderer { get; set; }
    public SpriteRenderer LegsSpriteRenderer { get; set; }
    public SpriteRenderer FeetSpriteRenderer { get; set; }

    private Rigidbody Rigidbody { get; set; }
    private Animator Animator { get; set; }

    public Player(Entity entity) : base(entity)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // TODO: Order shouldn't matter here.
        RectangleCollider playerCollider = Entity.AddComponent<RectangleCollider>();
        playerCollider.Size = new Vector2(0.25f);
        playerCollider.Offset = new Vector2(0f, -0.125f);

        Rigidbody = Entity.AddComponent<Rigidbody>();
        Animator = Entity.AddComponent<Animator>();

        AnimationDatabase animationDatabase = Engine.DatabaseManager.GetDatabase<AnimationDatabase>();
        Animator.PlayAnimation(animationDatabase.PlayerIdle, this);

        HeadSpriteRenderer = Entity.CreateChild().AddComponent<SpriteRenderer>();
        HeadSpriteRenderer.Color = new Color(0.933333337f, 0.847058833f, 0.7019608f);

        EyebrowsSpriteRenderer = HeadSpriteRenderer.Entity.CreateChild().AddComponent<SpriteRenderer>();
        EyebrowsSpriteRenderer.Color = new Color(0.34117648f, 0.2784314f, 0.141176462f);
        EyebrowsSpriteRenderer.Layer = 2;
        
        EyesBackSpriteRenderer = HeadSpriteRenderer.Entity.CreateChild().AddComponent<SpriteRenderer>();
        EyesBackSpriteRenderer.Layer = 1;

        EyesSpriteRenderer = HeadSpriteRenderer.Entity.CreateChild().AddComponent<SpriteRenderer>();
        EyesSpriteRenderer.Color = new Color(0.34117648f, 0.2784314f, 0.141176462f);
        EyesSpriteRenderer.Layer = 2;

        HairSpriteRenderer = HeadSpriteRenderer.Entity.CreateChild().AddComponent<SpriteRenderer>();
        HairSpriteRenderer.Color = new Color(0.34117648f, 0.2784314f, 0.141176462f);
        HairSpriteRenderer.Layer = 1;

        ChestSpriteRenderer = Entity.CreateChild().AddComponent<SpriteRenderer>();
        ChestSpriteRenderer.Color = new Color(0.807843149f, 0.572549045f, 0.282352984f);
        ChestSpriteRenderer.Layer = 1;

        LegsSpriteRenderer = Entity.CreateChild().AddComponent<SpriteRenderer>();
        LegsSpriteRenderer.Color = new Color(0.3019608f, 0.207843155f, 0.2f);

        FeetSpriteRenderer = Entity.CreateChild().AddComponent<SpriteRenderer>();
        FeetSpriteRenderer.Color = new Color(0.129411772f, 0.129411772f, 0.129411772f);

        RendererGroup rendererGroup = Entity.AddComponent<RendererGroup>();
        rendererGroup.AddRenderer(HairSpriteRenderer);
        rendererGroup.AddRenderer(EyebrowsSpriteRenderer);
        rendererGroup.AddRenderer(EyesBackSpriteRenderer);
        rendererGroup.AddRenderer(EyesSpriteRenderer);
        rendererGroup.AddRenderer(HeadSpriteRenderer);
        rendererGroup.AddRenderer(ChestSpriteRenderer);
        rendererGroup.AddRenderer(LegsSpriteRenderer);
        rendererGroup.AddRenderer(FeetSpriteRenderer);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        debugTextRenderer.Text = $"Flatlanders 0.0.1\nTPS:{(int)Engine.Time.TicksPerSecond}\nFPS:{(int)Engine.Time.FramesPerSecond}\nPOS: (X: {(int)Entity.Node.Position.X}, Y: {(int)Entity.Node.Position.Y})";

        KeyboardState keyboardState = Keyboard.GetState();

        Vector2 direction = Vector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            direction.Y -= 1;
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            direction.Y += 1;
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            direction.X -= 1;
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            direction.X += 1;
        }

        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = mouseState.Position.ToVector2();
        Vector2 worldMousePosition = Engine.Graphics.ScreenToWorldVector(mousePosition);

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            tilemap.SetTile(null, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            tilemap.SetTile(rockTile, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        Rigidbody.Velocity = direction * 5f;
    }
}