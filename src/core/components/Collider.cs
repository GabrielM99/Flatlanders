using Flatlanders.Core.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public abstract class Collider(Entity entity) : Component(entity), ICollider
{
    public abstract IShapeF Bounds { get; }

    public override int Order => -1;

    public string LayerName { get; } = PhysicsManager.DEFAULT_LAYER_NAME;

    public override void OnCreate()
    {
        base.OnCreate();
        Engine.PhysicsManager.AddCollider(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Engine.PhysicsManager.RemoveCollider(this);
    }

    public void OnCollision(CollisionInfo collisionInfo)
    {

    }

    public int Cast(in CollisionInfo[] collisionInfo, Vector2 offset = default)
    {
        IShapeF shape = Bounds;
        shape.Position += offset;
        return Engine.PhysicsManager.CastCollider(shape, in collisionInfo, LayerName, this);
    }
}
