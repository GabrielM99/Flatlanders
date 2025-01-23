using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Graphics.Drawers;

public readonly struct TextDrawer(string text, SpriteFont font, SpriteEffects effects) : IDrawer
{
    public readonly void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth)
    {
        if (string.IsNullOrEmpty(text) || font == null)
        {
            return;
        }

        SpriteEffects scaledEffects = effects;

        if (transform.Scale.X < 0f)
        {
            scaledEffects ^= SpriteEffects.FlipHorizontally;
        }

        if (transform.Scale.Y < 0f)
        {
            scaledEffects ^= SpriteEffects.FlipVertically;
        }

        spriteBatch.DrawString(font, text, transform.Position - transform.Size * 0.5f, color, transform.Rotation, Vector2.Zero, GameMath.Abs(transform.Scale), scaledEffects, layerDepth);
    }
}