using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Flatlanders.Core.Prefabs;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class EntityManager : GameComponent
{
    private Engine Engine { get; }
    private List<Component> Components { get; }

    public EntityManager(Engine engine) : base(engine)
    {
        Engine = engine;
        Components = new();
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

        for (int i = 0; i < Components.Count; i++)
        {
            Components[i].OnUpdate(deltaTime);
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
        component.ID = Components.Count;
        Components.Add(component);
        component.OnCreate();
    }

    private void OnDestroyComponent(Component component, bool cleanUp = true)
    {
        component.OnDestroy();

        if (cleanUp)
        {
            Component lastComponent = Components[^1];
            lastComponent.ID = component.ID;
            Components[component.ID] = lastComponent;
            Components.RemoveAt(Components.Count - 1);
        }
    }

    private void Clear()
    {
        for (int i = 0; i < Components.Count; i++)
        {
            OnDestroyComponent(Components[i], false);
        }

        Components.Clear();
    }

    private void OnSceneLoaded(Scene scene)
    {
        Clear();
    }
}