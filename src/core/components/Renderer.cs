using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public abstract class Renderer : Component, ISizable
{
    private RendererGroup _group;

    public Color Color { get; set; } = Color.White;
    public SpriteEffects Effects { get; set; }
    public short Layer { get; set; }

    public RendererGroup Group
    {
        get => _group;

        set
        {
            if (_group != value)
            {
                _group = value;

                if (value == null)
                {
                    _group.RemoveRenderer(this);
                }
                else
                {
                    _group.AddRenderer(this);
                }
            }
        }
    }

    public abstract Vector2 Size { get; }

    public Renderer(Entity entity) : base(entity)
    {
    }

    public abstract void OnDraw(Graphics graphics, short layer, SpriteEffects effects = SpriteEffects.None, Vector2 sortingOrigin = default);

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Group == null)
        {
            Draw(Layer, Effects);
        }
    }

    public void Draw(short layer, SpriteEffects effects = SpriteEffects.None, Vector2 sortingOrigin = default)
    {
        OnDraw(Engine.Graphics, layer, effects, sortingOrigin);
    }
}