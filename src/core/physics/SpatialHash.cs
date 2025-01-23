using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MonoGame.Extended;

namespace Flatlanders.Core.Physics;

public class SpatialHash : ISpaceAlgorithm
{
    public List<ICollider>.Enumerator GetEnumerator() => _actors.GetEnumerator();

    private readonly Dictionary<int, List<ICollider>> _dictionary;
    private readonly List<ICollider> _actors;
    private readonly SizeF _size;

    public SpatialHash(SizeF size)
    {
        _dictionary = [];
        _actors = [];
        _size = size;
    }

    public void Insert(ICollider actor)
    {
        InsertToHash(actor);
        _actors.Add(actor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InsertToHash(ICollider actor)
    {
        var rect = actor.Bounds.BoundingRectangle;
        for (var x = rect.Left; x < rect.Right; x += _size.Width)
            for (var y = rect.Top; y < rect.Bottom; y += _size.Height)
                AddToCell(x, y, actor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddToCell(float x, float y, ICollider actor)
    {
        var index = GetIndex(x, y);
        if (_dictionary.TryGetValue(index, out var actors))
            actors.Add(actor);
        else
            _dictionary[index] = new() { actor };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(float x, float y)
    {
        return (int)(x / _size.Width) << 16 + (int)(y / _size.Height);
    }

    public bool Remove(ICollider actor)
    {
        foreach (var actors in _dictionary.Values)
            actors.Remove(actor);
        return _actors.Remove(actor);
    }

    public IEnumerable<ICollider> Query(RectangleF boundsBoundingRectangle)
    {
        var results = new HashSet<ICollider>();
        var bounds = boundsBoundingRectangle.BoundingRectangle;

        for (var x = boundsBoundingRectangle.Left; x < boundsBoundingRectangle.Right; x += _size.Width)
            for (var y = boundsBoundingRectangle.Top; y < boundsBoundingRectangle.Bottom; y += _size.Height)
                if (_dictionary.TryGetValue(GetIndex(x, y), out var actors))
                    foreach (var actor in actors)
                        if (bounds.Intersects(actor.Bounds))
                            results.Add(actor);
        return results;
    }

    public void Reset()
    {
        _dictionary.Clear();
        foreach (var actor in _actors)
            InsertToHash(actor);
    }
}
