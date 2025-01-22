using System;

namespace Flatlanders.Core.Animations;

public abstract class RuntimeAnimationProperty
{
    private int _key;

    public abstract void OnEvaluateFrame(ref int key, int frame);
    public abstract void OnEvaluateBlend(int key, int frame, float t);

    public void OnEvaluateFrame(int frame)
    {
        OnEvaluateFrame(ref _key, frame);
    }

    public void OnEvaluateBlend(int frame, float t)
    {
        OnEvaluateBlend(_key, frame, t);
    }
}

public class RuntimeAnimationProperty<T>(AnimationProperty<T> property, Action<T> valueChanged) : RuntimeAnimationProperty
{
    private AnimationProperty<T> Property { get; } = property;
    private Action<T> ValueChanged { get; } = valueChanged;

    public override void OnEvaluateFrame(ref int key, int frame)
    {
        ValueChanged?.Invoke(Property.EvaluateFrame(ref key, frame));
    }

    public override void OnEvaluateBlend(int key, int frame, float t)
    {
        ValueChanged?.Invoke(Property.EvaluateBlend(key, frame, t));
    }
}