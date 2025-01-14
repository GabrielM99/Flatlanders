using Flatlanders.Application.Animations;
using Flatlanders.Core;

namespace Flatlanders.Application.Databases;

public class AnimationDatabase : Database<Animation>
{
    public PlayerWalkAnimation PlayerWalk { get; private set; }

    protected override void OnLoad(Engine engine)
    {
        Register(0, PlayerWalk = new PlayerWalkAnimation(engine));
    }
}