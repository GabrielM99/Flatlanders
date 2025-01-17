using Flatlanders.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.SpriteSheets;

public class PlayerSpriteSheet : SpriteSheet
{
    public Sprite HeadSprite { get; }
    public Sprite EyebrowsSprite { get; }
    public Sprite EyesSprite { get; }
    public Sprite EyesBackSprite { get; }
    public Sprite[] ChestSprites { get; }
    public Sprite[] LegsSprites { get; }
    public Sprite[] FeetSprites { get; }
    public Sprite HairSprite { get; }

    public PlayerSpriteSheet(Texture2D texture) : base(texture)
    {
        HeadSprite = Slice(0, 0, 16, 16);
        EyesBackSprite = Slice(0, 16, 16, 16);
        EyesSprite = Slice(16, 16, 16, 16);
        EyebrowsSprite = Slice(16 * 2, 16, 16, 16);
        ChestSprites = Slice(0, 16 * 2, 16 * 4, 16, 4, 1);
        LegsSprites = Slice(0, 16 * 3, 16 * 5, 16, 5, 1);
        FeetSprites = Slice(0, 16 * 4, 16 * 5, 16, 5, 1);
        HairSprite = Slice(0, 16 * 5, 16, 16);
    }
}