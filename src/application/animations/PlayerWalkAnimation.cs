using Flatlanders.Application.Components;
using Flatlanders.Core;

namespace Flatlanders.Application.Animations;

public class PlayerWalkAnimation : Animation<Player>
{
    public override int Frames => 80;

    private AnimationProperty<Sprite> Sprite { get; }

    public PlayerWalkAnimation()
    {
        
    }

    public override void Bind(RuntimeAnimation runtimeAnimation, Player obj)
    {
        
    }
}