using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

// TODO: Text wrapping options (support for break line and masking text based on parent size)
public class TextRenderer : Renderer, ISizable
{
    private SpriteFont _font;
    private string _text = "Text";

    public override Vector2 Size => Text == null || Font == null ? Vector2.Zero : Font.MeasureString(Text);

    public SpriteFont Font
    {
        get => _font;

        set
        {
            if (value != _font)
            {
                _font = value;
                Entity.Node.RecalculateSize();
            }
        }
    }

    public string Text
    {
        get => _text;

        set
        {
            if (value != _text)
            {
                _text = value;
                Entity.Node.RecalculateSize();
            }
        }
    }

    public TextRenderer(Entity entity) : base(entity)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Font = Engine.Content.Load<SpriteFont>("Arial");
    }

    public override void OnDraw(Graphics graphics, short layer, SpriteEffects effects, Vector2 sortingOrigin = default)
    {
        graphics.DrawText(Font, Text, Entity.Node, Color, effects, layer, sortingOrigin);
    }
}