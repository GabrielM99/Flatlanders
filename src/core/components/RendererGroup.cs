using System;
using System.Collections.Generic;

namespace Flatlanders.Core.Components;

public class RendererGroup : Component
{
    public short Layer { get; set; }

    private SortedDictionary<int, HashSet<Renderer>> RenderersByLayer { get; }

    public RendererGroup(Entity entity) : base(entity)
    {
        RenderersByLayer = new SortedDictionary<int, HashSet<Renderer>>();
    }

    public void AddRenderer(Renderer renderer)
    {
        if (renderer.Group == this)
        {
            return;
        }

        int layer = renderer.Layer;

        if (!RenderersByLayer.TryGetValue(layer, out HashSet<Renderer> renderers))
        {
            renderers = new HashSet<Renderer>();
            RenderersByLayer[layer] = renderers;
        }

        if (renderers.Add(renderer))
        {
            renderer.Group = this;
        }
    }

    public void RemoveRenderer(Renderer renderer)
    {
        if (renderer.Group != this)
        {
            return;
        }

        int layer = renderer.Layer;

        if (RenderersByLayer.TryGetValue(layer, out HashSet<Renderer> renderers))
        {
            if (renderers.Remove(renderer))
            {
                renderer.Group = null;

                if (renderers.Count == 0)
                {
                    RenderersByLayer.Remove(layer);
                }
            }
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        foreach (HashSet<Renderer> renderers in RenderersByLayer.Values)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.Draw(Layer);
            }
        }
    }
}