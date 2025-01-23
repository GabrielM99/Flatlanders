using Flatlanders.Core.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public class RectangleCollider(Entity entity) : Collider(entity), ICollider
{
    public override IShapeF Bounds => new RectangleF(Entity.Position - Size * 0.5f + Offset, Size);

    public Vector2 Size { get; set; } = Vector2.One;
    public Vector2 Offset { get; set; } = Vector2.Zero;
}
