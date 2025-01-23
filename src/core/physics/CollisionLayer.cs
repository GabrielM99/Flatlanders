using System;

namespace Flatlanders.Core.Physics;

public class CollisionLayer
{
    public bool IsDynamic { get; set; } = true;

    public readonly ISpaceAlgorithm Space;

    public CollisionLayer(ISpaceAlgorithm spaceAlgorithm)
    {
        Space = spaceAlgorithm ?? throw new ArgumentNullException(nameof(spaceAlgorithm));
    }

    public virtual void Reset()
    {
        if (IsDynamic)
            Space.Reset();
    }
}
