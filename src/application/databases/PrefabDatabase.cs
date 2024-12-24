using Flatlanders.Core.Prefabs;

namespace Flatlanders.Application.Databases;

public class PrefabDatabase : Database<Prefab>
{
    public TextPrefab Text { get; private set; }
    
    public override void Load()
    {
        Register(0, Text = new TextPrefab());
    }
}