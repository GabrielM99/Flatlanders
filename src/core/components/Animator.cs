using System.Collections.Generic;

namespace Flatlanders.Core.Components;

public class Animator : Component
{
    private class AnimatorLayer
    {
        private class RuntimeAnimationProperty
        {
            public IAnimationProperty Property { get; }
            public int PreviousFrameIndex { get; set; }
            public int NextFrameIndex { get; set; }

            public RuntimeAnimationProperty(IAnimationProperty animationProperty)
            {
                Property = animationProperty;
            }
        }

        public Animation PlayingAnimation { get; private set; }
        public float Time { get; private set; }

        private List<RuntimeAnimationProperty> RuntimeAnimationProperties { get; }

        public AnimatorLayer()
        {
            RuntimeAnimationProperties = new List<RuntimeAnimationProperty>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (PlayingAnimation == null)
            {
                return;
            }

            int frameIndex = (int)(PlayingAnimation.FrameRate * Time);

            foreach (RuntimeAnimationProperty runtimeAnimationProperty in RuntimeAnimationProperties)
            {
                int previousFrameIndex = runtimeAnimationProperty.PreviousFrameIndex;
                int nextFrameIndex = runtimeAnimationProperty.NextFrameIndex;

                runtimeAnimationProperty.Property.Evaluate(ref previousFrameIndex, frameIndex, ref nextFrameIndex);

                runtimeAnimationProperty.PreviousFrameIndex = previousFrameIndex;
                runtimeAnimationProperty.NextFrameIndex = nextFrameIndex;
            }

            if (frameIndex >= PlayingAnimation.Frames)
            {
                Time = 0f;
            }
            else
            {
                Time += deltaTime;
            }
        }

        public void PlayAnimation<T>(Animation<T> animation, T obj)
        {
            if (animation == null)
            {
                RuntimeAnimationProperties.Clear();
                return;
            }

            if (animation != PlayingAnimation)
            {
                animation.Bind(obj);

                foreach (IAnimationProperty animationProperty in animation.GetProperties())
                {
                    RuntimeAnimationProperties.Add(new RuntimeAnimationProperty(animationProperty));
                }

                PlayingAnimation = animation;
            }
        }
    }

    private SortedDictionary<int, AnimatorLayer> LayerByIndex { get; set; }

    public Animator(Entity entity) : base(entity)
    {
        LayerByIndex = new SortedDictionary<int, AnimatorLayer>();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        foreach (AnimatorLayer layer in LayerByIndex.Values)
        {
            layer.OnUpdate(deltaTime);
        }
    }

    public void PlayAnimation<T>(Animation<T> animation, T obj, int layerIndex = 0)
    {
        if (!LayerByIndex.TryGetValue(layerIndex, out AnimatorLayer layer))
        {
            layer = new AnimatorLayer();
            LayerByIndex[layerIndex] = layer;
        }

        layer.PlayAnimation(animation, obj);
    }
}