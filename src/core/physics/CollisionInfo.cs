using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Physics
{
    public class CollisionInfo
    {
        public ICollider Other { get; internal set; }
        public Vector2 Penetration { get; internal set; }
    }
}