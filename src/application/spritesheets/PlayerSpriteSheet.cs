using Flatlanders.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.SpriteSheets;

public class PlayerSpriteSheet : SpriteSheet
{
    public Sprite HeadSprite { get; }
    public Sprite[] ChestSprites { get; }
    public Sprite[] LegsSprites { get; }
    public Sprite[] FeetSprites { get; }
    public Sprite HairSprite { get; }

    public PlayerSpriteSheet(Texture2D texture) : base(texture)
    {
        Vector2 offset = new(0, 8);
        HeadSprite = Slice(0, 0, 16, 16, offset);
        ChestSprites = Slice(0, 32, 64, 16, 4, 1, offset);
        LegsSprites = Slice(0, 48, 80, 16, 5, 1, offset);
        FeetSprites = Slice(0, 64, 80, 16, 5, 1, offset);
        HairSprite = Slice(0, 80, 16, 16, offset);
    }
}