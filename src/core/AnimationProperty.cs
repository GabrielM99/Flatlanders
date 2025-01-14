using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public class AnimationProperty<T> : IAnimationProperty
{
    private class AnimationKeyframe
    {
        public int Index { get; }
        public T Value { get; }
        public AnimationInterpolator<T> Interpolator { get; }

        public AnimationKeyframe Next { get; set; }
        public AnimationKeyframe Previous { get; set; }

        public AnimationKeyframe(int index, T value, AnimationInterpolator<T> interpolator = null)
        {
            Index = index;
            Value = value;
            Interpolator = interpolator;
        }
    }

    public int Frames { get; private set; }

    private SortedDictionary<int, AnimationKeyframe> FrameByIndex { get; }
    private Animation Animation { get; }
    private AnimationInterpolator<T> DefaultInterpolator { get; }

    public AnimationProperty(Animation animation)
    {
        Animation = animation;
        FrameByIndex = new SortedDictionary<int, AnimationKeyframe>();
        DefaultInterpolator = animation.GetDefaultInterpolator<T>();

        SetKeyframe(0, default);
        SetKeyframe(animation.Frames, default);
    }

    public T Evaluate(ref int lastKeyframeIndex, int currentFrameIndex)
    {
        if (FrameByIndex.TryGetValue(currentFrameIndex, out AnimationKeyframe currentKeyframe))
        {
            lastKeyframeIndex = currentFrameIndex;
        }
        else
        {
            currentKeyframe = FrameByIndex[lastKeyframeIndex];
        }

        T lastKeyframeValue = FrameByIndex[lastKeyframeIndex].Value;
        T currentFrameValue = currentKeyframe.Value;

        AnimationInterpolator<T> interpolator = currentKeyframe.Interpolator ?? DefaultInterpolator;

        if (interpolator != null)
        {
            AnimationKeyframe nextKeyframe = currentKeyframe.Next;
            float t = nextKeyframe.Index - lastKeyframeIndex == 0f ? 0f : (float)(currentFrameIndex - lastKeyframeIndex) / Math.Abs(nextKeyframe.Index - lastKeyframeIndex);
            currentFrameValue = interpolator.Invoke(lastKeyframeValue, nextKeyframe.Value, t);
        }

        return currentFrameValue;
    }

    public void SetKeyframe(int index, T value, AnimationInterpolator<T> interpolator = null)
    {
        AnimationKeyframe newFrame = new(index, value, interpolator);
        FrameByIndex[index] = newFrame;

        foreach (AnimationKeyframe frame in FrameByIndex.Values)
        {
            LinkFrame(frame);
        }

        Frames = Math.Max(index, Frames);
    }

    private void LinkFrame(AnimationKeyframe frame)
    {
        int index = frame.Index;

        frame.Previous = FrameByIndex[0];
        frame.Next = FrameByIndex.GetValueOrDefault(Animation.Frames);

        foreach (AnimationKeyframe otherFrame in FrameByIndex.Values)
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
