using Flatlanders.Core.Inputs.Bindings;

namespace Flatlanders.Core.Inputs;

public class InputAction(InputManager inputManager, params IInputBinding[] bindings)
{
    public IInputBinding[] Bindings { get; } = bindings;

    public bool IsExecuting { get; private set; }
    public bool WasExecuting { get; private set; }

    private InputManager InputManager { get; } = inputManager;

    public void OnUpdate()
    {
        bool isExecuting = false;

        foreach (IInputBinding binding in Bindings)
        {
            if (binding.IsExecuting(InputManager))
            {
                isExecuting = true;
                break;
            }
        }

        WasExecuting = IsExecuting && !isExecuting;
        IsExecuting = isExecuting;
    }
}
