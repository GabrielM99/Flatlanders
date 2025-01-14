using Flatlanders.Application.Prefabs;
using Flatlanders.Core;
using Flatlanders.Core.Prefabs;

namespace Flatlanders.Application.Databases;

public class PrefabDatabase : Database<Prefab>
{
    public PlayerPrefab Player { get; private set; }

    protected override void OnLoad(Engine engine)
    {
        Register(0, Player = new PlayerPrefab());
    }
}