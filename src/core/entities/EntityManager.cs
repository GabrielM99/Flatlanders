using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Flatlanders.Core.Prefabs;
using Flatlanders.Core.Scenes;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class EntityManager : GameComponent
{
    private enum ComponentRequestType
    {
        Register,
        Unregister
    }

    private readonly struct ComponentRequest(ComponentRequestType type, Component component)
    {
        public ComponentRequestType Type { get; } = type;
        public Component Component { get; } = component;
    }

    private List<Entity> Entities { get; }
    private SortedDictionary<int, HashSet<Component>> ComponentsByOrder { get; }
    private Queue<ComponentRequest> ComponentRequests { get; }

    private Engine Engine { get; }

    public EntityManager(Engine engine) : base(engine)
    {
        Engine = engine;
        Entities = [];
        ComponentsByOrder = [];
        ComponentRequests = [];
    }

    public override void Initialize()
    {
        Engine.SceneManager.SceneLoading += OnSceneLoading;
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float deltaTime = Engine.TimeManager.DeltaTime;

        foreach (Entity entity in Entities)
        {
            entity.OnUpdate(deltaTime);
        }

        foreach (HashSet<Component> components in ComponentsByOrder.Values)
        {
            foreach (Component component in components)
            {
                component.OnUpdate(deltaTime);
            }
        }

        ProcessComponentRequests();
    }

    public Entity CreateEntity(string name = "")
    {
        Entity entity = (Entity)Activator.CreateInstance(typeof(Entity), [Engine, name]);
        entity.ID = Entities.Count;
        Entities.Add(entity);
        return entity;
    }

    public Entity CreateEntity(Prefab prefab, string name = "")
    {
        return prefab.Create(this, name);
    }

    public void DestroyEntity(Entity entity)
    {
        int id = entity.ID;

        if (id == Entity.InvalidEntityID)
        {
            return;
        }

        Entity lastEntity = Entities[^1];
        Entities[id] = lastEntity;
        lastEntity.ID = id;
        entity.ID = Entity.InvalidEntityID;
        Entities.RemoveAt(Entities.Count - 1);
    }

    public T CreateComponent<T>(Entity entity) where T : Component
    {
        T component = (T)Activator.CreateInstance(typeof(T), [entity]);
        component.OnCreate();
        CreateComponentRequest(ComponentRequestType.Register, component);
        return component;
    }

    public void DestroyComponent(Component component, bool unregister = true)
    {
        component.OnDestroy();

        if (unregister)
        {
            CreateComponentRequest(ComponentRequestType.Unregister, component);
        }
    }

    private void CreateComponentRequest(ComponentRequestType type, Component component)
    {
        ComponentRequests.Enqueue(new ComponentRequest(type, component));
    }

    private void ProcessComponentRequests()
    {
        while (ComponentRequests.Count > 0)
        {
            ComponentRequest request = ComponentRequests.Dequeue();
            Component component = request.Component;

            switch (request.Type)
            {
                case ComponentRequestType.Register:
                    RegisterComponent(component);
                    break;
                case ComponentRequestType.Unregister:
                    UnregisterComponent(component);
                    break;
            }
        }
    }

    private void RegisterComponent(Component component)
    {
        int order = component.Order;

        if (!ComponentsByOrder.TryGetValue(order, out HashSet<Component> components))
        {
            components = [];
            ComponentsByOrder.Add(order, components);
        }

        components.Add(component);
    }

    private void UnregisterComponent(Component component)
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

    private void Clear()
    {
        foreach (HashSet<Component> components in ComponentsByOrder.Values)
        {
            foreach (Component component in components)
            {
                DestroyComponent(component, false);
            }
        }

        Entities.Clear();
        ComponentsByOrder.Clear();
        ComponentRequests.Clear();
    }

    private void OnSceneLoading(Scene scene)
    {
        Clear();
    }
}