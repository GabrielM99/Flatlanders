using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;

namespace Flatlanders.Core;

public class Entity
{
    public event Action<Component> ComponentAdded;
    public event Action<Component> ComponentRemoved;

    public string Name { get; set; }
    public Engine Engine { get; }
    public Node Node { get; }

    private Dictionary<Type, Component> ComponentByType { get; }

    public Entity(Engine engine, string name = "")
    {
        Name = name;
        Engine = engine;
        ComponentByType = new Dictionary<Type, Component>();
        Node = AddComponent<Node>();
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
        if (ComponentByType.TryGetValue(typeof(T), out Component component))
        {
            Engine.EntityManager.DestroyComponent(component);
            ComponentRemoved?.Invoke(component);
        }
    }
}