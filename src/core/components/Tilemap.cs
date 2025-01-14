using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class Tilemap : Component
{
    public event Action<Tile, Vector3> TileSetted;

    private Dictionary<Vector3, Tile> TileByPosition { get; }

    public Tilemap(Entity entity) : base(entity)
    {
        TileByPosition = new Dictionary<Vector3, Tile>();
    }

    public Tile GetTile(Vector3 position)
    {
        return TileByPosition.GetValueOrDefault(position);
    }

    public void SetTile(Tile tile, Vector3 position)
    {
        position = Vector3.Ceiling(position);

        if (tile == null)
        {
            if (TileByPosition.Remove(position))
            {
                TileSetted?.Invoke(tile, position);
            }
        }
        else
        {
            if (!TileByPosition.TryGetValue(position, out Tile oldTile) || tile != oldTile)
            {
                TileByPosition[position] = tile;
                TileSetted?.Invoke(tile, position);
            }
        }

    }

    public IEnumerable<KeyValuePair<Vector3, Tile>> GetTiles(Vector3 min, Vector3 max)
    {
        for (int x = (int)min.X; x <= max.X; x++)
        {
            for (int y = (int)min.Y; y <= max.Y; y++)
            {
                for (int z = (int)min.Z; z <= max.Z; z++)
                {
                    Vector3 position = new(x, y, z);
                    yield return new KeyValuePair<Vector3, Tile>(position, GetTile(position));
                }
            }
        }
    }

    public IEnumerable<KeyValuePair<Vector3, Tile>> GetTiles()
    {
        return TileByPosition;
    }
}