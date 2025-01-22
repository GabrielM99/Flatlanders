using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Flatlanders.Core.Components;

public class TilemapCollider : Component
{
    private class TileCollider : ICollisionActor
    {
        public IShapeF Bounds { get; private set; }
        public string LayerName => Physics.DEFAULT_LAYER_NAME;

        public TileCollider(Vector3 position)
        {
            Bounds = new RectangleF(new Vector2(position.X, position.Y) - Vector2.One * 0.5f, Vector2.One);
        }

        public void OnCollision(CollisionEventArgs collisionInfo) { }
    }

    private static Vector3[] TileNeighborOffsets { get; } =
    [
        Vector3.UnitX, -Vector3.UnitX, -Vector3.UnitY, Vector3.UnitY
    ];

    private Tilemap _tilemap;

    public Tilemap Tilemap
    {
        get => _tilemap;

        set
        {
            if (value != _tilemap)
            {
                if (_tilemap != null)
                {
                    _tilemap.TileSetted -= OnTilemapTileSetted;
                }

                _tilemap = value;

                if (value != null)
                {
                    value.TileSetted += OnTilemapTileSetted;
                }
            }
        }
    }

    private Dictionary<Vector3, TileCollider> TileColliderByPosition { get; }

    public TilemapCollider(Entity entity) : base(entity)
    {
        TileColliderByPosition = [];
    }

    public void AddTileCollider(Vector3 position)
    {
        if (!TileColliderByPosition.ContainsKey(position))
        {
            TileCollider collider = new(position);
            TileColliderByPosition[position] = collider;
            Engine.Physics.AddCollider(collider);

            Penumbra.Hull hull = new Penumbra.Hull(new Vector2(1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f), new Vector2(1.0f, -1.0f))
            {
                Position = new Vector2(position.X, position.Y),
                Scale = new Vector2(0.5f)
            };

            Engine.Graphics.AddHull(hull);
        }
    }

    public void RemoveTileCollider(Vector3 position)
    {
        if (TileColliderByPosition.Remove(position, out TileCollider collider))
        {
            Engine.Physics.RemoveCollider(collider);
        }
    }

    private void UpdateTileCollider(Vector3 position)
    {
        Tile tile = Tilemap.GetTile(position);

        if (tile != null && tile.IsCollidable)
        {
            foreach (Vector3 neighborOffset in TileNeighborOffsets)
            {
                Vector3 neighborPosition = position + neighborOffset;
                Tile neighborTile = Tilemap.GetTile(neighborPosition);

                if (neighborTile == null || !neighborTile.IsCollidable)
                {
                    AddTileCollider(position);
                    return;
                }
            }
        }

        RemoveTileCollider(position);
    }

    private void OnTilemapTileSetted(TileInfo tileInfo)
    {
        UpdateTileCollider(tileInfo.Position);

        foreach (Vector3 neighborOffset in TileNeighborOffsets)
        {
            UpdateTileCollider(tileInfo.Position + neighborOffset);
        }
    }
}