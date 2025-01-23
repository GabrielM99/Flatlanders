using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Flatlanders.Core.Prefabs;

namespace Flatlanders.Core;

public class Entity
{
    public const int InvalidEntityID = -1;

    public event Action<Component> ComponentAdded;
    public event Action<Component> ComponentRemoved;

    public string Name { get; set; }
    public Engine Engine { get; }
    public Node Node { get; }

    internal int ID { get; set; } = InvalidEntityID;

    private Dictionary<Type, Component> ComponentByType { get; }

    public Entity(Engine engine, string name = "")
    {
        Name = name;
        Engine = engine;
        ComponentByType = new Dictionary<Type, Component>();
        Node = AddComponent<Node>();
    }
    
    public void OnUpdate(float deltaTime)
    {
        
    }

    public Entity CreateChild(string name = "")
    {
        Entity child = Engine.EntityManager.CreateEntity(name);
        OnCreateChild(child);
        return child;
    }

    public Entity CreateChild(Prefab prefab, string name = "")
    {
        Entity child = Engine.EntityManager.CreateEntity(prefab, name);
        OnCreateChild(child);
        return child;
    }

    public T AddComponent<T>() where T : Component
    {
        T component = Engine.EntityManager.CreateComponent<T>(this);
        ComponentByType[typeof(T)] = component;
        ComponentAdded?.Invoke(component);
        return component;
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = null;

        // Search by the specific type.
        if (ComponentByType.TryGetValue(typeof(T), out Component uncastComponent))
        {
            component = (T)uncastComponent;
            return true;
        }

        // Search by derivative types.
        foreach (Component tempUncastComponent in ComponentByType.Values)
        {
            if (tempUncastComponent is T castComponent)
            {
                component = castComponent;
                return true;
            }
        }

        return component != null;
    }

    public T GetComponent<T>() where T : Component
    {
        TryGetComponent(out T component);
        return component;
    }

    public IEnumerable<Component> GetComponents()
    {
        return ComponentByType.Values;
    }

    public void RemoveComponent<T>()
    {
        if (ComponentByType.Remove(typeof(T), out Component component))
        {
            RemoveComponent(component);
        }
    }

    public void RemoveComponents()
    {
        foreach (Component component in ComponentByType.Values)
        {
            RemoveComponent(component);
        }

        ComponentByType.Clear();
    }

    public void Destroy()
    {
        Engine.EntityManager.DestroyEntity(this);
        RemoveComponents();
    }

    private void RemoveComponent(Component component)
    {
        Engine.EntityManager.DestroyComponent(component);
        ComponentRemoved?.Invoke(component);
    }

    private void OnCreateChild(Entity child)
    {
        Node.AddChild(child.Node);
    }
}