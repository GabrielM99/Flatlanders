using System;
using Flatlanders.Application.Animations;
using Flatlanders.Application.Databases;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Flatlanders.Core.Graphics.Lighting;
using Flatlanders.Core.Inputs;
using Flatlanders.Core.Inputs.Bindings;
using Flatlanders.Core.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Application.Components;

public class Player(Entity entity) : Component(entity)
{
    private const float WalkSpeed = 2.5f;
    private const float RunSpeed = 3.75f;

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

    private InputAction MoveUpAction { get; set; }
    private InputAction MoveDownAction { get; set; }
    private InputAction MoveLeftAction { get; set; }
    private InputAction MoveRightAction { get; set; }
    private InputAction RunAction { get; set; }
    private InputAction PlaceAction { get; set; }
    private InputAction DestroyAction { get; set; }

    public override void OnCreate()
    {
        base.OnCreate();

        MoveUpAction = Engine.InputManager.CreateAction(new KeyboardBinding(Keys.W));
        MoveDownAction = Engine.InputManager.CreateAction(new KeyboardBinding(Keys.S));
        MoveLeftAction = Engine.InputManager.CreateAction(new KeyboardBinding(Keys.A));
        MoveRightAction = Engine.InputManager.CreateAction(new KeyboardBinding(Keys.D));
        RunAction = Engine.InputManager.CreateAction(new KeyboardBinding(Keys.LeftShift));
        PlaceAction = Engine.InputManager.CreateAction(new MouseBinding(MouseButton.Right));
        DestroyAction = Engine.InputManager.CreateAction(new MouseBinding(MouseButton.Left));

        // TODO: Static randomness.
        Random = new Random();

        PointLight light = Entity.AddComponent<PointLight>();
        light.Range = 10f;
        light.ShadowMode = LightShadowMode.Solid;

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
        graphicsEntity.LocalPosition = -Vector2.UnitY * 0.0625f * 8f;

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

        debugTextRenderer.Text = $"Flatlanders 0.0.1\nTPS:{(int)Engine.TimeManager.TicksPerSecond}\nFPS:{(int)Engine.TimeManager.FramesPerSecond}\nPOS: (X: {(int)Entity.Position.X}, Y: {(int)Entity.Position.Y})";

        float speed = WalkSpeed;

        Vector2 direction = Vector2.Zero;

        if (MoveUpAction.IsExecuting)
        {
            direction.Y -= 1;
        }

        if (MoveDownAction.IsExecuting)
        {
            direction.Y += 1;
        }

        if (MoveLeftAction.IsExecuting)
        {
            direction.X -= 1;
        }

        if (MoveRightAction.IsExecuting)
        {
            direction.X += 1;
        }

        if (RunAction.IsExecuting)
        {
            speed = RunSpeed;
        }

        if (Rigidbody.Velocity == Vector2.Zero)
        {
            Animator.PlayAnimation(PlayerIdleAnimation, this, 0, 0.1f);
        }
        else
        {
            Animator.PlayAnimation(PlayerWalkAnimation, this, 0, 0.1f, speed / WalkSpeed);
        }

        if (Random.NextSingle() <= 0.01f)
        {
            Animator.PlayAnimation(PlayerBlinkAnimation, this, 1, 0.1f);
        }

        Vector2 mousePosition = Engine.InputManager.MousePosition;
        Vector2 worldMousePosition = Engine.RenderManager.WindowToWorldPosition(mousePosition);
        Vector2 worldMouseDirection = Vector2.Normalize(worldMousePosition - Entity.Position);

        if (worldMouseDirection.X != 0f && !float.IsNaN(worldMouseDirection.X))
        {
            Entity.Scale = new Vector2(Math.Sign(worldMouseDirection.X), 1f);
        }

        if (PlaceAction.IsExecuting)
        {
            tilemap.SetTile(rockTile, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        if (DestroyAction.IsExecuting)
        {
            tilemap.SetTile(null, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        Rigidbody.Velocity = direction * speed;
    }

    public void PlayFootstepAudio()
    {
        AudioPlayer.Play(FootstepAudio);
    }
}