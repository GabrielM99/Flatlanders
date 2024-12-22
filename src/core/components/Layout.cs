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
        Entity.Transform.VolumeChanged += OnTransformVolumeChanged;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Entity.Transform.VolumeChanged -= OnTransformVolumeChanged;
    }

    private void OnTransformVolumeChanged(Transform transform)
    {
        Build();
    }
}