using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    public override void OnDraw(Graphics graphics, short layer, Vector2 sortingOrigin = default)
    {
        if (Tilemap != null)
        {
            Camera camera = Engine.Graphics.ActiveCamera;
            RectangleF cameraBounds = new(camera.Entity.Node.Position - camera.Entity.Node.Size * 0.5f, camera.Entity.Node.Size);
            Vector3 cameraMin = new Vector3(cameraBounds.TopLeft, 0f) - Vector3.One;
            Vector3 cameraMax = new Vector3(cameraBounds.BottomRight, 0f) + Vector3.One;

            foreach (KeyValuePair<Vector3, Tile> tileByPosition in Tilemap.GetTiles(cameraMin, cameraMax))
            {
                Vector3 position = tileByPosition.Key;
                Tile tile = tileByPosition.Value;

                if (tile != null)
                {
                    Vector2 size = tile.Sprite.Rectangle.Size.ToVector2();
                    Engine.Graphics.Draw(new Transform() { Position = new Vector2(position.X, position.Y), Size = size }, new SpriteDrawer(tile.Sprite, Effects), Color, layer);
                }
            }
        }
    }
}