using System;
using System.Collections.Generic;

namespace Flatlanders.Core.Animations;

public class RuntimeAnimation
{
    public Animation Animation { get; }

    public int Frame { get; private set; }

    public float Time { get; private set; }
    public float NormalizedTime => Time / Animation.Duration;

    public float Speed { get; set; }

    private List<RuntimeAnimationProperty> Properties { get; }
    private Dictionary<int, Action> EventByFrame { get; }

    public RuntimeAnimation(Animation animation, float speed = 1f)
    {
        Animation = animation;
        Speed = speed;
        Properties = [];
        EventByFrame = [];
    }

    public void OnUpdate(float deltaTime)
    {
        Time = Math.Clamp(Time + deltaTime * Speed, 0f, Animation.Duration);

        int newFrame = (int)(Animation.FrameRate * Time);
        int deltaFrame = newFrame >= Frame ? newFrame - Frame : Animation.FrameCount - Frame + newFrame;

        // Ensure all frames are always evaluated.
        while (deltaFrame > 0)
        {
            int stepFrame = (int)GameMath.Repeat(Frame + deltaFrame, Animation.FrameCount);

            foreach (RuntimeAnimationProperty property in Properties)
            {
                property.OnEvaluateFrame(stepFrame);
            }

            // Invoke frame events only once.
            if (stepFrame != Frame)
            {
                if (EventByFrame.TryGetValue(stepFrame, out Action e))
                {
                    e?.Invoke();
                }
            }

            deltaFrame--;
        }

        if (newFrame >= Animation.FrameCount)
        {
            if (Animation.IsLoopable)
            {
                Reset();
            }
        }

        Frame = newFrame;
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
