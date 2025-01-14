using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        
        AddDefaultInterpolator<int>(AnimationInterpolators.Linear);
        AddDefaultInterpolator<float>(AnimationInterpolators.Linear);
        AddDefaultInterpolator<double>(AnimationInterpolators.Linear);
        AddDefaultInterpolator<Vector2>(AnimationInterpolators.Linear);
    }

    public void AddDefaultInterpolator<T>(AnimationInterpolator<T> interpolator)
    {
        InterpolatorByType[typeof(T)] = interpolator;
    }

    public AnimationInterpolator<T> GetDefaultInterpolator<T>()
    {
        return (AnimationInterpolator<T>)InterpolatorByType[typeof(T)];
    }
}

public abstract class Animation<T> : Animation
{
    public abstract void Bind(RuntimeAnimation runtimeAnimation, T obj);
}