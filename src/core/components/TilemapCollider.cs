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

    private static Vector3[] TileNeighborOffsets { get; } = new Vector3[]
    {
        Vector3.UnitX, -Vector3.UnitX, -Vector3.UnitY, Vector3.UnitY
    };

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
        TileColliderByPosition = new Dictionary<Vector3, TileCollider>();
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Tilemap ??= Entity.GetComponent<Tilemap>();
    }

    public void AddTileCollider(Vector3 position)
    {
        if (!TileColliderByPosition.ContainsKey(position))
        {
            TileCollider collider = new(position);
            TileColliderByPosition[position] = collider;
            Engine.Physics.AddCollider(collider);
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

    private void OnTilemapTileSetted(Tile tile, Vector3 position)
    {
        UpdateTileCollider(position);

        foreach (Vector3 neighborOffset in TileNeighborOffsets)
        {
            UpdateTileCollider(position + neighborOffset);
        }
    }
}