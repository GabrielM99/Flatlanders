using System;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application.Components;

public class Player : Component
{
    public Player(Entity entity) : base(entity)
    {
    }

    float t;

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        t += deltaTime;
        Entity.Transform.Rotation += MathHelper.Pi / 2 * deltaTime;
        //Entity.Transform.Position += (MathF.Sin(t) * Vector2.UnitX + MathF.Cos(t) * Vector2.UnitY) * deltaTime;
        Entity.Transform.Position = Vector2.UnitY * MathF.Sin(t) * 3f;
    }
}