using System;
using Flatlanders.Application.Animations;
using Flatlanders.Application.Databases;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Application.Components;

public class Player(Entity entity) : Component(entity)
{
    public Tilemap tilemap;
    public Tile rockTile;
    public TextRenderer debugTextRenderer;

    public RendererGroup RendererGroup { get; private set; }
    public SpriteRenderer HairSpriteRenderer { get; set; }
    public SpriteRenderer EyebrowsSpriteRenderer { get; set; }
    public SpriteRenderer EyesBackSpriteRenderer { get; set; }
    public SpriteRenderer EyesSpriteRenderer { get; set; }
    public SpriteRenderer HeadSpriteRenderer { get; set; }
    public SpriteRenderer ChestSpriteRenderer { get; set; }
    public SpriteRenderer LegsSpriteRenderer { get; set; }
    public SpriteRenderer FeetSpriteRenderer { get; set; }
    public SpriteRenderer LeftHandSpriteRenderer { get; set; }
    public SpriteRenderer RightHandSpriteRenderer { get; set; }

    private Rigidbody Rigidbody { get; set; }
    private Animator Animator { get; set; }
    private AudioPlayer AudioPlayer { get; set; }

    private PlayerIdleAnimation PlayerIdleAnimation { get; set; }
    private PlayerWalkAnimation PlayerWalkAnimation { get; set; }
    private PlayerBlinkAnimation PlayerBlinkAnimation { get; set; }

    private SoundEffect FootstepAudio { get; set; }

    private Random Random { get; set; }

    public override void OnCreate()
    {
        base.OnCreate();

        // TODO: Static randomness.
        Random = new Random();

        // TODO: Order shouldn't matter here.
        RectangleCollider playerCollider = Entity.AddComponent<RectangleCollider>();
        playerCollider.Size = new Vector2(0.25f);
        playerCollider.Offset = new Vector2(0f, -0.125f);

        Rigidbody = Entity.AddComponent<Rigidbody>();
        Animator = Entity.AddComponent<Animator>();
        AudioPlayer = Entity.AddComponent<AudioPlayer>();

        FootstepAudio = Engine.Content.Load<SoundEffect>("GrassFootstep1");

        SpriteDatabase spriteDatabase = Engine.DatabaseManager.GetDatabase<SpriteDatabase>();

        Color skinColor = new(0.933333337f, 0.847058833f, 0.7019608f);

        Entity graphicsEntity = Entity.CreateChild();
        graphicsEntity.Node.LocalPosition = -Vector2.UnitY * 0.0625f * 8f;

        HeadSpriteRenderer = graphicsEntity.CreateChild().AddComponent<SpriteRenderer>();
        HeadSpriteRenderer.Color = skinColor;

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

        ChestSpriteRenderer = graphicsEntity.CreateChild().AddComponent<SpriteRenderer>();
        ChestSpriteRenderer.Color = new Color(0.807843149f, 0.572549045f, 0.282352984f);
        ChestSpriteRenderer.Layer = 1;

        LegsSpriteRenderer = graphicsEntity.CreateChild().AddComponent<SpriteRenderer>();
        LegsSpriteRenderer.Color = new Color(0.3019608f, 0.207843155f, 0.2f);

        FeetSpriteRenderer = graphicsEntity.CreateChild().AddComponent<SpriteRenderer>();
        FeetSpriteRenderer.Color = new Color(0.129411772f, 0.129411772f, 0.129411772f);

        LeftHandSpriteRenderer = graphicsEntity.CreateChild().AddComponent<SpriteRenderer>();
        LeftHandSpriteRenderer.Sprite = spriteDatabase.Hand;
        LeftHandSpriteRenderer.Layer = 2;
        LeftHandSpriteRenderer.Color = skinColor;

        RightHandSpriteRenderer = graphicsEntity.CreateChild().AddComponent<SpriteRenderer>();
        RightHandSpriteRenderer.Sprite = spriteDatabase.Hand;
        RightHandSpriteRenderer.Layer = -1;
        RightHandSpriteRenderer.Color = skinColor;

        RendererGroup = Entity.AddComponent<RendererGroup>();
        RendererGroup.AddRenderers(HairSpriteRenderer, EyebrowsSpriteRenderer, EyesBackSpriteRenderer, EyesSpriteRenderer, HeadSpriteRenderer, ChestSpriteRenderer, LegsSpriteRenderer, FeetSpriteRenderer, LeftHandSpriteRenderer, RightHandSpriteRenderer);

        AnimationDatabase animationDatabase = Engine.DatabaseManager.GetDatabase<AnimationDatabase>();
        PlayerIdleAnimation = animationDatabase.PlayerIdle;
        PlayerWalkAnimation = animationDatabase.PlayerWalk;
        PlayerBlinkAnimation = animationDatabase.PlayerBlink;
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

        if (Rigidbody.Velocity == Vector2.Zero)
        {
            Animator.PlayAnimation(PlayerIdleAnimation, this, 0.1f);
        }
        else
        {
            Animator.PlayAnimation(PlayerWalkAnimation, this, 0.1f);
        }

        if (Random.NextSingle() <= 0.01f)
        {
            Animator.PlayAnimation(PlayerBlinkAnimation, this, 0.1f, 1);
        }

        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = mouseState.Position.ToVector2();
        Vector2 worldMousePosition = Engine.Graphics.ScreenToWorldVector(mousePosition);
        Vector2 worldMouseDirection = Vector2.Normalize(worldMousePosition - Entity.Node.Position);

        if (worldMouseDirection.X != 0f && !float.IsNaN(worldMouseDirection.X))
        {
            Entity.Node.Scale = new Vector2(Math.Sign(worldMouseDirection.X), 1f);
        }

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            tilemap.SetTile(null, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            tilemap.SetTile(rockTile, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        Rigidbody.Velocity = direction * 3f;
    }

    public void PlayFootstepAudio()
    {
        AudioPlayer.Play(FootstepAudio);
    }
}