using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class Engine : Game
{
    private const string DefaultContentDirectory = "Content";

    public Graphics Graphics { get; }
    public EntityManager EntityManager { get; }
    public SceneManager SceneManager { get; }

    private IApplication Application { get; }

    public Engine(IApplication application)
    {
        Application = application;

        Graphics = new(this);
        EntityManager = new(this);
        SceneManager = new(this);

        if (string.IsNullOrEmpty(Content.RootDirectory))
        {
            Content.RootDirectory = DefaultContentDirectory;
        }
    }

    protected override void Initialize()
    {
        Application.Initialize();

        Components.Add(Graphics);
        Components.Add(EntityManager);
        Components.Add(SceneManager);

        base.Initialize();
    }
}
