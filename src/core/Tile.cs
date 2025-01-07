namespace Flatlanders.Core;

public class Tile
{
    public Sprite Sprite { get; }
    public bool IsCollidable { get; }

    public Tile(Sprite sprite, bool isCollidable = true)
    {
        Sprite = sprite;
        IsCollidable = isCollidable;
    }
}