using Flatlanders.Application.Animations;
using Flatlanders.Core;

namespace Flatlanders.Application.Databases;

public class AnimationDatabase : Database<Animation>
{
    public PlayerIdleAnimation PlayerIdle { get; private set; }
    public PlayerWalkAnimation PlayerWalk { get; private set; }
    public PlayerBlinkAnimation PlayerBlink { get; private set; }

    protected override void OnLoad(Engine engine)
    {
        Register(0, PlayerIdle = new PlayerIdleAnimation(engine));
        Register(1, PlayerWalk = new PlayerWalkAnimation(engine));
        Register(2, PlayerBlink = new PlayerBlinkAnimation(engine));
    }
}