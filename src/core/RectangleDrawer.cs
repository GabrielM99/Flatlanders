using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public struct RectangleDrawer : IDrawer
{
    public ITransform Transform { get; set; }
    public Color Color { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 SortingOrigin { get; set; }
    public SpriteEffects Effects { get; set; }
    public short Layer { get; set; }

    public readonly void Draw(SpriteBatch spriteBatch, RectangleF bounds, float layerDepth)
    {
        spriteBatch.DrawRectangle(bounds.X + Origin.X, bounds.Y + Origin.Y, bounds.Width, bounds.Height, Color, 1f, layerDepth);
    }
}