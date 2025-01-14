using Flatlanders.Application.Components;
using Flatlanders.Application.Databases;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Flatlanders.Core.Prefabs;

namespace Flatlanders.Application.Prefabs;

public class PlayerPrefab : Prefab
{
    public override void OnCreate(Entity entity)
    {
        entity.AddComponent<Player>();
    }
}