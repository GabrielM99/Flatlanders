using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Animations;

public abstract class Animation
{
    public abstract int FrameCount { get; }

    public virtual int FrameRate { get; } = 30;
    public virtual bool IsLoopable { get; } = true;

    public float Duration => (float)FrameCount / FrameRate;

    public Engine Engine { get; }

    private Dictionary<Type, object> InterpolatorByType { get; }

    public Animation(Engine engine)
    {
        Engine = engine;
        InterpolatorByType = [];

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
        return (AnimationInterpolator<T>)InterpolatorByType.GetValueOrDefault(typeof(T));
    }
}

public abstract class Animation<T>(Engine engine) : Animation(engine)
{
    public abstract void Bind(RuntimeAnimation runtimeAnimation, T obj);
}