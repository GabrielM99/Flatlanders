using System;
using System.Collections.Generic;

namespace Flatlanders.Core.Components;

public class Animator : Component
{
    private class AnimatorLayer
    {
        public RuntimeAnimation RuntimeAnimation { get; private set; }
        public float Time { get; private set; }

        private RuntimeAnimation NextRuntimeAnimation { get; set; }

        private float? TransitionDuration { get; set; }
        private float TransitionTime { get; set; }

        public void OnUpdate(float deltaTime)
        {
            if (RuntimeAnimation == null)
            {
                return;
            }

            int frameIndex = (int)(RuntimeAnimation.Animation.FrameRate * Time);

            if (TransitionDuration != null)
            {
                if (TransitionTime < TransitionDuration)
                {
                    TransitionTime = Math.Clamp(TransitionTime + deltaTime, 0f, TransitionDuration.Value);

                    float t = TransitionTime / TransitionDuration.Value;

                    foreach (RuntimeAnimationProperty property in RuntimeAnimation.GetProperties())
                    {
                        property.OnEvaluateTransition(frameIndex, t);
                    }
                }
                else
                {
                    Reset();
                    RuntimeAnimation = NextRuntimeAnimation;
                    TransitionDuration = null;
                    TransitionTime = 0f;
                }
                
                return;
            }

            foreach (RuntimeAnimationProperty property in RuntimeAnimation.GetProperties())
            {
                property.OnEvaluateFrame(frameIndex);
            }

            if (frameIndex >= RuntimeAnimation.Animation.Frames)
            {
                if (RuntimeAnimation.Animation.IsLoopable)
                {
                    Reset();
                }
                else
                {
                    Clear();
                }
            }
            else
            {
                Time += deltaTime;
            }
        }

        public void PlayAnimation<T>(Animation<T> animation, T obj, float transitionDuration = 0f)
        {
            if (animation == null)
            {
                TransitionDuration = null;
                TransitionTime = 0f;
                Clear();
                return;
            }

            if (RuntimeAnimation == null || (animation != RuntimeAnimation.Animation && (NextRuntimeAnimation == null || NextRuntimeAnimation.Animation != animation)))
            {
                RuntimeAnimation runtimeAnimation = new(animation);
                animation.Bind(runtimeAnimation, obj);

                if (RuntimeAnimation != null && transitionDuration > 0f)
                {
                    TransitionDuration = transitionDuration;
                    TransitionTime = 0f;
                    NextRuntimeAnimation = runtimeAnimation;
                }
                else
                {
                    RuntimeAnimation = runtimeAnimation;
                    Reset();
                }
            }
        }

        private void Reset()
        {
            Time = 0f;
        }

        private void Clear()
        {
            RuntimeAnimation = null;
            Reset();
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