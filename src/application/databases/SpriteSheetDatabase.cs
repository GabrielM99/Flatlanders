using Flatlanders.Application.SpriteSheets;
using Flatlanders.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.Databases;

public class SpriteSheetDatabase : Database<SpriteSheet>
{
    public PlayerSpriteSheet Player { get; private set; }

    protected override void OnLoad(Engine engine)
    {
        Register(0, Player = new PlayerSpriteSheet(engine.Content.Load<Texture2D>("Player")));
    }
}