using Flatlanders.Core.Graphics;

namespace Flatlanders.Core.Tiles;

public class Tile
{
    public Sprite[] Sprites { get; }
    public bool IsCollidable { get; }

    public Tile(Sprite sprite, bool isCollidable = true)
    {
        Sprites = [sprite];
        IsCollidable = isCollidable;
    }

    public Tile(Sprite[] sprites, bool isCollidable = true)
    {
        Sprites = sprites;
        IsCollidable = isCollidable;
    }

    public virtual void OnRefresh(TileInfo tileInfo)
    {
        if (Sprites != null && Sprites.Length > 0)
        {
            tileInfo.Sprite = Sprites[0];
        }
    }
}