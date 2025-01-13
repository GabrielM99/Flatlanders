namespace Flatlanders.Core;

public interface IAnimationInterpolator<T>
{
    T Interpolate(T a, T b, float t);
}
