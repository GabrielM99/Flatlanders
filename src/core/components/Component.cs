using System;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public abstract class Component
{
    public event Action<Component> SizeChanged;

    protected Vector2 _size;

    public Engine Engine { get; }
    public Entity Entity { get; }
    public int ID { get; internal set; }

    public Vector2 Size
    {
        get => _size;

        set
        {
            if (_size != value)
            {
                _size = value;
                OnSizeChanged();
            }
        }
    }

    public Component(Entity entity)
    {
        Engine = entity.Engine;
        Entity = entity;
    }

    public virtual void OnCreate() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnDestroy() { }

    private void OnSizeChanged()
    {
        SizeChanged?.Invoke(this);
    }
}