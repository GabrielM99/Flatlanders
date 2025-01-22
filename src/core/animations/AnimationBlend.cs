using System;

namespace Flatlanders.Core.Animations;

public class AnimationBlend(RuntimeAnimation startRuntimeAnimation, RuntimeAnimation endRuntimeAnimation, float duration)
{
    public RuntimeAnimation StartRuntimeAnimation { get; } = startRuntimeAnimation;
    public RuntimeAnimation EndRuntimeAnimation { get; } = endRuntimeAnimation;

    public float Duration { get; } = duration;

    public float Time { get; set; }
    public float NormalizedTime => Time / Duration;

    public void OnUpdate(float deltaTime)
    {
        Time = Math.Clamp(Time + deltaTime, 0f, Duration);

        foreach (RuntimeAnimationProperty property in StartRuntimeAnimation.GetProperties())
        {
            property.OnEvaluateBlend(StartRuntimeAnimation.Frame, NormalizedTime);
        }
    }
}
