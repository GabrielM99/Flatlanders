using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flatlanders.Core;

public class Engine : Game
{
    private const string ContentDirectory = "Content";

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

        Content.RootDirectory = ContentDirectory;
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        Application.Initialize();
        
        Components.Add(Graphics);
        Components.Add(EntityManager);
        Components.Add(SceneManager);
        
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}
