using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application.Animations;

public class NodeSwingAnimation : Animation<Node>
{
    public override void Bind(Node obj)
    {
        AnimationProperty<Vector2> LocalPosition = new(this, (value) => obj.Entity.Node.LocalPosition = value);
        LocalPosition.AddFrame(0, Vector2.Zero);
        LocalPosition.AddFrame(15, Vector2.UnitY * 100f);
        LocalPosition.AddFrame(30, Vector2.Zero);

        AddProperty(LocalPosition);
    }
}