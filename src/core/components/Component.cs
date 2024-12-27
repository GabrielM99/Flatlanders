namespace Flatlanders.Core.Components;

public abstract class Component
{
    public Engine Engine { get; }
    public Entity Entity { get; }
    public int ID { get; internal set; }

    public Component(Entity entity)
    {
        Engine = entity.Engine;
        Entity = entity;
    }

    public virtual void OnCreate() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnDestroy() { }
}