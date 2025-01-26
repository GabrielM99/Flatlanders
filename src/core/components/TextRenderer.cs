using Flatlanders.Core.Graphics;
using Flatlanders.Core.Graphics.Drawers;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

// TODO: Text wrapping options (support for break line and masking text based on parent size)
public class TextRenderer(Entity entity) : Renderer(entity), ISizable
{
    private string _text = "Text";
    private SpriteFont _font;

    public string Text
    {
        get => _text;

        set
        {
            if (value != _text)
            {
                _text = value;
                Entity.RecalculateSize();
            }
        }
    }

    public SpriteFont Font
    {
        get => _font;

        set
        {
            if (value != _font)
            {
                _font = value;
                Entity.RecalculateSize();
            }
        }
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Font = Engine.Content.Load<SpriteFont>("Arial");
    }

    public override Vector2 GetSize(TransformSpace space)
    {
        if (Text == null || Font == null)
        {
            return Vector2.Zero;
        }

        // TODO: Check performance.
        Vector2 screenSize = Font.MeasureString(Text);
        return space == TransformSpace.World ? Engine.RenderManager.ScreenToWorldVector(screenSize) : screenSize;
    }

    public override void OnDraw(RenderManager renderManager, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        renderManager.Draw(Entity, new TextDrawer(Text, Font, Effects), Color, layer, sortingOrigin, order);
    }
}