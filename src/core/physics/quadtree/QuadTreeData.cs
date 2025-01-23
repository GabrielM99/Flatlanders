using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended;

namespace Flatlanders.Core.Physics;

public class QuadtreeData
{
    private readonly ICollider _target;
    private readonly HashSet<QuadTree> _parents;

    public QuadtreeData(ICollider target)
    {
        _target = target;
        _parents = [];
        Bounds = _target.Bounds.BoundingRectangle;
    }

    public void RemoveParent(QuadTree parent)
    {
        _parents.Remove(parent);
    }

    public void AddParent(QuadTree parent)
    {
        _parents.Add(parent);
        Bounds = _target.Bounds.BoundingRectangle;
    }

    public void RemoveFromAllParents()
    {
        foreach (var parent in _parents.ToList())
        {
            parent.Remove(this);
        }

        _parents.Clear();
    }

    public RectangleF Bounds { get; set; }

    public ICollider Target => _target;

    public bool Dirty { get; private set; }

    public void MarkDirty()
    {
        Dirty = true;
    }

    public void MarkClean()
    {
        Dirty = false;
    }
}
