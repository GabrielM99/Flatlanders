using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

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
                Entity.Transform.RecalculateSize();
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
                Entity.Transform.RecalculateSize();
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

    public override void Draw(Graphics graphics)
    {
        graphics.DrawText(Font, Text, Entity.Transform, Color, Effects, Layer);
    }
}