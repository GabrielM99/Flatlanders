using System;
using Flatlanders.Application.Components;
using Flatlanders.Application.Databases;
using Flatlanders.Application.SpriteSheets;
using Flatlanders.Core;
using Flatlanders.Core.Components;
using Flatlanders.Core.Graphics;
using Flatlanders.Core.Scenes;
using Flatlanders.Core.Tiles;
using Flatlanders.Core.Transforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Flatlanders.Application.Scenes;

public class WorldScene(Engine engine) : Scene(engine)
{
    public override void Load()
    {
        base.Load();

        //Engine.RenderManager.AmbientLightColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        PrefabDatabase prefabDatabase = Engine.DatabaseManager.GetDatabase<PrefabDatabase>();

        Entity cameraEntity = Engine.EntityManager.CreateEntity();
        AudioPlayer ambientAudioPlayer = cameraEntity.AddComponent<AudioPlayer>();
        ambientAudioPlayer.Loop = true;
        ambientAudioPlayer.Play(Engine.Content.Load<SoundEffect>("ForestDay"));
        Camera camera = Engine.RenderManager.ActiveCamera = cameraEntity.AddComponent<Camera>();
        PlayerCamera playerCamera = cameraEntity.AddComponent<PlayerCamera>();
        camera.BackgroundColor = new Color(54, 197, 244);

        Tilemap worldTilemap = new();
        Tilemap viewTilemap = new();

        worldTilemap.TileSetted += (tileInfo) =>
        {
            Tile tile = tileInfo.Tile ?? viewTilemap.GetTile(tileInfo.Position);

            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    Vector3 viewTileOffset = new(x, y, 0);
                    Vector3 viewTilePosition = tileInfo.Position + viewTileOffset;

                    int mask = 0;

                    for (int i = 0; i <= 1; i++)
                    {
                        for (int j = 0; j <= 1; j++)
                        {
                            int index = i + j * 2;

                            Tile neighborTile = tileInfo.Tilemap.GetTile(viewTilePosition - new Vector3(i, j, 0));

                            if (neighborTile == tile)
                            {
                                mask |= 1 << index;
                            }
                        }
                    }

                    if (mask == 0)
                    {
                        viewTilemap.SetTile(null, viewTilePosition);
                    }
                    else
                    {
                        viewTilemap.SetTile(tile, viewTilePosition, out TileInfo viewTileInfo);
                        viewTileInfo.Sprite = tile.Sprites[mask];
                        viewTileInfo.SortingOrigin = Vector2.UnitY * 0.25f;
                    }
                }
            }
        };

        Entity tilemapEntity = Engine.EntityManager.CreateEntity();
        TilemapRenderer tilemapRenderer = tilemapEntity.AddComponent<TilemapRenderer>();
        tilemapRenderer.Entity.Node.LocalPosition -= Vector2.One * 0.5f;
        tilemapRenderer.Tilemap = viewTilemap;
        tilemapRenderer.Layer = 0;
        TilemapCollider tilemapCollider = tilemapEntity.AddComponent<TilemapCollider>();
        tilemapCollider.Tilemap = worldTilemap;

        DualGridTileSpriteSheet rockSpriteSheet = new(Engine.Content.Load<Texture2D>("Rock2"));
        DualGridTileSpriteSheet grassSpriteSheet = new(Engine.Content.Load<Texture2D>("Grass"));

        Tile grassTile = new(grassSpriteSheet.Sprites, false);
        Tile rockTile = new(rockSpriteSheet.Sprites);

        Vector2 worldSize = new(32, 32);

        Random random = new();

        int seed = random.Next();
        Console.WriteLine($"Generating world with seed {seed}.");

        Random tileRandom = new(seed);

        for (int x = -(int)(worldSize.X / 2); x < worldSize.X / 2; x++)
        {
            for (int y = -(int)(worldSize.Y / 2); y < worldSize.Y / 2; y++)
            {
                worldTilemap.SetTile(grassTile, new Vector3(x, y, -1f));

                if (tileRandom.NextSingle() > 0.75f)
                {
                    worldTilemap.SetTile(rockTile, new Vector3(x, y, 0f));
                }
            }
        }

        Entity debugTextEntity = Engine.EntityManager.CreateEntity();
        TextRenderer debugTextRenderer = debugTextEntity.AddComponent<TextRenderer>();
        debugTextRenderer.Font = Engine.Content.Load<SpriteFont>("DogicaPixel");
        debugTextRenderer.Color = Color.Black;
        debugTextEntity.Node.Pivot = -Vector2.One;
        debugTextEntity.Node.Position = Vector2.One;
        debugTextEntity.Node.Scale = Vector2.One * 0.5f;
        debugTextEntity.Node.Space = TransformSpace.Screen;
        debugTextEntity.Node.Anchor = TransformAnchor.TopLeft;

        Entity playerEntity = Engine.EntityManager.CreateEntity(prefabDatabase.Player, "Player");
        Player player = playerEntity.GetComponent<Player>();
        player.tilemap = worldTilemap;
        player.rockTile = rockTile;
        player.debugTextRenderer = debugTextRenderer;

        playerCamera.Target = playerEntity.Node;

        Entity enemyEntity = Engine.EntityManager.CreateEntity();
        enemyEntity.Node.Position = Vector2.UnitX * 3f;
        RectangleCollider enemyCollider = enemyEntity.AddComponent<RectangleCollider>();
        enemyCollider.Size = new Vector2(0.25f);
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