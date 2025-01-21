using System;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class SpriteRenderer(Entity entity) : Renderer(entity)
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

    public override void OnDraw(Graphics graphics, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {        
        Engine.Graphics.Draw(Entity.Node, new SpriteDrawer(Sprite, Effects), Color, layer, sortingOrigin, order);
    }
}