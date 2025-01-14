using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public abstract class Animation
{
    public abstract int Frames { get; }

    public virtual int FrameRate { get; } = 30;
    public virtual bool IsLoopable { get; } = true;

    public Engine Engine { get; }

    private Dictionary<Type, object> InterpolatorByType { get; }

    public Animation(Engine engine)
    {
        Engine = engine;
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
        return (AnimationInterpolator<T>)InterpolatorByType.GetValueOrDefault(typeof(T));
    }
}

public abstract class Animation<T> : Animation
{
    protected Animation(Engine engine) : base(engine)
    {
    }

    public abstract void Bind(RuntimeAnimation runtimeAnimation, T obj);
}