using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public abstract class Renderer : Component, ISizable
{
    public override int Order => 1;

    public Color Color { get; set; } = Color.White;
    public SpriteEffects Effects { get; set; }
    public short Layer { get; set; }

    public abstract Vector2 Size { get; }

    public Renderer(Entity entity) : base(entity)
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        Draw(Engine.Graphics);
    }

    public abstract void Draw(Graphics graphics);
}