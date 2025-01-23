using System.Collections.Generic;
using Flatlanders.Core.Inputs.Bindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Core.Inputs;

public class InputManager : GameComponent
{
    public Vector2 MousePosition => MouseState.Position.ToVector2();

    private KeyboardState KeyboardState { get; set; }
    private MouseState MouseState { get; set; }
    private GamePadState GamePadState { get; set; }

    private List<InputAction> Actions { get; }

    public InputManager(Game game) : base(game)
    {
        Actions = [];
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        KeyboardState = Keyboard.GetState();
        MouseState = Mouse.GetState();
        // TODO: Support multiple players.
        GamePadState = GamePad.GetState(PlayerIndex.One);

        foreach (InputAction action in Actions)
        {
            action.OnUpdate();
        }
    }

    public InputAction CreateAction(params IInputBinding[] bindings)
    {
        InputAction action = new(this, bindings);
        Actions.Add(action);
        return action;
    }

    public bool IsButtonPressed(Keys button)
    {
        return KeyboardState.IsKeyDown(button);
    }

    public bool IsButtonPressed(Buttons button)
    {
        return GamePadState.IsButtonDown(button);
    }

    public bool IsButtonPressed(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed,
            MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => MouseState.RightButton == ButtonState.Pressed,
            MouseButton.XButton1 => MouseState.XButton1 == ButtonState.Pressed,
            MouseButton.XButton2 => MouseState.XButton2 == ButtonState.Pressed,
            _ => false,
        };
    }
}
