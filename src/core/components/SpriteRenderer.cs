using System;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class SpriteRenderer : Renderer
{
    private Sprite _sprite;

    public Sprite Sprite
    {
        get => _sprite;

        set
        {
            if (!Sprite.Equals(value))
            {
                _sprite = value;
                Entity.Node.RecalculateSize();
            }
        }
    }

    public override Vector2 Size => Sprite.Rectangle.Size.ToVector2();

    public SpriteRenderer(Entity entity) : base(entity)
    {
    }

    public override void Draw(Graphics graphics)
    {
        Engine.Graphics.DrawSprite(Sprite, Entity.Node, Color, Effects, Layer);
    }
}