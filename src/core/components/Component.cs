namespace Flatlanders.Core.Components;

public abstract class Component
{
    public virtual int Order => ComponentOrder.Default;
    
    public Engine Engine { get; }
    public Entity Entity { get; }

    public Component(Entity entity)
    {
        Engine = entity.Engine;
        Entity = entity;
    }

    public virtual void OnCreate() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnDestroy() { }
}