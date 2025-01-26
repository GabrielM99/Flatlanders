using System;
using Flatlanders.Core.Physics;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public abstract class Layout(Entity entity) : Component(entity), ISizable
{
    private RectangleCollider Collider { get; set; }

    public virtual Vector2 GetSize(TransformSpace space)
    {
        return default;
    }

    protected virtual void Build() { }

    public override void OnCreate()
    {
        base.OnCreate();

        Collider = Entity.AddComponent<RectangleCollider>();
        Collider.LayerName = PhysicsManager.UILayerName;

        Build();

        Entity.ChildrenSizeChanged += OnEntityChildrenSizeChanged;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Entity.ChildrenSizeChanged -= OnEntityChildrenSizeChanged;
    }

    private void OnEntityChildrenSizeChanged(Entity entity)
    {
        Build();
        Collider.Size = GetSize(Entity.Space);
    }
}