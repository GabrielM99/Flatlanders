using Flatlanders.Application.Scenes;
using Flatlanders.Core;

namespace Flatlanders.Application;

public class Application : IApplication
{
    private Engine Engine { get; }
    
    public Application()
    {
        Engine = new(this);
        Engine.Graphics.PixelsPerUnit = 16;
        Engine.Run();
    }

    public static void Main()
    {
        new Application();
    }

    public void Initialize()
    {
        Engine.SceneManager.LoadScene<WorldScene>();
    }
}