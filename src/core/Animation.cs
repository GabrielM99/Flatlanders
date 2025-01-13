using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public abstract class Animation
{
    public virtual int FrameRate { get; } = 30;

    public int Frames { get; private set; }

    private HashSet<IAnimationProperty> Properties { get; }
    private Dictionary<Type, object> InterpolatorByType { get; }

    public Animation()
    {
        Properties = new HashSet<IAnimationProperty>();
        InterpolatorByType = new Dictionary<Type, object>();
        AddInterpolator(new Vector2Interpolator());
    }

    public void AddProperty(IAnimationProperty property)
    {
        Frames = Math.Max(property.Frames, Frames);
        Properties.Add(property);
    }

    public IEnumerable<IAnimationProperty> GetProperties()
    {
        return Properties;
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
    public abstract void Bind(T obj);
}