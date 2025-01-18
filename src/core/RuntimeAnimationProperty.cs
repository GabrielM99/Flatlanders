using System;

namespace Flatlanders.Core;

public abstract class RuntimeAnimationProperty
{
    private int _keyframeIndex;

    public abstract void OnEvaluateFrame(ref int keyframeIndex, int frame);
    public abstract void OnEvaluateBlend(int keyframeIndex, int frame, float t);

    public void OnEvaluateFrame(int frame)
    {
        OnEvaluateFrame(ref _keyframeIndex, frame);
    }

    public void OnEvaluateBlend(int frame, float t)
    {
        OnEvaluateBlend(_keyframeIndex, frame, t);
    }
}

public class RuntimeAnimationProperty<T>(AnimationProperty<T> property, Action<T> valueChanged) : RuntimeAnimationProperty
{
    private AnimationProperty<T> Property { get; } = property;
    private Action<T> ValueChanged { get; } = valueChanged;

    public override void OnEvaluateFrame(ref int keyframeIndex, int frame)
    {
        ValueChanged?.Invoke(Property.EvaluateFrame(ref keyframeIndex, frame));
    }

    public override void OnEvaluateBlend(int keyframeIndex, int frame, float t)
    {
        ValueChanged?.Invoke(Property.EvaluateBlend(keyframeIndex, frame, t));
    }
}