using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Application.Components;

public class Player : Component
{     
    public Tilemap tilemap;
    public Tile rockTile;
    public TextRenderer debugTextRenderer;

    public Player(Entity entity) : base(entity)
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        debugTextRenderer.Text = $"Flatlands 0.0.1\nTPS:{(int)Engine.Time.TicksPerSecond}\nFPS:{(int)Engine.Time.FramesPerSecond}";

        KeyboardState keyboardState = Keyboard.GetState();

        Vector2 direction = Vector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            direction.Y -= 1;
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            direction.Y += 1;
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            direction.X -= 1;
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            direction.X += 1;
        }

        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = mouseState.Position.ToVector2();
        Vector2 worldMousePosition = Engine.Graphics.ScreenToWorldVector(mousePosition);

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            tilemap.SetTile(null, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            tilemap.SetTile(rockTile, new Vector3(worldMousePosition - Vector2.One * 0.5f, 0f));
        }

        Entity.Transform.Position += direction * 5f * deltaTime;
        Engine.Graphics.ActiveCamera.Entity.Transform.Position += direction * 5f * deltaTime;
    }
}