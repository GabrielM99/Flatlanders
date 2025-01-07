using Flatlanders.Core;
using Flatlanders.Core.Components;

namespace Flatlanders.Application.Components;

public class PlayerCamera : Component
{
    public override int Order => 2;

    public Node Target { get; set; }

    public PlayerCamera(Entity entity) : base(entity)
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Target != null)
        {
            Entity.Node.Position = Target.Position;
        }
    }
}