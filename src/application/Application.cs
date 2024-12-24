using Flatlanders.Application.Scenes;
using Flatlanders.Core;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application;

public class Application : IApplication
{
    private Engine Engine { get; }

    public Application()
    {
        Engine = new Engine(this);
        Engine.Graphics.PixelsPerUnit = 16;
        Engine.Graphics.WindowSize = new Vector2(1280f, 720f);
        Engine.IsMouseVisible = true;
        Engine.Window.AllowUserResizing = true;
        Engine.Run();
    }

    public void Initialize()
    {
        Engine.SceneManager.LoadScene<WorldScene>();
    }
}