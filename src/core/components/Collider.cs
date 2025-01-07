using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Flatlanders.Core.Components;

public abstract class Collider : Component, ICollisionActor
{
    public abstract IShapeF Bounds { get; }

    public override int Order => -1;

    public string LayerName { get; } = Physics.DEFAULT_LAYER_NAME;

    public Collider(Entity entity) : base(entity)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Engine.Physics.AddCollider(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Engine.Physics.RemoveCollider(this);
    }

    public void OnCollision(CollisionEventArgs collisionInfo)
    {

    }

    public int Cast(in CollisionEventArgs[] collisionInfo, Vector2 offset = default)
    {
        IShapeF shape = Bounds;
        shape.Position += offset;
        return Engine.Physics.CastCollider(shape, in collisionInfo, LayerName, this);
    }
}
