using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public class Transform : Component, ISizableComponent
{
    public event Action<Transform> ChildAdded;
    public event Action<Transform> ChildRemoved;
    public event Action<Transform> VolumeChanged;

    private Transform _parent;

    private static Vector2[] AnchorPositions { get; } = new Vector2[]
    {
        new(-1f, -1f),  new(0f, -1f),   new(1f, -1f),
        new(-1f, 0f),   new(0f, 0f),    new(1f, 0f),
        new(-1f, 1f),   new(0f, 1f),    new(1f, 1f)
    };

    public RectangleF Bounds => new(LocalPosition + LocalBounds.Position, LocalBounds.Size);
    public RectangleF LocalBounds
    {
        get
        {
            Vector2 size = Size;

            if (Space == TransformSpace.World)
            {
                size = Engine.Graphics.ViewToWorldVector(size);
            }

            return new(Vector2.Multiply(-Pivot, size * 0.5f) - size * 0.5f + (Parent == null ? Vector2.Zero : Parent.Bounds.Center + Vector2.Multiply(AnchorPosition, Parent.Bounds.Size * 0.5f)), size);
        }
    }

    public Transform Parent
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
    public Transform Root { get; private set; }

    public TransformSpace Space { get; set; } = TransformSpace.World;

    public TransformAnchor Anchor { get; set; } = TransformAnchor.Center;
    public Vector2 AnchorPosition => GetAnchorPosition(Anchor);

    public Vector2 Pivot { get; set; }

    public Vector2 LocalPosition { get; set; }
    public Vector2 Position
    {
        get => LocalPosition + (Parent == null ? Vector2.Zero : Parent.Position);
        set => LocalPosition = value - (Parent == null ? Vector2.Zero : Parent.Position);
    }

    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;

    private Vector2 _volume;
    public Vector2 Volume
    {
        get => _volume;

        private set
        {
            if (value != _volume)
            {
                _volume = value;
                VolumeChanged?.Invoke(this);
            }
        }
    }

    private List<Transform> Children { get; }

    // Transforms will always update their sizes in the first frame.
    private bool IsUpdateSizePending { get; set; } = true;
    private List<ISizableComponent> SizableComponents { get; }

    public Transform(Entity entity) : base(entity)
    {
        Children = new List<Transform>();
        SizableComponents = new List<ISizableComponent>();
        Root = this;
    }

    private static Vector2 GetAnchorPosition(TransformAnchor anchor)
    {
        return AnchorPositions[(int)anchor];
    }

    public override void OnCreate()
    {
        base.OnCreate();

        Entity.ComponentAdded += OnEntityComponentAdded;
        Entity.ComponentRemoved += OnEntityComponentRemoved;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        ProcessPendingUpdateSize();
        Engine.Graphics.DrawRectangle(this, Color.Red, short.MaxValue);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Entity.ComponentAdded -= OnEntityComponentAdded;
        Entity.ComponentRemoved -= OnEntityComponentRemoved;
    }

    public void AddChild(Transform child)
    {
        if (child != this)
        {
            child._parent = this;
            child.Root = Root;
            Children.Add(child);
            OnChildAdded(child);
        }
    }

    public void RemoveChild(Transform child)
    {
        child._parent = null;
        child.Root = child;
        Children.Remove(child);
        OnChildRemoved(child);
    }

    public Transform GetChild(int index)
    {
        return Children[index];
    }

    public int GetChildCount()
    {
        return Children.Count;
    }

    public IEnumerable<Transform> GetChildren()
    {
        return Children;
    }

    private void OnChildAdded(Transform child)
    {
        ChildAdded?.Invoke(child);
        UpdateSize();
    }

    private void OnChildRemoved(Transform child)
    {
        ChildRemoved?.Invoke(child);
        UpdateSize();
    }

    private void OnEntityComponentSizeChanged(Component component)
    {
        UpdateSize();
    }

    private void OnEntityComponentAdded(Component component)
    {
        component.SizeChanged += OnEntityComponentSizeChanged;

        if (component is ISizableComponent sizableComponent)
        {
            SizableComponents.Add(sizableComponent);
        }
    }

    private void OnEntityComponentRemoved(Component component)
    {
        component.SizeChanged -= OnEntityComponentSizeChanged;

        if (component is ISizableComponent sizableComponent)
        {
            SizableComponents.Remove(sizableComponent);
        }
    }

    private void UpdateSize()
    {
        IsUpdateSizePending = true;
    }

    private void OnUpdateSize()
    {
        Vector2 size = Vector2.Zero;
        Vector2 volume = Vector2.Zero;

        foreach (ISizableComponent sizableComponent in SizableComponents)
        {
            if (sizableComponent != this)
            {
                size = Vector2.Max(size, sizableComponent.Size);
            }
        }

        foreach (Transform child in Children)
        {
            size = Vector2.Max(size, Vector2.Multiply(child.Size, child.Scale));
            volume += child.Size;
        }

        Volume = volume;
        Size = Vector2.Multiply(size, Scale);

        Parent?.UpdateSize();
    }

    private void ProcessPendingUpdateSize()
    {
        if (IsUpdateSizePending)
        {
            IsUpdateSizePending = false;
            OnUpdateSize();
        }
    }
}