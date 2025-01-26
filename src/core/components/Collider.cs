using System;
using Flatlanders.Core.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public abstract class Collider(Entity entity) : Component(entity), ICollider
{
    public event Action<CollisionInfo> Collided;

    private string _layerName = PhysicsManager.DefaultLayerName;

    public abstract IShapeF Bounds { get; }

    public override int Order => -1;

    public string LayerName
    {
        get => _layerName;

        set
        {
            if (value != _layerName)
            {
                Engine.PhysicsManager.RemoveCollider(this);
                _layerName = value;
                Engine.PhysicsManager.AddCollider(this);
            }
        }
    }

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
        Collided?.Invoke(collisionInfo);
    }

    public int Cast(in CollisionInfo[] collisionInfo, Vector2 offset = default)
    {
        IShapeF shape = Bounds;
        shape.Position += offset;
        return Engine.PhysicsManager.CastCollider(shape, in collisionInfo, LayerName, this);
    }
}
