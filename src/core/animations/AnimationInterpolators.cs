using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Animations;

public static partial class AnimationInterpolators
{
    public static int Linear(int a, int b, float t)
    {
        return (int)(a + (b - a) * t);
    }

    public static int EaseInOut(int a, int b, float t)
    {
        return Linear(a, b, t * t * (3f - 2f * t));
    }

    public static float Linear(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static float EaseInOut(float a, float b, float t)
    {
        return Linear(a, b, t * t * (3f - 2f * t));
    }

    public static double Linear(double a, double b, float t)
    {
        return a + (b - a) * t;
    }

    public static double EaseInOut(double a, double b, float t)
    {
        return Linear(a, b, t * t * (3f - 2f * t));
    }

    public static Vector2 Linear(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    public static Vector2 EaseInOut(Vector2 a, Vector2 b, float t)
    {
        return Linear(a, b, t * t * (3f - 2f * t));
    }
}