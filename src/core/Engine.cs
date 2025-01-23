using System;
using System.Collections.Generic;
using Flatlanders.Core.Graphics;
using Flatlanders.Core.Inputs;
using Flatlanders.Core.Physics;
using Flatlanders.Core.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core;

public class Engine : Game
{
    private const string DefaultContentDirectory = "Content";

    public TimeManager TimeManager { get; }
    public RenderManager RenderManager { get; }
    public EntityManager EntityManager { get; }
    public SceneManager SceneManager { get; }
    public DatabaseManager DatabaseManager { get; }
    public PhysicsManager PhysicsManager { get; }
    public InputManager InputManager { get; }

    private HashSet<Enum> DebugFlags { get; }

    private IApplication Application { get; }

    public Engine(IApplication application)
    {
        Application = application;
        DebugFlags = [];

        TimeManager = new TimeManager(this);
        RenderManager = new RenderManager(this);
        EntityManager = new EntityManager(this);
        SceneManager = new SceneManager(this);
        DatabaseManager = new DatabaseManager(this);
        PhysicsManager = new PhysicsManager(this, new RectangleF(int.MinValue * 0.5f, int.MinValue * 0.5f, int.MaxValue, int.MaxValue));
        InputManager = new InputManager(this);

        if (string.IsNullOrEmpty(Content.RootDirectory))
        {
            Content.RootDirectory = DefaultContentDirectory;
        }
    }

    public void SetDebugFlags(params Enum[] flags)
    {
        DebugFlags.UnionWith(flags);
    }

    public bool HasDebugFlag(Enum flag)
    {
        return DebugFlags.Contains(flag);
    }

    protected override void Initialize()
    {
        Components.Add(TimeManager);
        Components.Add(RenderManager);
        Components.Add(EntityManager);
        Components.Add(SceneManager);
        Components.Add(PhysicsManager);
        Components.Add(DatabaseManager);
        Components.Add(InputManager);

        base.Initialize();

        Application.Initialize();
    }
}
