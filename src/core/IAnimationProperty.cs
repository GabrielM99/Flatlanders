namespace Flatlanders.Core;

public interface IAnimationProperty
{
    int Frames { get; }
    void Evaluate(ref int previousFrameIndex, int currentFrameIndex, ref int nextFrameIndex);
}
