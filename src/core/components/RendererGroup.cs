using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Components;

public class RendererGroup : Component
{
    public override int Order => ComponentOrder.Graphics;

    public sbyte Layer { get; set; }

    private SortedDictionary<int, HashSet<Renderer>> RenderersByLayer { get; }

    public RendererGroup(Entity entity) : base(entity)
    {
        RenderersByLayer = [];
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

    public void AddRenderers(params Renderer[] renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            AddRenderer(renderer);
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

        sbyte order = 0;

        foreach (HashSet<Renderer> renderers in RenderersByLayer.Values)
        {
            foreach (Renderer renderer in renderers)
            {
                Vector2 sortingOrigin = Entity.Node.Position - renderer.Entity.Node.Position;
                renderer.Draw(Layer, sortingOrigin, order);
                order++;
            }
        }
    }
}