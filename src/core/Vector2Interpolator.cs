using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public struct Vector2Interpolator : IAnimationInterpolator<Vector2>
{
    public readonly Vector2 Interpolate(Vector2 a, Vector2 b, float t)
    {
        return a + (b - a) * t;
    }
}