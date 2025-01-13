using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application.Animations;

public class NodeSwingAnimation : Animation<Node>
{
    public override int Frames => 20;

    private AnimationProperty<Vector2> LocalPosition { get; }

    public NodeSwingAnimation()
    {
        LocalPosition = new(this);
        LocalPosition.SetKeyframe(0, Vector2.Zero);
        LocalPosition.SetKeyframe(10, new Vector2(100f, 100f));
        LocalPosition.SetKeyframe(20, new Vector2(200f, 0f));
        LocalPosition.SetKeyframe(30, Vector2.Zero);
    }

    public override void Bind(RuntimeAnimation runtimeAnimation, Node obj)
    {
        runtimeAnimation.BindProperty(LocalPosition, (value) => obj.LocalPosition = value);
    }
}