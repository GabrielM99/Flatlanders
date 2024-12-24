using System;
using Flatlanders.Application.Components;
using Flatlanders.Application.Databases;
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

        PrefabDatabase prefabDatabase = new();
        prefabDatabase.Load();

        Entity cameraEntity = Engine.EntityManager.CreateEntity();
        Camera camera = Engine.Graphics.ActiveCamera = cameraEntity.AddComponent<Camera>();
        camera.BackgroundColor = new Color(54, 197, 244);

        Entity tilemapEntity = Engine.EntityManager.CreateEntity();
        Tilemap tilemap = tilemapEntity.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapEntity.AddComponent<TilemapRenderer>();
        tilemapRenderer.Layer = -1;

        Tile grassTile = new(new Sprite(Engine.Content.Load<Texture2D>("Tiles"), new Rectangle(0, 0, 16, 16)));
        Tile rockTile = new(new Sprite(Engine.Content.Load<Texture2D>("Tiles"), new Rectangle(16, 0, 16, 16)));

        Vector2 worldSize = new(256, 256);

        Random random = new();

        for (int x = -(int)(worldSize.X / 2); x < worldSize.X / 2; x++)
        {
            for (int y = -(int)(worldSize.Y / 2); y < worldSize.Y / 2; y++)
            {
                if (random.NextSingle() <= 0.75f)
                {
                    tilemap.SetTile(grassTile, new Vector3(x, y, 0f));
                }
                else
                {
                    tilemap.SetTile(rockTile, new Vector3(x, y, 0f));
                }
            }
        }

        Entity playerEntity = Engine.EntityManager.CreateEntity();
        Player player = playerEntity.AddComponent<Player>();
        player.tilemap = tilemap;
        player.rockTile = rockTile;
        SpriteRenderer playerSpriteRenderer = playerEntity.AddComponent<SpriteRenderer>();
        playerSpriteRenderer.Sprite = new Sprite(Engine.Content.Load<Texture2D>("Player"));

        Entity textEntity = Engine.EntityManager.CreateEntity(prefabDatabase.Text);
        TextRenderer textRenderer = textEntity.GetComponent<TextRenderer>();
        textRenderer.Font = Engine.Content.Load<SpriteFont>("DogicaPixel");
        textRenderer.Text = "Flatlanders";
        textRenderer.Color = Color.Black;
        textEntity.Transform.Pivot = -Vector2.UnitY;
        textEntity.Transform.Position = Vector2.UnitY;
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