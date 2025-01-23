using Flatlanders.Application.Databases;
using Flatlanders.Application.Scenes;
using Flatlanders.Core;
using Microsoft.Xna.Framework;

namespace Flatlanders.Application;

public class Application : IApplication
{
    private Engine Engine { get; }

    public Application()
    {
        Engine = new Engine(this)
        {
            IsMouseVisible = true
        };

        Engine.SetDebugFlags(EngineDebugFlags.DrawColliders);

        Engine.Window.AllowUserResizing = true;

        Engine.RenderManager.PixelsPerUnit = 16;
        Engine.RenderManager.WindowSize = new Vector2(1280f, 720f);
        Engine.RenderManager.SortingAxis = Vector2.UnitY;

        Engine.DatabaseManager.AddDatabase<PrefabDatabase>();
        Engine.DatabaseManager.AddDatabase<SpriteDatabase>();
        Engine.DatabaseManager.AddDatabase<SpriteSheetDatabase>();
        Engine.DatabaseManager.AddDatabase<AnimationDatabase>();

        Engine.Run();
    }

    public void Initialize()
    {
        Engine.SceneManager.LoadScene<WorldScene>();
    }
}