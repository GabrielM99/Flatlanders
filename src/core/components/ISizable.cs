using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Components;

public interface ISizable
{
    Vector2 GetSize(TransformSpace space);
}