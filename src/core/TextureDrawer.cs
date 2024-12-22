using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public struct TextureDrawer : IDrawer
{
    public Texture2D Texture { get; set; }
    public Transform Transform { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Color Color { get; set; }
    public Vector2 Origin { get; set; }
    public SpriteEffects Effects { get; set; }
    public short Layer { get; set; }

    public readonly void Draw(SpriteBatch spriteBatch, RectangleF bounds, float layerDepth)
    {
        // PIVOT for WORLD!
        spriteBatch.Draw(Texture, bounds.Center, SourceRectangle, Color, Transform.Rotation, Origin + SourceRectangle.Size.ToVector2() * 0.5f, Transform.Scale, Effects, layerDepth);
    }
}