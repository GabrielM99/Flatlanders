using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public static partial class GameMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Remap(double value, double oldMin, double oldMax, double newMin, double newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Repeat(float value, float length)
    {
        return value - (float)Math.Floor(value / length) * length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Abs(Vector2 vector)
    {
        vector.X = Math.Abs(vector.X);
        vector.Y = Math.Abs(vector.Y);
        return vector;
    }
}