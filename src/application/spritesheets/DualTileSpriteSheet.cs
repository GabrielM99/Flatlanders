using System;
using Flatlanders.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.SpriteSheets;

public class DualGridTileSpriteSheet : SpriteSheet
{
    private const int RequiredSize = 64;

    public Sprite[] Sprites { get; }

    public DualGridTileSpriteSheet(Texture2D texture) : base(texture)
    {
        if (texture.Height != RequiredSize || texture.Width != RequiredSize)
        {
            throw new Exception($"{nameof(DualGridTileSpriteSheet)} doesn't have the required size.");
        }

        Sprites = new Sprite[16];
        Sprites[0] = new Sprite(texture, new Rectangle(16 * 0, 16 * 3, 16, 16));
        Sprites[1] = new Sprite(texture, new Rectangle(16 * 1, 16 * 3, 16, 16));
        Sprites[2] = new Sprite(texture, new Rectangle(16 * 0, 16 * 0, 16, 16));
        Sprites[3] = new Sprite(texture, new Rectangle(16 * 3, 16 * 0, 16, 16));
        Sprites[4] = new Sprite(texture, new Rectangle(16 * 0, 16 * 2, 16, 16));
        Sprites[5] = new Sprite(texture, new Rectangle(16 * 1, 16 * 0, 16, 16));
        Sprites[6] = new Sprite(texture, new Rectangle(16 * 2, 16 * 3, 16, 16));
        Sprites[7] = new Sprite(texture, new Rectangle(16 * 1, 16 * 1, 16, 16));
        Sprites[8] = new Sprite(texture, new Rectangle(16 * 3, 16 * 3, 16, 16));
        Sprites[9] = new Sprite(texture, new Rectangle(16 * 0, 16 * 1, 16, 16));
        Sprites[10] = new Sprite(texture, new Rectangle(16 * 3, 16 * 2, 16, 16));
        Sprites[11] = new Sprite(texture, new Rectangle(16 * 2, 16 * 0, 16, 16));
        Sprites[12] = new Sprite(texture, new Rectangle(16 * 1, 16 * 2, 16, 16));
        Sprites[13] = new Sprite(texture, new Rectangle(16 * 2, 16 * 2, 16, 16));
        Sprites[14] = new Sprite(texture, new Rectangle(16 * 3, 16 * 1, 16, 16));
        Sprites[15] = new Sprite(texture, new Rectangle(16 * 2, 16 * 1, 16, 16));
    }
}