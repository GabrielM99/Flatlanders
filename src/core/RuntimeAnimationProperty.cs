using System;

namespace Flatlanders.Core;

public abstract class RuntimeAnimationProperty
{
    private int _keyframeIndex;

    public abstract void OnEvaluateFrame(ref int keyframeIndex, int frameIndex);
    public abstract void OnEvaluateTransition(int keyframeIndex, int frameIndex, float t);

    public void OnEvaluateFrame(int frameIndex)
    {
        OnEvaluateFrame(ref _keyframeIndex, frameIndex);
    }

    public void OnEvaluateTransition(int frameIndex, float t)
    {
        OnEvaluateTransition(_keyframeIndex, frameIndex, t);
    }
}

public class RuntimeAnimationProperty<T>(AnimationProperty<T> property, Action<T> valueChanged) : RuntimeAnimationProperty
{
    private AnimationProperty<T> Property { get; } = property;
    private Action<T> ValueChanged { get; } = valueChanged;

    public override void OnEvaluateFrame(ref int keyframeIndex, int frameIndex)
    {
        ValueChanged?.Invoke(Property.EvaluateFrame(ref keyframeIndex, frameIndex));
    }

    public override void OnEvaluateTransition(int keyframeIndex, int frameIndex, float t)
    {
        ValueChanged?.Invoke(Property.EvaluateTransition(keyframeIndex, frameIndex, t));
    }
}