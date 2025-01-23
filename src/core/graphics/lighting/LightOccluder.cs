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
            Data.Position = value * Engine.RenderManager.ReferencePixelsPerUnit;
        }
    }
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            Data.Scale = value * Engine.RenderManager.ReferencePixelsPerUnit;
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