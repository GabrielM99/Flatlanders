using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public abstract class Animation
{
    public abstract int Frames { get; }

    public virtual int FrameRate { get; } = 30;
    public virtual bool IsLoopable { get; } = true;

    private Dictionary<Type, object> InterpolatorByType { get; }

    public Animation()
    {
        InterpolatorByType = new Dictionary<Type, object>();
        AddInterpolator(new Vector2Interpolator());
    }

    public void AddInterpolator<T>(IAnimationInterpolator<T> interpolator)
    {
        InterpolatorByType[typeof(T)] = interpolator;
    }

    public IAnimationInterpolator<T> GetInterpolator<T>()
    {
        return (IAnimationInterpolator<T>)InterpolatorByType[typeof(T)];
    }
}

public abstract class Animation<T> : Animation
{
    public abstract void Bind(RuntimeAnimation runtimeAnimation, T obj);
}