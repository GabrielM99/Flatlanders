using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace Flatlanders.Core.Components;

public class Rigidbody : Component
{
    private const int CollisionBufferSize = 8;
    
    private readonly CollisionEventArgs[] _collisionBuffer;
    
    public override int Order => 1;

    public Vector2 Velocity { get; set; }

    private Collider Collider { get; set; }

    public Rigidbody(Entity entity) : base(entity)
    {
        _collisionBuffer = new CollisionEventArgs[CollisionBufferSize];
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
            }

            int collisionCountY = Collider.Cast(in _collisionBuffer, offset.Y * Vector2.UnitY);

            if (collisionCountY > 0)
            {
                offset.Y = 0f;
            }
        }

        Entity.Node.Position += offset;
    }
}