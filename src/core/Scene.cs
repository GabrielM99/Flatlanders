namespace Flatlanders.Core;

public abstract class Scene
{
    public Engine Engine { get; }

    public Scene(Engine engine)
    {
        Engine = engine;
    }

    public virtual void Load() { }
    public virtual void Unload() { }
}