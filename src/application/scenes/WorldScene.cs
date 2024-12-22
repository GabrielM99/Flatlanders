using System;
using Flatlanders.Application.Components;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.Scenes;

public class WorldScene : Scene
{
    public WorldScene(Engine engine) : base(engine)
    {
    }

    public override void Load()
    {
        base.Load();

        Entity cameraEntity = Engine.EntityManager.CreateEntity();
        Engine.Graphics.ActiveCamera = cameraEntity.AddComponent<Camera>();

        Entity playerEntity = Engine.EntityManager.CreateEntity();
        playerEntity.AddComponent<Player>();
        SpriteRenderer playerSpriteRenderer = playerEntity.AddComponent<SpriteRenderer>();
        playerSpriteRenderer.Sprite = new Sprite(Engine.Content.Load<Texture2D>("Tiles"), new Rectangle(16, 0, 16, 16));

        Entity textEntity = Engine.EntityManager.CreateEntity();
        TextRenderer textRenderer = textEntity.AddComponent<TextRenderer>();
        textRenderer.Font = Engine.Content.Load<SpriteFont>("Arial");
        textRenderer.Text = "Flatlanders";
        textRenderer.Color = Color.Orange;
        textEntity.Transform.Pivot = -Vector2.UnitY;
        textEntity.Transform.Space = TransformSpace.Screen;
        textEntity.Transform.Anchor = TransformAnchor.Top;

        Entity slotParentEntity = Engine.EntityManager.CreateEntity();
        slotParentEntity.Transform.Position = new Vector2(0f, -1f);
        slotParentEntity.Transform.Pivot = new Vector2(0f, 1f);
        slotParentEntity.Transform.Anchor = TransformAnchor.Bottom;
        slotParentEntity.Transform.Space = TransformSpace.Screen;
        slotParentEntity.AddComponent<HorizontalContainer>().Spacing = 1f;

        for (int i = 0; i < 5; i++)
        {
            Entity slotEntity = Engine.EntityManager.CreateEntity();
            slotEntity.Transform.Parent = slotParentEntity.Transform;
            slotEntity.Transform.Space = TransformSpace.Screen;
            slotEntity.Transform.Anchor = TransformAnchor.Center;
            SpriteRenderer slotSpriteRenderer = slotEntity.AddComponent<SpriteRenderer>();
            slotSpriteRenderer.Sprite = new Sprite(Engine.Content.Load<Texture2D>("Slots"), new Rectangle(0, 0, 16, 16));
        }
    }
}