using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Flatlanders.Core.Components;
using Flatlanders.Core.Prefabs;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class EntityManager : GameComponent
{
    private Engine Engine { get; }
    private SortedDictionary<int, HashSet<Component>> ComponentsByOrder { get; }

    public EntityManager(Engine engine) : base(engine)
    {
        Engine = engine;
        ComponentsByOrder = new SortedDictionary<int, HashSet<Component>>();
    }

    public override void Initialize()
    {
        Engine.SceneManager.SceneLoaded += OnSceneLoaded;
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float deltaTime = Engine.Time.DeltaTime;
        
        foreach (HashSet<Component> components in ComponentsByOrder.Values)
        {
            foreach (Component component in components)
            {
                component.OnUpdate(deltaTime);
            }
        }
    }

    public Entity CreateEntity(string name = "")
    {
        return (Entity)Activator.CreateInstance(typeof(Entity), new object[] { Engine, name });
    }

    public Entity CreateEntity(Prefab prefab, string name = "")
    {
        return prefab.Create(this, name);
    }

    public T CreateComponent<T>(Entity entity) where T : Component
    {
        T component = (T)Activator.CreateInstance(typeof(T), new object[] { entity });
        OnCreateComponent(component);
        return component;
    }

    public void DestroyComponent(Component component)
    {
        OnDestroyComponent(component);
    }

    private void OnCreateComponent(Component component)
    {
        int order = component.Order;

        if (!ComponentsByOrder.TryGetValue(order, out HashSet<Component> components))
        {
            components = new HashSet<Component>();
            ComponentsByOrder.Add(order, components);
        }

        components.Add(component);
        component.OnCreate();
    }

    private void OnDestroyComponent(Component component, bool cleanUp = true)
    {
        component.OnDestroy();

        if (cleanUp)
        {
            int order = component.Order;

            if (ComponentsByOrder.TryGetValue(order, out HashSet<Component> components))
            {
                components.Remove(component);

                if (components.Count == 0)
                {
                    ComponentsByOrder.Remove(order);
                }
            }
        }
    }

    private void Clear()
    {
        foreach (HashSet<Component> components in ComponentsByOrder.Values)
        {
            foreach (Component component in components)
            {
                OnDestroyComponent(component, false);
            }
        }

        ComponentsByOrder.Clear();
    }

    private void OnSceneLoaded(Scene scene)
    {
        Clear();
    }
}