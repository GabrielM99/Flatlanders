using System;

namespace Flatlanders.Core;

public static class GameMath
{
    public static double Remap(double value, double oldMin, double oldMax, double newMin, double newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    public static float Repeat(float value, float length)
    {
        return value - (float)Math.Floor(value / length) * length;
    }
}