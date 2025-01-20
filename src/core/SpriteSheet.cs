using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core;

public class SpriteSheet
{
    private Texture2D Texture { get; }

    public SpriteSheet(Texture2D texture)
    {
        Texture = texture;
    }

    public Sprite Slice(int x, int y, int width, int height, Vector2 origin = default)
    {
        return new Sprite(Texture, new Rectangle(x, y, width, height), origin);
    }

    public Sprite[] Slice(int x, int y, int width, int height, int cols, int rows, Vector2 origin = default)
    {
        int spriteWidth = width / cols;
        int spriteHeight = height / rows;

        Sprite[] sprites = new Sprite[cols * rows];

        int index = 0;

        for (int i = 0; i < width; i += spriteWidth)
        {
            for (int j = 0; j < height; j += spriteHeight)
            {
                sprites[index] = Slice(x + i, y + j, spriteWidth, spriteHeight, origin);
                index++;
            }
        }

        return sprites;
    }

    public Sprite[] Slice(int cols, int rows, Vector2 origin = default)
    {
        return Slice(0, 0, Texture.Width, Texture.Height, cols, rows, origin);
    }
}