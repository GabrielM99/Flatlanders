using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application.Animations;

public class NodeDemoAnimation : Animation<Node>
{
    public override int Frames => 40;
    public override int FrameRate => 30;

    private AnimationProperty<Vector2> LocalPosition { get; }

    public NodeDemoAnimation(Engine engine) : base(engine)
    {
        AddDefaultInterpolator<Vector2>(AnimationInterpolators.EaseInOut);
        
        LocalPosition = new(this);
        LocalPosition.SetKeyframe(0, Vector2.Zero);
        LocalPosition.SetKeyframe(20, new Vector2(0f, 150f));
    }

    public override void Bind(RuntimeAnimation runtimeAnimation, Node obj)
    {
        runtimeAnimation.BindProperty(LocalPosition, (value) => obj.LocalPosition = value);
    }
}