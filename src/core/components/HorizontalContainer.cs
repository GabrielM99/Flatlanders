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

        Node node = Entity.Node;

        int childrenCount = node.GetChildCount();

        float sizeX = node.ChildrenSize.X + Spacing * (childrenCount - 1);
        float lastChildWidth = 0f;

        for (int i = 0; i < childrenCount; i++)
        {
            Node child = node.GetChild(i);
            float childWidth = child.Bounds.Width;
            child.LocalPosition = Vector2.UnitX * (-sizeX * 0.5f + childWidth * 0.5f + lastChildWidth * i + Spacing * i);
            lastChildWidth = childWidth;
        }

        Size = new Vector2(sizeX, node.Size.Y);
        Entity.Node.RecalculateSize();
    }
}