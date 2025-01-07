using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public abstract class Layout : Component
{
    public Layout(Entity entity) : base(entity)
    {
    }

    protected virtual void Build() { }

    public override void OnCreate()
    {
        base.OnCreate();
        Build();
        Entity.Node.ChildrenSizeChanged += OnTransformChildrenSizeChanged;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Entity.Node.ChildrenSizeChanged -= OnTransformChildrenSizeChanged;
    }

    private void OnTransformChildrenSizeChanged(Node node)
    {
        Build();
    }
}