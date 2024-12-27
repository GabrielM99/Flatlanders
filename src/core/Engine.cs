using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class Engine : Game
{
    private const string DefaultContentDirectory = "Content";

    public Graphics Graphics { get; }
    public Time Time { get; }
    public EntityManager EntityManager { get; }
    public SceneManager SceneManager { get; }
    public Physics Physics { get; }

    private IApplication Application { get; }

    public Engine(IApplication application)
    {
        Application = application;

        Graphics = new Graphics(this);
        Time = new Time(this);
        EntityManager = new EntityManager(this);
        SceneManager = new SceneManager(this);

        if (string.IsNullOrEmpty(Content.RootDirectory))
        {
            Content.RootDirectory = DefaultContentDirectory;
        }
    }

    protected override void Initialize()
    {
        Application.Initialize();

        Components.Add(Time);
        Components.Add(Graphics);
        Components.Add(EntityManager);
        Components.Add(SceneManager);

        base.Initialize();
    }
}
