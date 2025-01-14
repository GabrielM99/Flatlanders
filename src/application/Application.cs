﻿using Flatlanders.Application.Databases;
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
        
        Engine.IsMouseVisible = true;
        Engine.Window.AllowUserResizing = true;
        
        Engine.Graphics.PixelsPerUnit = 16;
        Engine.Graphics.WindowSize = new Vector2(1280f, 720f);
        Engine.Graphics.SortingAxis = Vector2.UnitY;
        
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