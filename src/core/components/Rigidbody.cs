using Flatlanders.Core.Physics;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class Rigidbody : Component
{
    private const int CollisionBufferSize = 8;

    private readonly CollisionInfo[] _collisionBuffer;

    public override int Order => ComponentOrder.Physics;

    public Vector2 Velocity { get; set; }

    private Collider Collider { get; set; }

    public Rigidbody(Entity entity) : base(entity)
    {
        _collisionBuffer = new CollisionInfo[CollisionBufferSize];
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Collider = Entity.GetComponent<Collider>();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Velocity == Vector2.Zero)
        {
            return;
        }

        Vector2 offset = Velocity * deltaTime;

        if (Collider != null)
        {
            int collisionCountX = Collider.Cast(in _collisionBuffer, offset.X * Vector2.UnitX);

            // TODO: This isn't the correct way of dealing with collision response. Ideally, we should subtract the penetration vector from the offset. 
            if (collisionCountX > 0)
            {
                offset.X = 0f;
                Velocity = Vector2.UnitY * Velocity.Y;
            }

            int collisionCountY = Collider.Cast(in _collisionBuffer, offset.Y * Vector2.UnitY);

            if (collisionCountY > 0)
            {
                offset.Y = 0f;
                Velocity = Vector2.UnitX * Velocity.X;
            }
        }

        Entity.Position += offset;
    }
}