using Flatlanders.Core.Graphics;
using Flatlanders.Core.Graphics.Drawers;
using Flatlanders.Core.Tiles;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Components;

public class TilemapRenderer(Entity entity) : Renderer(entity)
{
    public Tilemap Tilemap { get; set; }

    public override Vector2 GetSize(TransformSpace space)
    {
        // TODO: Implement this.
        return default;
    }

    public override void OnDraw(RenderManager renderManager, sbyte layer, Vector2 sortingOrigin = default, sbyte order = 0)
    {
        if (Tilemap != null)
        {
            Camera camera = renderManager.ActiveCamera;

            RectangleF cameraBounds = new(camera.Entity.Position - camera.Entity.Size * 0.5f, camera.Entity.Size);

            Vector3 cameraMin = new Vector3(cameraBounds.TopLeft, 0f) - Vector3.One;
            Vector3 cameraMax = new Vector3(cameraBounds.BottomRight, 0f) + Vector3.One;

            foreach (TileInfo tileInfo in Tilemap.GetTileInfos(cameraMin, cameraMax))
            {
                if (tileInfo != null)
                {
                    Transform tileTransform = new()
                    {
                        Position = Entity.Position +
                            new Vector2(tileInfo.Position.X, tileInfo.Position.Y)
                    };
                    renderManager.Draw(tileTransform, new SpriteDrawer(tileInfo.Sprite, Effects), Color, (sbyte)(layer + tileInfo.Position.Z), sortingOrigin + tileInfo.SortingOrigin, order);
                }
            }
        }
    }
}