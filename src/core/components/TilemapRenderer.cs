using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public class TilemapRenderer : Renderer
{
    // TODO: Implement this.
    public override Vector2 Size { get; }

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
            Camera camera = Engine.Graphics.ActiveCamera;
            RectangleF cameraBounds = camera.Entity.Transform.Bounds;
            Vector3 cameraMin = new Vector3(cameraBounds.TopLeft, 0f) - Vector3.One;
            Vector3 cameraMax = new Vector3(cameraBounds.BottomRight, 0f) + Vector3.One;

            foreach (KeyValuePair<Vector3, Tile> tileByPosition in Tilemap.GetTiles(cameraMin, cameraMax))
            {
                Vector3 position = tileByPosition.Key;
                Tile tile = tileByPosition.Value;

                if (tile != null)
                {
                    Engine.Graphics.DrawSprite(tile.Sprite, Entity.Transform, new Vector2(position.X, position.Y), Color, Effects, Layer);
                }
            }
        }
    }
}