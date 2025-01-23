namespace Flatlanders.Core.Inputs.Bindings;

public interface IInputBinding
{
    bool IsExecuting(InputManager inputManager);
}
