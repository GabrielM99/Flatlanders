using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class TileInfo(Tilemap tilemap, Tile tile, Vector3 position)
{
    public Tilemap Tilemap { get; } = tilemap;
    public Tile Tile { get; private set; } = tile;
    public Vector3 Position { get; } = position;
    public Sprite Sprite { get; set; }

    public void OnRemove()
    {
        Tile = null;
    }
}
