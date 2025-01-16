using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core;

public interface IDrawer
{
    void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth);
}