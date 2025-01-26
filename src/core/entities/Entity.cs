using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;
using Flatlanders.Core.Graphics.Drawers;
using Flatlanders.Core.Prefabs;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class Entity : ITransform
{
    public const int InvalidEntityID = -1;

    public event Action<Component> ComponentAdded;
    public event Action<Component> ComponentRemoved;

    public event Action<Entity> ChildAdded;
    public event Action<Entity> ChildRemoved;
    public event Action<Entity> ChildrenSizeChanged;

    public string Name { get; set; }
    public Engine Engine { get; }

    internal int ID { get; set; } = InvalidEntityID;

    private Dictionary<Type, Component> ComponentByType { get; }

    private Entity _parent;

    // TODO: Perhaps move these to Layout.cs?
    private Vector2 _minSize;
    public Vector2 MinSize
    {
        get => _minSize;

        set
        {
            if (value != _minSize)
            {
                _minSize = value;
                RecalculateSize();
            }
        }
    }
    private Vector2? _maxSize;
    public Vector2? MaxSize
    {
        get => _maxSize;

        set
        {
            if (value != _maxSize)
            {
                _maxSize = value;
                RecalculateSize();
            }
        }
    }

    // TODO: Check if setting a size is really supported.
    public Vector2 LocalSize { get; set; }
    public Vector2 Size { get => LocalSize; set => LocalSize = value - (Parent == null ? Vector2.Zero : Parent.Size); }

    public Entity Parent
    {
        get => _parent;

        set
        {
            if (value != Parent && value != this)
            {
                Parent?.RemoveChild(this);
                _parent = value;
                value?.AddChild(this);
            }
        }
    }

    // TODO: Implement rerooting.
    public ITransform Root { get; set; }
    public TransformSpace Space { get; set; } = TransformSpace.World;
    public TransformAnchor Anchor { get; set; } = TransformAnchor.Center;

    public Vector2 Pivot { get; set; }

    public Vector2 LocalPosition { get; set; }
    public Vector2 Position
    {
        get => Vector2.Multiply(-Pivot, Size * 0.5f) + (Parent == null ? LocalPosition : Vector2.Multiply(LocalPosition, Parent.Scale) + Parent.Position + Vector2.Multiply(ITransform.GetAnchorPosition(Anchor), Parent.Size * 0.5f));
        set => LocalPosition = Parent == null ? value : value - Parent.Position;
    }

    public float LocalRotation { get; set; }
    public float Rotation
    {
        get => Parent == null ? LocalRotation : LocalRotation * Parent.Scale.X + Parent.Rotation;
        set => LocalRotation = Parent == null ? value : value - Parent.Rotation;
    }

    public Vector2 LocalScale { get; set; } = Vector2.One;
    public Vector2 Scale
    {
        get => Vector2.Multiply(LocalScale, Parent == null ? Vector2.One : Parent.Scale);
        set => LocalScale = Vector2.Divide(value, Parent == null ? Vector2.One : Parent.Scale);
    }

    private Vector2 _childrenSize;
    public Vector2 ChildrenSize
    {
        get => _childrenSize;

        private set
        {
            if (value != _childrenSize)
            {
                _childrenSize = value;
                ChildrenSizeChanged?.Invoke(this);
            }
        }
    }
    private List<Entity> Children { get; }

    private bool IsRecalculateSizePending { get; set; }
    private List<ISizable> SizableComponents { get; }

    public Entity(Engine engine, string name = "")
    {
        Name = name;
        Engine = engine;
        ComponentByType = [];
        Children = [];
        SizableComponents = [];
        // By default, entities are their own roots.
        Root = this;
        // Entities will always recalculate their sizes upon start.
        IsRecalculateSizePending = true;
    }

    public void OnUpdate(float deltaTime)
    {
        ProcessPendingRecalculateSize();

        if (Engine.HasDebugFlag(EngineDebugFlags.DrawTransforms))
        {
            Engine.RenderManager.Draw(this, new RectangleDrawer(1f), Color.White, sbyte.MaxValue);
        }
    }

    public Entity CreateChild(string name = "")
    {
        Entity child = Engine.EntityManager.CreateEntity(name);
        AddChild(child);
        return child;
    }

    public Entity CreateChild(Prefab prefab, string name = "")
    {
        Entity child = Engine.EntityManager.CreateEntity(prefab, name);
        AddChild(child);
        return child;
    }

    public T AddComponent<T>() where T : Component
    {
        T component = Engine.EntityManager.CreateComponent<T>(this);
        AddComponent(component);
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


    public void AddChild(Entity child)
    {
        if (child != this)
        {
            child._parent = this;
            child.Root = Root;
            Children.Add(child);
            OnChildAdded(child);
        }
    }

    public void RemoveChild(Entity child)
    {
        child._parent = null;
        child.Root = child;
        Children.Remove(child);
        OnChildRemoved(child);
    }

    public Entity GetChild(int index)
    {
        return Children[index];
    }

    public int GetChildCount()
    {
        return Children.Count;
    }

    public IEnumerable<Entity> GetChildren()
    {
        return Children;
    }

    public void Destroy()
    {
        Engine.EntityManager.DestroyEntity(this);
        RemoveComponents();

        foreach (Entity child in Children.ToArray())
        {
            child.Destroy();
        }

        Parent?.RemoveChild(this);
    }

    private void AddComponent(Component component)
    {
        ComponentByType[component.GetType()] = component;
        ComponentAdded?.Invoke(component);

        if (component is ISizable sizable)
        {
            SizableComponents.Add(sizable);
        }
    }

    private void RemoveComponent(Component component)
    {
        Engine.EntityManager.DestroyComponent(component);
        ComponentRemoved?.Invoke(component);

        if (component is ISizable sizable)
        {
            SizableComponents.Remove(sizable);
        }
    }

    private void OnChildAdded(Entity child)
    {
        ChildAdded?.Invoke(child);
        RecalculateSize();
    }

    private void OnChildRemoved(Entity child)
    {
        ChildRemoved?.Invoke(child);
        RecalculateSize();
    }

    public void RecalculateSize()
    {
        IsRecalculateSizePending = true;
    }

    private void OnRecalculateSize()
    {
        Vector2 size = Vector2.Zero;
        Vector2 childrenSize = Vector2.Zero;

        foreach (ISizable sizableComponent in SizableComponents)
        {
            if (sizableComponent != this)
            {
                size = Vector2.Max(size, sizableComponent.GetSize(Space));
            }
        }

        foreach (Entity child in Children)
        {
            size = Vector2.Max(size, child.Size);
            childrenSize += child.Size;
        }

        if (MaxSize != null)
        {
            size = Vector2.Min(size, MaxSize.Value);
        }

        ChildrenSize = childrenSize;
        Size = Vector2.Multiply(Vector2.Max(size, MinSize), Scale);

        Parent?.RecalculateSize();
    }

    private void ProcessPendingRecalculateSize()
    {
        if (IsRecalculateSizePending)
        {
            IsRecalculateSizePending = false;
            OnRecalculateSize();
        }
    }
}