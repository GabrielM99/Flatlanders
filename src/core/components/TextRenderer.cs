using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class TextRenderer : Renderer
{
    private SpriteFont _font;
    private string _text = "Text";

    public SpriteFont Font
    {
        get => _font;

        set
        {
            if (value != _font)
            {
                _font = value;
                UpdateSize();
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
                UpdateSize();
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

    private void UpdateSize()
    {
        Size = Font == null ? Vector2.Zero : Font.MeasureString(Text);
    }
}