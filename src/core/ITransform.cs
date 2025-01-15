using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public interface ITransform
{
    private static Vector2[] AnchorPositions { get; } = new Vector2[]
    {
            new(-1f, -1f),  new(0f, -1f),   new(1f, -1f),
            new(-1f, 0f),   new(0f, 0f),    new(1f, 0f),
            new(-1f, 1f),   new(0f, 1f),    new(1f, 1f)
    };

    ITransform Root { get; set; }

    TransformSpace Space { get; set; }

    Vector2 Position { get; set; }
    float Rotation { get; set; }
    Vector2 Scale { get; set; }
    Vector2 Size { get; set; }

    RectangleF Bounds => new(Position, Size);

    TransformAnchor Anchor { get; set; }
    Vector2 AnchorPosition => GetAnchorPosition(Anchor);

    Vector2 Pivot { get; set; }

    private static Vector2 GetAnchorPosition(TransformAnchor anchor)
    {
        return AnchorPositions[(int)anchor];
    }
}
