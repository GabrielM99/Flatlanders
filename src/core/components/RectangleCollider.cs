using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Flatlanders.Core.Components;

public class RectangleCollider : Collider, ICollisionActor
{
    public override IShapeF Bounds => new RectangleF(Entity.Node.Bounds.Position + Offset - (Size - Vector2.One) * 0.5f, Size);

    public Vector2 Size { get; set; } = Vector2.One;
    public Vector2 Offset { get; set; } = Vector2.Zero;

    public RectangleCollider(Entity entity) : base(entity)
    {
    }
}
