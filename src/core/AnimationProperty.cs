using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public class AnimationProperty<T> : IAnimationProperty
{
    private class AnimationKeyframe
    {
        public int Index { get; }
        public T Value { get; }

        public AnimationKeyframe Next { get; set; }
        public AnimationKeyframe Previous { get; set; }

        public AnimationKeyframe(int index, T value)
        {
            Index = index;
            Value = value;
        }
    }

    public int Frames { get; private set; }

    private SortedDictionary<int, AnimationKeyframe> FrameByIndex { get; }
    private IAnimationInterpolator<T> Interpolator { get; }

    public AnimationProperty(Animation animation)
    {
        FrameByIndex = new SortedDictionary<int, AnimationKeyframe>();
        Interpolator = animation.GetInterpolator<T>();
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

        if (Interpolator != null)
        {
            AnimationKeyframe nextKeyframe = currentKeyframe.Next;
            float t = (float)(currentFrameIndex - lastKeyframeIndex) / Math.Abs(nextKeyframe.Index - lastKeyframeIndex);
            currentFrameValue = Interpolator.Interpolate(lastKeyframeValue, nextKeyframe.Value, t);
        }

        return currentFrameValue;
    }

    public void SetKeyframe(int index, T value)
    {
        AnimationKeyframe newFrame = new(index, value);
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

        AnimationKeyframe firstFrame = FrameByIndex[0];

        frame.Previous = firstFrame;
        frame.Next = firstFrame;

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
