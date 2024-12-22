namespace Flatlanders.Core.Components;

public abstract class Container : Layout
{
    private float _spacing;

    public float Spacing
    {
        get => _spacing;

        set
        {
            if (value != _spacing)
            {
                _spacing = value;
                Build();
            }
        }
    }

    public Container(Entity entity) : base(entity)
    {
    }
}