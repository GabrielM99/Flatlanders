using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flatlanders.Core;

public interface IDrawer
{
    ITransform Transform { get; set; }
    Color Color { get; set; }
    Vector2 Origin { get; set; }
    Vector2 SortingOrigin { get; set; }
    SpriteEffects Effects { get; set; }
    short Layer { get; set; }
    
    void Draw(SpriteBatch spriteBatch, RectangleF bounds, float layerDepth);
}