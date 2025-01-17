using Flatlanders.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.Databases;

public class SpriteDatabase : Database<Sprite>
{
    public Sprite Hand { get; private set; }

    protected override void OnLoad(Engine engine)
    {
        Register(0, Hand = new Sprite(engine.Content.Load<Texture2D>("Hand")));
    }
}