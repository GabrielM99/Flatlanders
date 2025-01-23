using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Core.Inputs.Bindings;

public class GamePadBinding(params Buttons[] value) : InputBinding<Buttons>(value)
{
    public override bool IsButtonPressed(InputManager inputManager, Buttons button)
    {
        return inputManager.IsButtonPressed(button);
    }
}
