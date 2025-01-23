using MonoGame.Extended;

namespace Flatlanders.Core.Physics
{
    public interface ICollider
    {
        string LayerName { get => null; }
        IShapeF Bounds { get; }
        void OnCollision(CollisionInfo collisionInfo);
    }
}
