using System;
using System.Collections.Generic;
using Flatlanders.Core.Components;

namespace Flatlanders.Core;

public class AnimationProperty<T> : IAnimationProperty
{
    private class AnimationFrame
    {
        public int Index { get; }
        public T Value { get; }

        public AnimationFrame Next { get; set; }
        public AnimationFrame Previous { get; set; }

        public AnimationFrame(int index, T value)
        {
            Index = index;
            Value = value;
        }
    }

    public int Frames { get; private set; }
    public Action<T> ValueChanged { get; }

    private SortedDictionary<int, AnimationFrame> FrameByIndex { get; }
    private IAnimationInterpolator<T> Interpolator { get; }

    public AnimationProperty(Animation animation, Action<T> valueChanged)
    {
        ValueChanged = valueChanged;
        FrameByIndex = new SortedDictionary<int, AnimationFrame>();
        Interpolator = animation.GetInterpolator<T>();
    }

    public void Evaluate(ref int previousFrameIndex, int currentFrameIndex, ref int nextFrameIndex)
    {
        if (FrameByIndex.TryGetValue(currentFrameIndex, out AnimationFrame currentFrame))
        {
            previousFrameIndex = currentFrameIndex;
            nextFrameIndex = currentFrame.Next == null ? 0 : currentFrame.Next.Index;
        }

        T previousFrameValue = FrameByIndex[previousFrameIndex].Value;
        T currentFrameValue = previousFrameValue;

        if (Interpolator != null)
        {
            T nextFrameValue = FrameByIndex[nextFrameIndex].Value;
            float t = (float)(currentFrameIndex - previousFrameIndex) / (nextFrameIndex - previousFrameIndex);
            currentFrameValue = Interpolator.Interpolate(previousFrameValue, nextFrameValue, t);
        }

        ValueChanged?.Invoke(currentFrameValue);
    }

    public void AddFrame(int index, T value)
    {
        AnimationFrame newFrame = new(index, value);
        FrameByIndex[index] = newFrame;

        foreach (AnimationFrame frame in FrameByIndex.Values)
        {
            LinkFrame(frame);
        }

        Frames = Math.Max(index, Frames);
    }

    private void LinkFrame(AnimationFrame frame)
    {
        int index = frame.Index;

        foreach (AnimationFrame otherFrame in FrameByIndex.Values)
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
