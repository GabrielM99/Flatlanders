using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public class Node : Component, ITransform, ISizable
{
    public event Action<Node> ChildAdded;
    public event Action<Node> ChildRemoved;
    public event Action<Node> ChildrenSizeChanged;

    private Node _parent;

    private static Vector2[] AnchorPositions { get; } =
    [
        new(-1f, -1f),  new(0f, -1f),   new(1f, -1f),
        new(-1f, 0f),   new(0f, 0f),    new(1f, 0f),
        new(-1f, 1f),   new(0f, 1f),    new(1f, 1f)
    ];

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
    public Vector2 Size { get; set; }

    public RectangleF Bounds
    {
        get
        {
            Vector2 size = Size;

            if (Space == TransformSpace.World)
            {
                size = Engine.Graphics.ViewToWorldVector(size);
            }

            return new(Position + Vector2.Multiply(-Pivot, size * 0.5f) - size * 0.5f, size);
        }
    }

    public Node Parent
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
    public Vector2 AnchorPosition => GetAnchorPosition(Anchor);

    public Vector2 Pivot { get; set; }

    public Vector2 LocalPosition { get; set; }
    public Vector2 Position
    {
        get => Parent == null ? LocalPosition : Vector2.Multiply(LocalPosition, Parent.Scale) + Parent.Bounds.Center + Vector2.Multiply(AnchorPosition, Parent.Bounds.Size * 0.5f);
        set => LocalPosition = value - (Parent == null ? Vector2.Zero : Parent.Position);
    }

    public float LocalRotation { get; set; }
    public float Rotation
    {
        get => Parent == null ? LocalRotation : LocalRotation * Parent.Scale.X + Parent.Rotation;
        set => LocalRotation = value - (Parent == null ? 0f : Parent.Rotation);
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
    private List<Node> Children { get; }

    private bool IsRecalculateSizePending { get; set; }
    private List<ISizable> SizableComponents { get; }

    public Node(Entity entity) : base(entity)
    {
        Children = new List<Node>();
        SizableComponents = new List<ISizable>();
        Root = this;
        // Nodes will always recalculate their sizes upon start.
        IsRecalculateSizePending = true;
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
        ProcessPendingRecalculateSize();
        //Engine.Graphics.DrawRectangle(this, Color.Red, short.MaxValue);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Entity.ComponentAdded -= OnEntityComponentAdded;
        Entity.ComponentRemoved -= OnEntityComponentRemoved;
    }

    public void AddChild(Node child)
    {
        if (child != this)
        {
            child._parent = this;
            child.Root = Root;
            Children.Add(child);
            OnChildAdded(child);
        }
    }

    public void RemoveChild(Node child)
    {
        child._parent = null;
        child.Root = child;
        Children.Remove(child);
        OnChildRemoved(child);
    }

    public Node GetChild(int index)
    {
        return Children[index];
    }

    public int GetChildCount()
    {
        return Children.Count;
    }

    public IEnumerable<Node> GetChildren()
    {
        return Children;
    }

    private void OnChildAdded(Node child)
    {
        ChildAdded?.Invoke(child);
        RecalculateSize();
    }

    private void OnChildRemoved(Node child)
    {
        ChildRemoved?.Invoke(child);
        RecalculateSize();
    }

    private void OnEntityComponentAdded(Component component)
    {
        if (component is ISizable sizable)
        {
            SizableComponents.Add(sizable);
        }
    }

    private void OnEntityComponentRemoved(Component component)
    {
        if (component is ISizable sizable)
        {
            SizableComponents.Remove(sizable);
        }
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
                size = Vector2.Max(size, sizableComponent.Size);
            }
        }

        foreach (Node child in Children)
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