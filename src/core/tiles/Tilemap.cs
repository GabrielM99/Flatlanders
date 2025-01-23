using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Tiles;

public class Tilemap
{
    public event Action<TileInfo> TileSetted;

    private Dictionary<Vector3, TileInfo> TileInfoByPosition { get; }

    public Tilemap()
    {
        TileInfoByPosition = [];
    }

    public Tile GetTile(Vector3 position)
    {
        return TileInfoByPosition.GetValueOrDefault(position)?.Tile;
    }

    public void SetTile(Tile tile, Vector3 position)
    {
        SetTile(tile, position, out _);
    }

    public void SetTile(Tile tile, Vector3 position, out TileInfo tileInfo)
    {
        position = Vector3.Ceiling(position);

        if (tile == null)
        {
            if (TileInfoByPosition.Remove(position, out tileInfo))
            {
                tileInfo.OnRemove();
                TileSetted?.Invoke(tileInfo);
                Refresh(tileInfo);
            }
        }
        else
        {
            if (!TileInfoByPosition.TryGetValue(position, out tileInfo) || tile != tileInfo.Tile)
            {
                tileInfo = new(this, tile, position);
                SetTileInfo(tileInfo);
                TileSetted?.Invoke(tileInfo);
                Refresh(tileInfo);
            }
        }
    }

    public TileInfo GetTileInfo(Vector3 position)
    {
        return TileInfoByPosition.GetValueOrDefault(position);
    }

    public void SetTileInfo(TileInfo tileInfo)
    {
        TileInfoByPosition[tileInfo.Position] = tileInfo;
    }

    public void Refresh(TileInfo tileInfo)
    {
        tileInfo.Tile?.OnRefresh(tileInfo);
    }

    public IEnumerable<TileInfo> GetTileInfos(Vector3 min, Vector3 max)
    {
        for (int x = (int)min.X; x <= max.X; x++)
        {
            for (int y = (int)min.Y; y <= max.Y; y++)
            {
                for (int z = (int)min.Z; z <= max.Z; z++)
                {
                    Vector3 position = new(x, y, z);
                    yield return GetTileInfo(position);
                }
            }
        }
    }
}