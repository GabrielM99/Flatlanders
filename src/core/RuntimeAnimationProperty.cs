using System;

namespace Flatlanders.Core;

public abstract class RuntimeAnimationProperty
{
    private int _lastKeyframeIndex;

    public abstract void OnEvaluate(ref int lastKeyframeIndex, int currentFrameIndex);

    public void Evaluate(int currentFrameIndex)
    {
        OnEvaluate(ref _lastKeyframeIndex, currentFrameIndex);
    }
}

public class RuntimeAnimationProperty<T> : RuntimeAnimationProperty
{
    private AnimationProperty<T> Property { get; }
    private Action<T> ValueChanged { get; }

    public RuntimeAnimationProperty(AnimationProperty<T> property, Action<T> valueChanged)
    {
        Property = property;
        ValueChanged = valueChanged;
    }

    public override void OnEvaluate(ref int lastKeyframeIndex, int currentFrameIndex)
    {
        ValueChanged?.Invoke(Property.Evaluate(ref lastKeyframeIndex, currentFrameIndex));
    }
}