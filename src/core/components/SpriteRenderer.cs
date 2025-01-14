using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class SpriteRenderer : Renderer
{
    private Sprite _sprite;

    public Sprite Sprite
    {
        get => _sprite;

        set
        {
            if (Sprite != value)
            {
                _sprite = value;
                Entity.Node.RecalculateSize();
            }
        }
    }

    public override Vector2 Size => Sprite == null ? Vector2.Zero : Sprite.Rectangle.Size.ToVector2();

    public SpriteRenderer(Entity entity) : base(entity)
    {
    }

    public override void OnDraw(Graphics graphics, short layer, SpriteEffects effects, Vector2 sortingOrigin = default)
    {
        Engine.Graphics.DrawSprite(Sprite, Entity.Node, Color, effects, layer, sortingOrigin);
    }
}