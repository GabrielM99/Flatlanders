using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core.Graphics.Drawers;

public readonly struct RectangleDrawer(float thickness) : IDrawer
{
    public readonly void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth)
    {
        spriteBatch.DrawRectangle(transform.Position, transform.Size, color, thickness, layerDepth);
    }
}