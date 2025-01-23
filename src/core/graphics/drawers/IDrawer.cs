using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Core.Graphics.Drawers;

public interface IDrawer
{
    void Draw(SpriteBatch spriteBatch, ITransform transform, Color color, float layerDepth);
}