using Flatlanders.Application.Components;
using Flatlanders.Core;
using Flatlanders.Core.Animations;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application.Animations;

public class PlayerBlinkAnimation : Animation<Player>
{
    public override int FrameCount => 8;
    public override bool IsLoopable => false;

    private AnimationProperty<Vector2> EyebrowsLocalPosition { get; }

    public PlayerBlinkAnimation(Engine engine) : base(engine)
    {
        EyebrowsLocalPosition = new AnimationProperty<Vector2>(this);
        EyebrowsLocalPosition.SetKeyframe(0, Vector2.Zero);
        EyebrowsLocalPosition.SetKeyframe(4, Vector2.UnitY * 0.0625f);
    }

    public override void Bind(RuntimeAnimation runtimeAnimation, Player obj)
    {
        runtimeAnimation.BindProperty(EyebrowsLocalPosition, (value) => obj.EyebrowsSpriteRenderer.Entity.Node.LocalPosition = value);
    }
}