using System;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core;

public readonly struct SpriteDrawer(Sprite sprite, SpriteEffects effects) : IDrawer
{
    public void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth)
    {
        if (sprite == null)
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

        spriteBatch.Draw(sprite.Texture, transform.Position, sprite.Rectangle, color, transform.Rotation, sprite.Origin + sprite.Rectangle.Size.ToVector2() * 0.5f, GameMath.Abs(transform.Scale), scaledEffects, layerDepth);
    }
}