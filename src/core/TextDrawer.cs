using System;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core;

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

        spriteBatch.DrawString(font, text, transform.Position - transform.Size * 0.5f, color, transform.Rotation, Vector2.Zero, new Vector2(Math.Abs(transform.Scale.X), Math.Abs(transform.Scale.Y)), scaledEffects, layerDepth);
    }
}