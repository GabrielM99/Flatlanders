using Flatlanders.Application.Components;
using Flatlanders.Application.Databases;
using Flatlanders.Application.SpriteSheets;
using Flatlanders.Core;

namespace Flatlanders.Application.Animations;

public class PlayerWalkAnimation : Animation<Player>
{
    public override int Frames => 0;

    private AnimationProperty<Sprite> HeadSprite { get; }
    private AnimationProperty<Sprite> ChestSprite { get; }
    private AnimationProperty<Sprite> LegsSprite { get; }
    private AnimationProperty<Sprite> FeetSprite { get; }
    private AnimationProperty<Sprite> HairSprite { get; }

    public PlayerWalkAnimation(Engine engine) : base(engine)
    {
        SpriteSheetDatabase spriteSheetDatabase = engine.DatabaseManager.GetDatabase<SpriteSheetDatabase>();
        PlayerSpriteSheet playerSpriteSheet = spriteSheetDatabase.Player;

        HeadSprite = new AnimationProperty<Sprite>(this);
        HeadSprite.SetKeyframe(0, playerSpriteSheet.HeadSprite);

        ChestSprite = new AnimationProperty<Sprite>(this);
        ChestSprite.SetKeyframe(0, playerSpriteSheet.ChestSprites[0]);

        LegsSprite = new AnimationProperty<Sprite>(this);
        LegsSprite.SetKeyframe(0, playerSpriteSheet.LegsSprites[0]);

        FeetSprite = new AnimationProperty<Sprite>(this);
        FeetSprite.SetKeyframe(0, playerSpriteSheet.FeetSprites[0]);

        HairSprite = new AnimationProperty<Sprite>(this);
        HairSprite.SetKeyframe(0, playerSpriteSheet.HairSprite);
    }

    public override void Bind(RuntimeAnimation runtimeAnimation, Player obj)
    {
        runtimeAnimation.BindProperty(HeadSprite, (value) => obj.HeadSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(ChestSprite, (value) => obj.ChestSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(LegsSprite, (value) => obj.LegsSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(FeetSprite, (value) => obj.FeetSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(HairSprite, (value) => obj.HairSpriteRenderer.Sprite = value);
    }
}