using System;

namespace Flatlanders.Core;

public static class GameMath
{
    public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    public static float Repeat(float value, float length)
    {
        return value - (float)Math.Floor(value / length) * length;
    }
}