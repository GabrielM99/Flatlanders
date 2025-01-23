using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Graphics;

public class Sprite
{
    public Texture2D Texture { get; }
    public Rectangle Rectangle { get; }
    public Vector2 Origin { get; }

    public Sprite(Texture2D texture, Rectangle? rectangle = null, Vector2 origin = default)
    {
        Texture = texture;
        Rectangle = rectangle == null ? new Rectangle(0, 0, texture.Width, texture.Height) : rectangle.Value;
        Origin = origin;
    }
}