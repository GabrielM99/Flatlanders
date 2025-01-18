using System;
using Flatlanders.Application.Components;
using Flatlanders.Application.Databases;
using Flatlanders.Application.SpriteSheets;
using Flatlanders.Core;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application.Animations;

public class PlayerWalkAnimation : Animation<Player>
{
    public override int Frames => 30;

    private AnimationProperty<Sprite> HairSprite { get; }

    private AnimationProperty<Sprite> EyebrowsSprite { get; }
    private AnimationProperty<Sprite> EyesBackSprite { get; }
    private AnimationProperty<Sprite> EyesSprite { get; }

    private AnimationProperty<Sprite> HeadSprite { get; }
    private AnimationProperty<Vector2> HeadLocalPosition { get; }

    private AnimationProperty<Sprite> ChestSprite { get; }
    private AnimationProperty<Vector2> ChestLocalPosition { get; }

    private AnimationProperty<Sprite> LegsSprite { get; }
    private AnimationProperty<Sprite> FeetSprite { get; }

    private AnimationProperty<Vector2> LeftHandLocalPosition { get; }
    private AnimationProperty<float> LeftHandLocalRotation { get; }

    private AnimationProperty<Vector2> RightHandLocalPosition { get; }
    private AnimationProperty<float> RightHandLocalRotation { get; }    

    public PlayerWalkAnimation(Engine engine) : base(engine)
    {
        SpriteSheetDatabase spriteSheetDatabase = engine.DatabaseManager.GetDatabase<SpriteSheetDatabase>();
        PlayerSpriteSheet playerSpriteSheet = spriteSheetDatabase.Player;

        HairSprite = new AnimationProperty<Sprite>(this);
        HairSprite.SetKeyframe(0, playerSpriteSheet.HairSprite);

        HeadSprite = new AnimationProperty<Sprite>(this);
        HeadSprite.SetKeyframe(0, playerSpriteSheet.HeadSprite);

        HeadLocalPosition = new AnimationProperty<Vector2>(this);
        HeadLocalPosition.SetKeyframe(0, Vector2.Zero);
        HeadLocalPosition.SetKeyframe(8, -Vector2.UnitY * 0.0625f);
        HeadLocalPosition.SetKeyframe(16, Vector2.Zero);
        HeadLocalPosition.SetKeyframe(24, -Vector2.UnitY * 0.0625f);

        EyebrowsSprite = new AnimationProperty<Sprite>(this);
        EyebrowsSprite.SetKeyframe(0, playerSpriteSheet.EyebrowsSprite);

        EyesBackSprite = new AnimationProperty<Sprite>(this);
        EyesBackSprite.SetKeyframe(0, playerSpriteSheet.EyesBackSprite);

        EyesSprite = new AnimationProperty<Sprite>(this);
        EyesSprite.SetKeyframe(0, playerSpriteSheet.EyesSprite);

        ChestSprite = new AnimationProperty<Sprite>(this);
        ChestSprite.SetKeyframe(0, playerSpriteSheet.ChestSprites[0]);

        ChestLocalPosition = new AnimationProperty<Vector2>(this);
        ChestLocalPosition.SetKeyframe(0, Vector2.Zero);
        ChestLocalPosition.SetKeyframe(8, -Vector2.UnitY * 0.0625f);
        ChestLocalPosition.SetKeyframe(16, Vector2.Zero);
        ChestLocalPosition.SetKeyframe(24, -Vector2.UnitY * 0.0625f);

        LegsSprite = new AnimationProperty<Sprite>(this);
        LegsSprite.SetKeyframe(0, playerSpriteSheet.LegsSprites[0]);
        LegsSprite.SetKeyframe(8, playerSpriteSheet.LegsSprites[1]);
        LegsSprite.SetKeyframe(16, playerSpriteSheet.LegsSprites[0]);
        LegsSprite.SetKeyframe(24, playerSpriteSheet.LegsSprites[1]);

        FeetSprite = new AnimationProperty<Sprite>(this);
        FeetSprite.SetKeyframe(0, playerSpriteSheet.FeetSprites[0]);
        FeetSprite.SetKeyframe(8, playerSpriteSheet.FeetSprites[1]);
        FeetSprite.SetKeyframe(16, playerSpriteSheet.FeetSprites[0]);
        FeetSprite.SetKeyframe(24, playerSpriteSheet.FeetSprites[1]);

        LeftHandLocalPosition = new AnimationProperty<Vector2>(this);
        LeftHandLocalPosition.SetKeyframe(0, new Vector2(0.0625f * -2f, 0.0625f * 3f));
        LeftHandLocalPosition.SetKeyframe(8, new Vector2(0.0625f * -3.5f, 0.0625f * 2f));
        LeftHandLocalPosition.SetKeyframe(16, new Vector2(0.0625f * -2f, 0.0625f * 3f));
        LeftHandLocalPosition.SetKeyframe(24, new Vector2(0.0625f * 0f, 0.0625f * 2f));

        LeftHandLocalRotation = new AnimationProperty<float>(this);
        LeftHandLocalRotation.SetKeyframe(0, 0f);
        LeftHandLocalRotation.SetKeyframe(8, MathF.PI * 0.25f);
        LeftHandLocalRotation.SetKeyframe(16, 0f);
        LeftHandLocalRotation.SetKeyframe(24, -MathF.PI * 0.25f);

        RightHandLocalPosition = new AnimationProperty<Vector2>(this);
        RightHandLocalPosition.SetKeyframe(0, new Vector2(0.0625f * 3f, 0.0625f * 3f));
        RightHandLocalPosition.SetKeyframe(8, new Vector2(0.0625f * 4f, 0.0625f * 2f));
        RightHandLocalPosition.SetKeyframe(16, new Vector2(0.0625f * 3f, 0.0625f * 3f));
        RightHandLocalPosition.SetKeyframe(24, new Vector2(0.0625f * -1f, 0.0625f * 2f));

        RightHandLocalRotation = new AnimationProperty<float>(this);
        RightHandLocalRotation.SetKeyframe(0, 0f);
        RightHandLocalRotation.SetKeyframe(8, -MathF.PI * 0.25f);
        RightHandLocalRotation.SetKeyframe(16, 0);
        RightHandLocalRotation.SetKeyframe(24, MathF.PI * 0.25f);
    }

    public override void Bind(RuntimeAnimation runtimeAnimation, Player obj)
    {
        runtimeAnimation.BindProperty(HeadSprite, (value) => obj.HeadSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(HeadLocalPosition, (value) => obj.HeadSpriteRenderer.Entity.Node.LocalPosition = value);

        runtimeAnimation.BindProperty(EyebrowsSprite, (value) => obj.EyebrowsSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(EyesBackSprite, (value) => obj.EyesBackSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(EyesSprite, (value) => obj.EyesSpriteRenderer.Sprite = value);

        runtimeAnimation.BindProperty(ChestSprite, (value) => obj.ChestSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(ChestLocalPosition, (value) => obj.ChestSpriteRenderer.Entity.Node.LocalPosition = value);

        runtimeAnimation.BindProperty(LegsSprite, (value) => obj.LegsSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(FeetSprite, (value) => obj.FeetSpriteRenderer.Sprite = value);
        runtimeAnimation.BindProperty(HairSprite, (value) => obj.HairSpriteRenderer.Sprite = value);

        runtimeAnimation.BindProperty(LeftHandLocalPosition, (value) => obj.LeftHandSpriteRenderer.Entity.Node.LocalPosition = value);

        runtimeAnimation.BindProperty(LeftHandLocalRotation, (value) => obj.LeftHandSpriteRenderer.Entity.Node.LocalRotation = value);

        runtimeAnimation.BindProperty(RightHandLocalPosition, (value) => obj.RightHandSpriteRenderer.Entity.Node.LocalPosition = value);
        runtimeAnimation.BindProperty(RightHandLocalRotation, (value) => obj.RightHandSpriteRenderer.Entity.Node.LocalRotation = value);
    }
}