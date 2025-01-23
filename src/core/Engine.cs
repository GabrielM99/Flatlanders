using Flatlanders.Core.Graphics;
using Flatlanders.Core.Physics;
using Flatlanders.Core.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core;

public class Engine : Game
{
    private const string DefaultContentDirectory = "Content";

    public RenderManager RenderManager { get; }
    public TimeManager TimeManager { get; }
    public EntityManager EntityManager { get; }
    public SceneManager SceneManager { get; }
    public DatabaseManager DatabaseManager { get; }
    public PhysicsManager PhysicsManager { get; }

    private IApplication Application { get; }

    public Engine(IApplication application)
    {
        Application = application;

        RenderManager = new RenderManager(this);
        TimeManager = new TimeManager(this);
        EntityManager = new EntityManager(this);
        SceneManager = new SceneManager(this);
        DatabaseManager = new DatabaseManager(this);
        PhysicsManager = new PhysicsManager(this, new RectangleF(int.MinValue * 0.5f, int.MinValue * 0.5f, int.MaxValue, int.MaxValue));

        if (string.IsNullOrEmpty(Content.RootDirectory))
        {
            Content.RootDirectory = DefaultContentDirectory;
        }
    }

    protected override void Initialize()
    {
        Components.Add(TimeManager);
        Components.Add(RenderManager);
        Components.Add(EntityManager);
        Components.Add(SceneManager);
        Components.Add(PhysicsManager);
        Components.Add(DatabaseManager);

        base.Initialize();

        Application.Initialize();
    }
}
