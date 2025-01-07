using Microsoft.Xna.Framework;
using MonoGame.Extended;

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
        Physics = new Physics(this, new RectangleF(int.MinValue * 0.5f, int.MinValue * 0.5f, int.MaxValue, int.MaxValue));

        if (string.IsNullOrEmpty(Content.RootDirectory))
        {
            Content.RootDirectory = DefaultContentDirectory;
        }
    }

    protected override void Initialize()
    {
        Components.Add(Time);
        Components.Add(Graphics);
        Components.Add(EntityManager);
        Components.Add(SceneManager);
        Components.Add(Physics);
        
        Application.Initialize();

        base.Initialize();
    }
}
