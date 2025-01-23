using Flatlanders.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public abstract class Renderer(Entity entity) : Component(entity), ISizable
{
    private RendererGroup _group;

    public override int Order => ComponentOrder.Graphics;

    public Color Color { get; set; } = Color.White;
    public SpriteEffects Effects { get; set; }
    public sbyte Layer { get; set; }

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

    public abstract void OnDraw(RenderManager graphics, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0);

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Group == null)
        {
            Draw(Layer);
        }
    }

    public void Draw(sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        OnDraw(Engine.RenderManager, layer, sortingOrigin, order);
    }
}