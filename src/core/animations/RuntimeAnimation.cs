using System;
using System.Collections.Generic;

namespace Flatlanders.Core.Animations;

public class RuntimeAnimation
{
    public Animation Animation { get; }

    public int Frame { get; private set; }

    public float Time { get; private set; }
    public float NormalizedTime => Time / Animation.Duration;

    private List<RuntimeAnimationProperty> Properties { get; }
    private Dictionary<int, Action> EventByFrame { get; }

    public RuntimeAnimation(Animation animation)
    {
        Animation = animation;
        Properties = [];
        EventByFrame = [];
    }

    public void OnUpdate(float deltaTime)
    {
        Time = Math.Clamp(Time + deltaTime, 0f, Animation.Duration);

        int frame = (int)(Animation.FrameRate * Time);

        // TODO: Ensure ALL frames are ALWAYS evaluated, and only ONCE.
        foreach (RuntimeAnimationProperty property in Properties)
        {
            property.OnEvaluateFrame(frame);
        }

        if (frame != Frame)
        {
            if (EventByFrame.TryGetValue(frame, out Action e))
            {
                e?.Invoke();
            }
        }

        if (frame >= Animation.FrameCount)
        {
            if (Animation.IsLoopable)
            {
                Reset();
            }
        }

        Frame = frame;
    }

    public void Reset()
    {
        Time = 0f;
    }

    public void BindProperty<T>(AnimationProperty<T> property, Action<T> valueChanged)
    {
        Properties.Add(new RuntimeAnimationProperty<T>(property, valueChanged));
    }

    public void BindEvent(int frame, Action e)
    {
        EventByFrame[frame] = e;
    }

    public IEnumerable<RuntimeAnimationProperty> GetProperties()
    {
        return Properties;
    }
}
