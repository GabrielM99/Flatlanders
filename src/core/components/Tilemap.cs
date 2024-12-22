using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class Tilemap : Component
{
    /*private class TileChunk
    {
        private Tile[,,] Tiles { get; }
        
        public TileChunk(Vector3 size)
        {
            Tiles = new Tile[(int)size.X, (int)size.Y, (int)size.Z];
        }
        
        public void SetTile(Tile tile, Vector3 position)
        {
            Tiles[(int)position.X, (int)position.Y, (int)position.Z] = tile;
        }
        
        public IEnumerable<>
    }*/

    private Dictionary<Vector3, Tile> TileByPosition { get; }

    public Tilemap(Entity entity) : base(entity)
    {
        TileByPosition = new Dictionary<Vector3, Tile>();
    }

    public void SetTile(Tile tile, Vector3 position)
    {
        position = Vector3.Ceiling(position);

        if (tile == null)
        {
            TileByPosition.Remove(position);
        }
        else
        {
            TileByPosition[position] = tile;
        }
    }

    public IEnumerable<KeyValuePair<Vector3, Tile>> GetTileByPosition()
    {
        return TileByPosition;
    }
}