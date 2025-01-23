namespace Flatlanders.Core.Inputs.Bindings;

public class MouseBinding(params MouseButton[] value) : InputBinding<MouseButton>(value)
{
    public override bool IsButtonPressed(InputManager inputManager, MouseButton button)
    {
        return inputManager.IsButtonPressed(button);
    }
}
