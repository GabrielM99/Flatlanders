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

    private SortedDictionary<int, AnimationKeyframe> KeyframeByIndex { get; }
    private Animation Animation { get; }
    private AnimationInterpolator<T> DefaultInterpolator { get; }

    public AnimationProperty(Animation animation)
    {
        Animation = animation;
        KeyframeByIndex = [];
        DefaultInterpolator = animation.GetDefaultInterpolator<T>();
    }

    public T EvaluateFrame(ref int keyframeIndex, int frameIndex)
    {
        if (KeyframeByIndex.Count == 0)
        {
            return default;
        }

        if (KeyframeByIndex.TryGetValue(frameIndex, out AnimationKeyframe keyframe))
        {
            keyframeIndex = frameIndex;
        }
        else
        {
            keyframe = KeyframeByIndex[keyframeIndex];
        }

        T frameValue = keyframe.Value;

        AnimationInterpolator<T> interpolator = keyframe.Interpolator ?? DefaultInterpolator;

        if (interpolator != null)
        {
            AnimationKeyframe nextKeyframe = keyframe.Next;
            float t = nextKeyframe.Index - keyframeIndex == 0f ? 0f : (float)(frameIndex - keyframeIndex) / Math.Abs(nextKeyframe.Index - keyframeIndex);
            frameValue = interpolator.Invoke(KeyframeByIndex[keyframeIndex].Value, nextKeyframe.Value, t);
        }

        return frameValue;
    }

    public T EvaluateTransition(int keyframeIndex, int frameIndex, float t)
    {
        if (KeyframeByIndex.Count == 0)
        {
            return default;
        }

        AnimationKeyframe endKeyframe = KeyframeByIndex[Animation.Frames];

        if (KeyframeByIndex.TryGetValue(keyframeIndex, out AnimationKeyframe keyframe))
        {
            AnimationInterpolator<T> interpolator = keyframe.Interpolator ?? DefaultInterpolator;

            if (interpolator != null)
            {
                T frameValue = interpolator.Invoke(keyframe.Value, keyframe.Next.Value, (float)(frameIndex - keyframeIndex) / Math.Abs(keyframe.Next.Index - keyframeIndex));
                return interpolator.Invoke(frameValue, endKeyframe.Value, t);
            }
        }

        return endKeyframe.Value;
    }

    public void SetKeyframe(int index, T value, AnimationInterpolator<T> interpolator = null)
    {
        AnimationKeyframe newFrame = new(index, value, interpolator);
        KeyframeByIndex[index] = newFrame;

        foreach (AnimationKeyframe frame in KeyframeByIndex.Values)
        {
            LinkFrame(frame);
        }

        // Ensures a start and end keyframe always exist.
        if (index == 0 && !KeyframeByIndex.ContainsKey(Animation.Frames))
        {
            SetKeyframe(Animation.Frames, value);
        }
        else if (index != 0 && !KeyframeByIndex.ContainsKey(0))
        {
            SetKeyframe(0, value);
        }
    }

    private void LinkFrame(AnimationKeyframe frame)
    {
        int index = frame.Index;

        frame.Previous = KeyframeByIndex[0];
        frame.Next = KeyframeByIndex.GetValueOrDefault(Animation.Frames);

        foreach (AnimationKeyframe otherFrame in KeyframeByIndex.Values)
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
