using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public class TextRenderer : Renderer
{
    private SpriteFont _font;
    private string _text;

    public SpriteFont Font
    {
        get => _font;

        set
        {
            if (value != _font)
            {
                CalculateTransformSize(Text);
            }

            _font = value;
        }
    }

    public string Text
    {
        get => _text;

        set
        {
            if (value != _text)
            {
                CalculateTransformSize(value);
            }

            _text = value;
        }
    }

    public TextRenderer(Entity entity) : base(entity)
    {
    }

    public override void Draw(Graphics graphics)
    {
        graphics.DrawText(Font, Text, Entity.Transform, Color, Effects, Layer);
    }

    // TODO: Make calculating a Transform's size be generic.
    private void CalculateTransformSize(string text)
    {
        Size = Font == null ? Vector2.Zero : Font.MeasureString(text);
    }
}