using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class RuntimeAnimation
{
    public Animation Animation { get; }

    public int Frame => (int)(Animation.FrameRate * Time);

    public float Time { get; private set; }
    public float NormalizedTime => Time / Animation.Duration;

    private List<RuntimeAnimationProperty> Properties { get; }

    public RuntimeAnimation(Animation animation)
    {
        Animation = animation;
        Properties = [];
    }

    public void OnUpdate(float deltaTime)
    {
        Time = Math.Clamp(Time + deltaTime, 0f, Animation.Duration);
        
        // TODO: Ensure ALL frames are ALWAYS evaluated.
        foreach (RuntimeAnimationProperty property in Properties)
        {
            property.OnEvaluateFrame(Frame);
        }

        if (Frame >= Animation.Frames)
        {
            if (Animation.IsLoopable)
            {
                Reset();
            }
        }
    }

    public void Reset()
    {
        Time = 0f;
    }

    public void BindProperty<T>(AnimationProperty<T> property, Action<T> valueChanged)
    {
        Properties.Add(new RuntimeAnimationProperty<T>(property, valueChanged));
    }

    public IEnumerable<RuntimeAnimationProperty> GetProperties()
    {
        return Properties;
    }
}
