using System.Collections.Generic;

namespace Flatlanders.Core.Components;

public class Animator : Component
{
    private class AnimatorLayer
    {
        public RuntimeAnimation RuntimeAnimation { get; private set; }
        public float Time { get; private set; }

        public void OnUpdate(float deltaTime)
        {
            if (RuntimeAnimation == null)
            {
                return;
            }

            int currentFrameIndex = (int)(RuntimeAnimation.Animation.FrameRate * Time);

            foreach (RuntimeAnimationProperty property in RuntimeAnimation.GetProperties())
            {
                property.Evaluate(currentFrameIndex);
            }

            if (currentFrameIndex >= RuntimeAnimation.Animation.Frames)
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
                RuntimeAnimation = null;
                return;
            }

            if (RuntimeAnimation == null || animation != RuntimeAnimation.Animation)
            {
                RuntimeAnimation runtimeAnimation = new(animation);
                animation.Bind(runtimeAnimation, obj);
                RuntimeAnimation = runtimeAnimation;
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