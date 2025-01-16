using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public readonly struct RectangleDrawer(float thickness) : IDrawer
{
    public readonly void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth)
    {
        spriteBatch.DrawRectangle(transform.Position, transform.Size, color, thickness, layerDepth);
    }
}