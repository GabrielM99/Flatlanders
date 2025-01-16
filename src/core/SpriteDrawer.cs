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

        SpriteEffects newEffects = effects;

        if (transform.Scale.X < 0)
        {
            newEffects ^= SpriteEffects.FlipHorizontally;
        }

        if (transform.Scale.Y < 0)
        {
            newEffects ^= SpriteEffects.FlipVertically;
        }

        spriteBatch.Draw(sprite.Texture, transform.Position, sprite.Rectangle, color, transform.Rotation, sprite.Origin + sprite.Rectangle.Size.ToVector2() * 0.5f, new Vector2(Math.Abs(transform.Scale.X), Math.Abs(transform.Scale.Y)), newEffects, layerDepth);
    }
}