using System;
using Flatlanders.Core.Physics;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public abstract class Layout(Entity entity) : Component(entity), ISizable
{
    public Vector2 Size { get; protected set; }

    protected virtual void Build() { }

    private RectangleCollider Collider { get; set; }

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
        Collider.Size = Size;
        Console.WriteLine(Collider.Bounds);
    }
}