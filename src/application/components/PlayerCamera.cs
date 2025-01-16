using System;
using Flatlanders.Core;
using Flatlanders.Core.Components;

namespace Flatlanders.Application.Components;

public class PlayerCamera(Entity entity) : Component(entity)
{
    public override int Order => ComponentOrder.Physics + 1;

    public Node Target { get; set; }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Target != null)
        {
            Entity.Node.Position = Target.Position;
        }
    }
}