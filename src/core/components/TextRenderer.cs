using Flatlanders.Core.Graphics;
using Flatlanders.Core.Graphics.Drawers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

// TODO: Text wrapping options (support for break line and masking text based on parent size)
public class TextRenderer(Entity entity) : Renderer(entity), ISizable
{
    private string _text = "Text";
    private SpriteFont _font;
    
    // TODO: Check performance.
    public override Vector2 Size => Text == null || Font == null ? Vector2.Zero : Font.MeasureString(Text);

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

    public override void OnDraw(RenderManager renderManager, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        renderManager.Draw(Entity, new TextDrawer(Text, Font, Effects), Color, layer, sortingOrigin, order);
    }
}