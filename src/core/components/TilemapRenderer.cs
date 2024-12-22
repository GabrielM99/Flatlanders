using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public class TilemapRenderer : Renderer
{
    public Tilemap Tilemap { get; set; }

    public TilemapRenderer(Entity entity) : base(entity)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Tilemap ??= Entity.GetComponent<Tilemap>();
    }

    public override void Draw(Graphics graphics)
    {
        if (Tilemap != null)
        {
            foreach (KeyValuePair<Vector3, Tile> tileByPosition in Tilemap.GetTileByPosition())
            {
                Vector3 position = tileByPosition.Key;
                Tile tile = tileByPosition.Value;
                Engine.Graphics.DrawSprite(tile.Sprite, Entity.Transform, new Vector2(position.X, position.Y), Color, Effects, Layer);
            }
        }
    }
}