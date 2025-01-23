using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Core.Inputs.Bindings;

public class KeyboardBinding(params Keys[] buttons) : InputBinding<Keys>(buttons)
{
    public override bool IsButtonPressed(InputManager inputManager, Keys button)
    {
        return inputManager.IsButtonPressed(button);
    }
}
