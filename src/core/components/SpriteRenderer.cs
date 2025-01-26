using Flatlanders.Core.Graphics;
using Flatlanders.Core.Graphics.Drawers;
using Flatlanders.Core.Transforms;
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

    public override Vector2 GetSize(TransformSpace space)
    {
        if (Sprite == null)
        {
            return Vector2.Zero;
        }

        Vector2 screenSize = Sprite.Rectangle.Size.ToVector2();
        return space == TransformSpace.World ? Engine.RenderManager.ScreenToWorldVector(screenSize) : screenSize;
    }

    public override void OnDraw(RenderManager renderManager, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        renderManager.Draw(Entity, new SpriteDrawer(Sprite, Effects), Color, layer, sortingOrigin, order);
    }
}