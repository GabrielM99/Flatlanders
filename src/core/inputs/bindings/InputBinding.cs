namespace Flatlanders.Core.Inputs.Bindings;

public abstract class InputBinding<T>(T[] buttons) : IInputBinding
{
    public T[] Buttons { get; } = buttons;

    public abstract bool IsButtonPressed(InputManager inputManager, T button);

    public bool IsExecuting(InputManager inputManager)
    {
        foreach (T button in Buttons)
        {
            if (!IsButtonPressed(inputManager, button))
            {
                return false;
            }
        }

        return true;
    }
}
