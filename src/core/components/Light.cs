using Flatlanders.Core.Graphics.Lighting;
using Microsoft.Xna.Framework;
using Penumbra;

namespace Flatlanders.Core.Components;

public abstract class Light : Component
{
    private Vector2 _position;
    private Vector2 _scale;

    public override int Order => ComponentOrder.Graphics;

    public Vector2 Position
    {
        get => _position;

        set
        {
            _position = value;
            // Lighting works with window coordinates to maintain the high quality of shadows.
            Data.Position = Engine.RenderManager.WorldToWindowVector(value);
        }
    }
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            // Lighting works with window coordinates to maintain the high quality of shadows.
            Data.Scale = Engine.RenderManager.WorldToWindowVector(value) * Range;
        }
    }
    public float Rotation { get => Data.Rotation; set => Data.Rotation = value; }

    public float Intensity { get => Data.Intensity; set => Data.Intensity = value; }
    public float Falloff { get => Data.Radius; set => Data.Radius = value; }
    public Color Color { get => Data.Color; set => Data.Color = value; }

    public bool CastShadows { get => Data.CastsShadows; set => Data.CastsShadows = value; }
    public LightShadowMode ShadowMode { get => (LightShadowMode)Data.ShadowType; set => Data.ShadowType = (ShadowType)value; }

    public float Range { get; set; } = 1f;

    public Penumbra.Light Data { get; }

    public Light(Entity entity) : base(entity)
    {
        Data = CreateData();
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Engine.RenderManager.AddLight(this);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // TODO: Event for transform changes.
        Position = Entity.Position;
        Rotation = Entity.Rotation;
        Scale = GameMath.Abs(Entity.Scale);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Engine.RenderManager.RemoveLight(this);
    }

    protected abstract Penumbra.Light CreateData();
}