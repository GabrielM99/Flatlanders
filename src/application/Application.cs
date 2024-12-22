using Flatlanders.Application.Scenes;
using Flatlanders.Core;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application;

public class Application : IApplication
{
    private Engine Engine { get; }
    
    public Application()
    {
        Engine = new(this);
        Engine.Graphics.PixelsPerUnit = 16;
        Engine.Graphics.Resolution = new Vector2(1280f, 720f);
        Engine.Graphics.IsFullscreen = true;
        Engine.Run();
    }

    public void Initialize()
    {
        Engine.SceneManager.LoadScene<WorldScene>();
    }
}