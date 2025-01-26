using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class HorizontalContainer(Entity entity) : Container(entity)
{
    protected override void Build()
    {
        base.Build();

        int childrenCount = Entity.GetChildCount();

        float sizeX = Entity.ChildrenSize.X + Spacing * (childrenCount - 1);
        float sizeY = float.MinValue;
        
        float lastChildWidth = 0f;

        for (int i = 0; i < childrenCount; i++)
        {
            Entity child = Entity.GetChild(i);
            
            float childWidth = child.Size.X;
            float childHeight = child.Size.Y;
            
            if(childHeight > sizeY)
            {
                sizeY = childHeight;
            }
            
            child.LocalPosition = Vector2.UnitX * (-sizeX * 0.5f + childWidth * 0.5f + lastChildWidth * i + Spacing * i);
            lastChildWidth = childWidth;
        }

        Size = new Vector2(sizeX, sizeY);
        Entity.RecalculateSize();
    }
}