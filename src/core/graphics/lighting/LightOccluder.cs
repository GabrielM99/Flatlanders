using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Penumbra;

namespace Flatlanders.Core.Graphics.Lighting;

public class LightOccluder
{
    private Vector2 _position;
    private Vector2 _scale;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            // Lighting works with window coordinates to maintain the high quality of shadows.
            Data.Position = Engine.RenderManager.WorldToWindowVector(value);
        }
    }
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            // Lighting works with window coordinates to maintain the high quality of shadows.
            Data.Scale = Engine.RenderManager.WorldToWindowVector(value);
        }
    }

    public Hull Data { get; }
    public IList<Vector2> Points => Data.Points;

    private Engine Engine { get; }

    public LightOccluder(Engine engine, Vector2 position, params Vector2[] points)
    {
        Engine = engine;
        Data = new Hull();

        Position = position;
        Scale = Vector2.One;

        Points.AddRange(points);
    }

    public void Destroy()
    {
        Engine.RenderManager.DestroyLightOccluder(this);
    }
}