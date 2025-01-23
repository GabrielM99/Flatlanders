using Flatlanders.Core.Graphics;
using Flatlanders.Core.Graphics.Drawers;
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
                Entity.RecalculateSize();
            }
        }
    }

    public override Vector2 Size => Sprite == null ? Vector2.Zero : Sprite.Rectangle.Size.ToVector2();

    public override void OnDraw(RenderManager renderManager, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        renderManager.Draw(Entity, new SpriteDrawer(Sprite, Effects), Color, layer, sortingOrigin, order);
    }
}