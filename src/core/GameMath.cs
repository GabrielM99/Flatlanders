namespace Flatlanders.Core;

public static class GameMath
{
    public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }
}