using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public class AnimationProperty<T>
{
    private class AnimationKeyframe(int index, T value, AnimationInterpolator<T> interpolator = null)
    {
        public int Index { get; } = index;
        public T Value { get; } = value;
        
        public AnimationInterpolator<T> Interpolator { get; } = interpolator;

        public AnimationKeyframe Next { get; set; }
        public AnimationKeyframe Previous { get; set; }
    }

    private SortedDictionary<int, AnimationKeyframe> KeyByFrame { get; }
    private Animation Animation { get; }
    private AnimationInterpolator<T> DefaultInterpolator { get; }

    public AnimationProperty(Animation animation)
    {
        Animation = animation;
        KeyByFrame = [];
        DefaultInterpolator = animation.GetDefaultInterpolator<T>();
    }

    public T EvaluateFrame(ref int key, int frame)
    {
        if (KeyByFrame.Count == 0)
        {
            return default;
        }

        if (KeyByFrame.TryGetValue(frame, out AnimationKeyframe keyframe))
        {
            key = frame;
        }
        else
        {
            keyframe = KeyByFrame[key];
        }

        T frameValue = keyframe.Value;

        AnimationInterpolator<T> interpolator = keyframe.Interpolator ?? DefaultInterpolator;

        if (interpolator != null)
        {
            AnimationKeyframe nextKeyframe = keyframe.Next;
            float t = nextKeyframe.Index - key == 0f ? 0f : (float)(frame - key) / Math.Abs(nextKeyframe.Index - key);
            frameValue = interpolator.Invoke(KeyByFrame[key].Value, nextKeyframe.Value, t);
        }

        return frameValue;
    }

    public T EvaluateBlend(int key, int frame, float t)
    {
        if (KeyByFrame.Count == 0)
        {
            return default;
        }

        AnimationKeyframe endKeyframe = KeyByFrame[Animation.FrameCount];

        if (KeyByFrame.TryGetValue(key, out AnimationKeyframe keyframe))
        {
            AnimationInterpolator<T> interpolator = keyframe.Interpolator ?? DefaultInterpolator;

            if (interpolator != null)
            {
                T frameValue = interpolator.Invoke(keyframe.Value, keyframe.Next.Value, (float)(frame - key) / Math.Abs(keyframe.Next.Index - key));
                return interpolator.Invoke(frameValue, endKeyframe.Value, t);
            }
        }

        return endKeyframe.Value;
    }

    public void SetKeyframe(int index, T value, AnimationInterpolator<T> interpolator = null)
    {
        AnimationKeyframe newFrame = new(index, value, interpolator);
        KeyByFrame[index] = newFrame;

        foreach (AnimationKeyframe frame in KeyByFrame.Values)
        {
            LinkFrame(frame);
        }

        // Ensures a start and end keyframe always exist.
        if (index == 0 && !KeyByFrame.ContainsKey(Animation.FrameCount))
        {
            SetKeyframe(Animation.FrameCount, value);
        }
        else if (index != 0 && !KeyByFrame.ContainsKey(0))
        {
            SetKeyframe(0, value);
        }
    }

    private void LinkFrame(AnimationKeyframe frame)
    {
        int index = frame.Index;

        frame.Previous = KeyByFrame[0];
        frame.Next = KeyByFrame.GetValueOrDefault(Animation.FrameCount);

        foreach (AnimationKeyframe otherFrame in KeyByFrame.Values)
        {
            if (otherFrame.Index < index)
            {
                frame.Previous = otherFrame;
            }
            else if (otherFrame.Index > index)
            {
                frame.Next = otherFrame;
                break;
            }
        }
    }
}
