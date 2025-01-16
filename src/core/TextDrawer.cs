using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core;

public readonly struct TextDrawer(string text, SpriteFont font, SpriteEffects effects) : IDrawer
{
    public readonly void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth)
    {
        spriteBatch.DrawString(font, text, transform.Position - transform.Size * 0.5f, color, transform.Rotation, Vector2.Zero, transform.Scale, effects, layerDepth);
    }
}