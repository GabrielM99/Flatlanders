using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Flatlanders.Core.Components;

public class RectangleCollider : Collider, ICollisionActor
{
    public override IShapeF Bounds => new RectangleF(Entity.Node.Position - Size * 0.5f + Offset, Size);

    public Vector2 Size { get; set; } = Vector2.One;
    public Vector2 Offset { get; set; } = Vector2.Zero;

    public RectangleCollider(Entity entity) : base(entity)
    {
    }
}
