using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core;

public readonly struct Sprite
{
    public Texture2D Texture { get; }
    public Rectangle Rectangle { get; }
    public Vector2 Origin { get; }

    public Sprite(Texture2D texture, Rectangle rectangle, Vector2 origin = default)
    {
        Texture = texture;
        Rectangle = rectangle;
        Origin = origin;
    }
}