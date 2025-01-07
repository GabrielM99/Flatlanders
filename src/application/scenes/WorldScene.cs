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
        PlayerCamera playerCamera = cameraEntity.AddComponent<PlayerCamera>();
        camera.BackgroundColor = new Color(54, 197, 244);

        Entity tilemapEntity = Engine.EntityManager.CreateEntity();
        Tilemap tilemap = tilemapEntity.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapEntity.AddComponent<TilemapRenderer>();
        TilemapCollider tilemapCollider = tilemapEntity.AddComponent<TilemapCollider>();
        tilemapRenderer.Layer = -1;

        Tile grassTile = new(new Sprite(Engine.Content.Load<Texture2D>("Tiles"), new Rectangle(0, 0, 16, 16)), false);
        Tile rockTile = new(new Sprite(Engine.Content.Load<Texture2D>("Tiles"), new Rectangle(16, 0, 16, 16)));

        Vector2 worldSize = new(16, 16);

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

        Entity debugTextEntity = Engine.EntityManager.CreateEntity(prefabDatabase.Text);
        TextRenderer debugTextRenderer = debugTextEntity.GetComponent<TextRenderer>();
        debugTextRenderer.Font = Engine.Content.Load<SpriteFont>("DogicaPixel");
        debugTextRenderer.Color = Color.Black;
        debugTextEntity.Node.Pivot = -Vector2.One;
        debugTextEntity.Node.Position = Vector2.One;
        debugTextEntity.Node.Scale = Vector2.One * 0.5f;
        debugTextEntity.Node.Space = TransformSpace.Screen;
        debugTextEntity.Node.Anchor = TransformAnchor.TopLeft;

        Entity playerEntity = Engine.EntityManager.CreateEntity();
        RectangleCollider playerCollider = playerEntity.AddComponent<RectangleCollider>();
        playerCollider.Size = new Vector2(0.5f);
        playerCollider.Offset = new Vector2(0f);
        Rigidbody playerRigidbody = playerEntity.AddComponent<Rigidbody>();
        Player player = playerEntity.AddComponent<Player>();
        player.tilemap = tilemap;
        player.rockTile = rockTile;
        player.debugTextRenderer = debugTextRenderer;
        SpriteRenderer playerSpriteRenderer = playerEntity.AddComponent<SpriteRenderer>();
        playerSpriteRenderer.Sprite = new Sprite(Engine.Content.Load<Texture2D>("Player"), null, new Vector2(0f, 8f));

        playerCamera.Target = playerEntity.Node;

        Entity enemyEntity = Engine.EntityManager.CreateEntity();
        enemyEntity.Node.Position = Vector2.UnitX * 3f;
        RectangleCollider enemyCollider = enemyEntity.AddComponent<RectangleCollider>();
        enemyCollider.Size = new Vector2(0.25f);
        enemyCollider.Offset = new Vector2(0f, 0.25f);
        SpriteRenderer enemySpriteRenderer = enemyEntity.AddComponent<SpriteRenderer>();
        enemySpriteRenderer.Sprite = new Sprite(Engine.Content.Load<Texture2D>("Anti"), null, new Vector2(0f, 8f));

        Entity slotParentEntity = Engine.EntityManager.CreateEntity();
        slotParentEntity.Node.Position = new Vector2(0f, -1f);
        slotParentEntity.Node.Pivot = new Vector2(0f, 1f);
        slotParentEntity.Node.Anchor = TransformAnchor.Bottom;
        slotParentEntity.Node.Space = TransformSpace.Screen;
        slotParentEntity.AddComponent<HorizontalContainer>().Spacing = 1f;

        for (int i = 0; i < 5; i++)
        {
            Entity slotEntity = Engine.EntityManager.CreateEntity();
            slotEntity.Node.Parent = slotParentEntity.Node;
            slotEntity.Node.Space = TransformSpace.Screen;
            slotEntity.Node.Anchor = TransformAnchor.Center;
            SpriteRenderer slotSpriteRenderer = slotEntity.AddComponent<SpriteRenderer>();
            slotSpriteRenderer.Sprite = new Sprite(Engine.Content.Load<Texture2D>("Slots"), new Rectangle(0, 0, 16, 16));
        }
    }
}