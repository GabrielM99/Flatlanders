using System;
using Flatlanders.Core;
using Flatlanders.Core.Components;

namespace Flatlanders.Application.Components;

public class PlayerCamera(Entity entity) : Component(entity)
{
    public override int Order => ComponentOrder.Physics + 1;

    public Entity Target { get; set; }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Target != null)
        {
            Entity.Position = Target.Position;
        }
    }
}