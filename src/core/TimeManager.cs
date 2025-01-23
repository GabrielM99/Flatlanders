using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class TimeManager(Game game) : DrawableGameComponent(game)
{
    public float DeltaTime { get; private set; }
    public float TicksPerSecond { get; private set; }
    public float FramesPerSecond { get; private set; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        TicksPerSecond = 1f / DeltaTime;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        FramesPerSecond = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}