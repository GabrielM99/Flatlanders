using System.Collections.Generic;

namespace Flatlanders.Core.Components;

public class Animator : Component
{
    private class AnimatorLayer
    {
        private RuntimeAnimation RuntimeAnimation { get; set; }
        private AnimationBlend AnimationBlend { get; set; }

        public void OnUpdate(float deltaTime)
        {
            if (RuntimeAnimation == null)
            {
                return;
            }

            if (AnimationBlend != null)
            {
                AnimationBlend.OnUpdate(deltaTime);

                if (AnimationBlend.NormalizedTime >= 1f)
                {
                    RuntimeAnimation = AnimationBlend.EndRuntimeAnimation;
                    AnimationBlend = null;
                }

                return;
            }

            RuntimeAnimation.OnUpdate(deltaTime);

            if (RuntimeAnimation.NormalizedTime >= 1f && !RuntimeAnimation.Animation.IsLoopable)
            {
                Clear();
            }
        }

        public void PlayAnimation<T>(Animation<T> animation, T obj, float blendTime = 0f)
        {
            if (animation == null)
            {
                Clear();
                return;
            }

            if (RuntimeAnimation == null || (animation != RuntimeAnimation.Animation && (AnimationBlend == null || AnimationBlend.EndRuntimeAnimation.Animation != animation)))
            {
                RuntimeAnimation newRuntimeAnimation = new(animation);
                animation.Bind(newRuntimeAnimation, obj);

                if (RuntimeAnimation != null && blendTime > 0f)
                {
                    AnimationBlend = new AnimationBlend(RuntimeAnimation, newRuntimeAnimation, blendTime);
                }
                else
                {
                    RuntimeAnimation = newRuntimeAnimation;
                }
            }
        }

        private void Clear()
        {
            RuntimeAnimation = null;
            AnimationBlend = null;
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

    public void PlayAnimation<T>(Animation<T> animation, T obj, float transitionDuration = 0f, int layerIndex = 0)
    {
        if (!LayerByIndex.TryGetValue(layerIndex, out AnimatorLayer layer))
        {
            layer = new AnimatorLayer();
            LayerByIndex[layerIndex] = layer;
        }

        layer.PlayAnimation(animation, obj, transitionDuration);
    }
}