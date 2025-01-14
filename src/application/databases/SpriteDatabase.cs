using Flatlanders.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.Databases;

public class SpriteDatabase : Database<Sprite>
{
    public Sprite Player { get; private set; }

    protected override void OnLoad(Engine engine)
    {
        Register(0, Player = new Sprite(engine.Content.Load<Texture2D>("")));
    }
}