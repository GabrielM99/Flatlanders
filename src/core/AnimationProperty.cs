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

    private SortedDictionary<int, AnimationKeyframe> KeyframeByIndex { get; }
    private Animation Animation { get; }
    private AnimationInterpolator<T> DefaultInterpolator { get; }

    public AnimationProperty(Animation animation)
    {
        Animation = animation;
        KeyframeByIndex = new SortedDictionary<int, AnimationKeyframe>();
        DefaultInterpolator = animation.GetDefaultInterpolator<T>();
    }

    public T Evaluate(ref int lastKeyframeIndex, int currentFrameIndex)
    {
        if(KeyframeByIndex.Count == 0)
        {
            return default;
        }
        
        if (KeyframeByIndex.TryGetValue(currentFrameIndex, out AnimationKeyframe currentKeyframe))
        {
            lastKeyframeIndex = currentFrameIndex;
        }
        else
        {
            currentKeyframe = KeyframeByIndex[lastKeyframeIndex];
        }

        T lastKeyframeValue = KeyframeByIndex[lastKeyframeIndex].Value;
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
        KeyframeByIndex[index] = newFrame;

        foreach (AnimationKeyframe frame in KeyframeByIndex.Values)
        {
            LinkFrame(frame);
        }

        Frames = Math.Max(index, Frames);

        if (index == 0 && !KeyframeByIndex.ContainsKey(Animation.Frames))
        {
            SetKeyframe(Animation.Frames, value);
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
