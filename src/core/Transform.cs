using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public struct Transform : ITransform
{
    public ITransform Root { get; set; }
    
    public TransformSpace Space { get; set; }
    
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 Size { get; set; }
        
    public TransformAnchor Anchor { get; set; }
    
    public Vector2 Pivot { get; set; }
    
    public Transform()
    {
        Space = TransformSpace.World;
        Scale = Vector2.One;
        Anchor = TransformAnchor.Center;
    }
}
