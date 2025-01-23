using System;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core.Scenes;

public class SceneManager : GameComponent
{
    public Scene LoadedScene { get; private set; }
    public event Action<Scene> SceneLoading;

    private Engine Engine { get; }

    public SceneManager(Engine engine) : base(engine)
    {
        Engine = engine;
    }

    public void LoadScene<T>() where T : Scene
    {
        T scene = (T)Activator.CreateInstance(typeof(T), new object[] { Engine });
        LoadedScene?.Unload();
        LoadedScene = scene;
        SceneLoading?.Invoke(scene);
        scene.Load();
    }
}