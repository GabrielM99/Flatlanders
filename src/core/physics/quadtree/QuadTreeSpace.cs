using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended;

namespace Flatlanders.Core.Physics;

public class QuadTreeSpace: ISpaceAlgorithm
{
    private readonly QuadTree _collisionTree;
    private readonly List<ICollider> _actors;
    private readonly Dictionary<ICollider, QuadtreeData> _targetDataDictionary;

    public QuadTreeSpace(RectangleF boundary)
    {
        _actors = [];
        _targetDataDictionary = [];
        _collisionTree = new QuadTree(boundary);
    }

    public void Insert(ICollider target)
    {
        if (!_targetDataDictionary.ContainsKey(target))
        {
            var data = new QuadtreeData(target);
            _targetDataDictionary.Add(target, data);
            _collisionTree.Insert(data);
            _actors.Add(target);
        }
    }

    public bool Remove(ICollider target)
    {
        if (_targetDataDictionary.ContainsKey(target))
        {
            var data = _targetDataDictionary[target];
            data.RemoveFromAllParents();
            _targetDataDictionary.Remove(target);
            _collisionTree.Shake();
            _actors.Remove(target);
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _collisionTree.ClearAll();
        foreach (var value in _targetDataDictionary.Values)
        {
            _collisionTree.Insert(value);
        }
        _collisionTree.Shake();
    }

    public List<ICollider>.Enumerator GetEnumerator() => _actors.GetEnumerator();

    public IEnumerable<ICollider> Query(RectangleF boundsBoundingRectangle)
    {
        return _collisionTree.Query(ref boundsBoundingRectangle).Select(x => x.Target);
    }
}
