using System.Collections.Generic;
using MonoGame.Extended;

namespace Flatlanders.Core.Physics;

public interface ISpaceAlgorithm
{
    void Insert(ICollider actor);
    bool Remove(ICollider actor);
    IEnumerable<ICollider> Query(RectangleF boundsBoundingRectangle);
    List<ICollider>.Enumerator GetEnumerator();
    void Reset();
}
