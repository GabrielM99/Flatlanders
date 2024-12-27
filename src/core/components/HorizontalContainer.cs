using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class HorizontalContainer : Container, ISizable
{
    public Vector2 Size { get; private set; }
    
    public HorizontalContainer(Entity entity) : base(entity)
    {
    }

    protected override void Build()
    {
        base.Build();

        Transform transform = Entity.Transform;

        int childrenCount = transform.GetChildCount();

        float sizeX = transform.Volume.X + Spacing * (childrenCount - 1);
        float lastChildWidth = 0f;

        for (int i = 0; i < childrenCount; i++)
        {
            // TODO: Update child anchors?
            Transform child = transform.GetChild(i);
            float childWidth = child.Size.X;
            child.LocalPosition = Vector2.UnitX * (-sizeX * 0.5f + childWidth * 0.5f + lastChildWidth * i + Spacing * i);
            lastChildWidth = childWidth;
        }

        Size = new Vector2(sizeX, transform.Size.Y);
        Entity.Transform.RecalculateSize();
    }
}