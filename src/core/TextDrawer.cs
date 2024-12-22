using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public struct TextDrawer : IDrawer
{
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public Transform Transform { get; set; }
    public Color Color { get; set; }
    public Vector2 Origin { get; set; }
    public SpriteEffects Effects { get; set; }
    public short Layer { get; set; }

    public readonly void Draw(SpriteBatch spriteBatch, RectangleF bounds, float layerDepth)
    {
        spriteBatch.DrawString(Font, Text, bounds.Position, Color, Transform.Rotation, Origin, Transform.Scale, Effects, layerDepth);
    }
}