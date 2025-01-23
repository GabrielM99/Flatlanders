namespace Flatlanders.Core.Components;

public abstract class Layout(Entity entity) : Component(entity)
{
    protected virtual void Build() { }

    public override void OnCreate()
    {
        base.OnCreate();
        Build();
        Entity.ChildrenSizeChanged += OnEntityChildrenSizeChanged;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Entity.ChildrenSizeChanged -= OnEntityChildrenSizeChanged;
    }

    private void OnEntityChildrenSizeChanged(Entity entity)
    {
        Build();
    }
}