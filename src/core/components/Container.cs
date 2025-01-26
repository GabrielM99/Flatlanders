namespace Flatlanders.Core.Components;

public abstract class Container(Entity entity) : Layout(entity)
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
}