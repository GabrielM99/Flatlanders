using Flatlanders.Core.Components;

namespace Flatlanders.Core.Prefabs;

public class TextPrefab : Prefab
{
    public override void OnCreate(Entity entity)
    {
        entity.AddComponent<TextRenderer>();
    }
}