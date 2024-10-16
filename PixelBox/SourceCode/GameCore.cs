using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WraithLib;
namespace PixelBox;

public class GameCore : Game
{   
    // Public Properties
    public static Random Random {get; private set;}
    public World GameWorld {get; private set;}
    public Vector2 WorldSize {get; private set;} = new Vector2(220, 220);
    public enum SelectableCellTypes { Water, Sand, Stone, Steam, Lava, Smoke, Fire, Wood, Acid, Tornado };
    public SelectableCellTypes SelectedCellType {get; private set;} = SelectableCellTypes.Water; // Default selected cell is water

    // Variables & private properties
    private GraphicsDeviceManager graphics;
    private int ScrollValue {get; set;} = 1;
    public const int FRAMES_PER_SECOND = 60; // 2 minimum
    private SpriteFont Font {get; set;}
    private int CellCount {get; set;} = 0;

    // UI
    private Color UI_Color = Color.Yellow;
    private Vector2 UI_Position_0 = new Vector2(0, 0);
    private Vector2 UI_Position_1 = new Vector2(0, 20);
    private Vector2 UI_Position_2 = new Vector2(0, 40);
    private Vector2 UI_Position_3 = new Vector2(0, 60);
    private Vector2 UI_Position_4 = new Vector2(0, 80);
    private Vector2 UI_Position_5 = new Vector2(0, 100);
    private Vector2 UI_Position_6 = new Vector2(0, 120);
    private Vector2 UI_Position_7 = new Vector2(0, 140);
    private Vector2 UI_Position_8 = new Vector2(0, 160);
    private Vector2 UI_Position_9 = new Vector2(0, 180);
    private Vector2 UI_Position_10 = new Vector2(0, 200);
    private Vector2 UI_Position_11 = new Vector2(0, 220);


    public GameCore()
    {   
        graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;

        Content.RootDirectory = "Content";

        TargetElapsedTime = TimeSpan.FromSeconds( 1.0 / FRAMES_PER_SECOND);
        IsFixedTimeStep = true;
    }

    protected override void Initialize() { base.Initialize(); }

    protected override void LoadContent()
    {   
        base.LoadContent();

        // Init WraithLib
        Globals.Initialize(graphics, this.Content);

        // Init general
        Font = Content.Load<SpriteFont>("Font");
        Random = new Random();

        // Init world
        CellStats.InitCellStats();
        GameWorld = new World( new Vector2(WorldSize.X, WorldSize.Y) );
    }

    protected override void Update(GameTime game_time)
    {
        base.Update(game_time);

        // Update WraithLib
        Globals.Update(game_time, GameWorld.WorldCanvas);

        // Update game world
        GameWorld.Update();
        
        ManageMouse();
        SelectCellType();
        CellCount = GameWorld.WorldCells.Count;
    }

    protected override void Draw(GameTime game_time)
    {
        base.Draw(game_time);
        
        // Activate the Canvas, then begin drawing
        GameWorld.WorldCanvas.Activate(Color.DimGray);
        Globals.Sprite_Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
        
        GameWorld.Draw();

        // End drawing on the canvas
        Globals.Sprite_Batch.End();
        
        // Draw the Canvas to the screen, and then anything else drawn is also to the screen
        Globals.Sprite_Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
        GameWorld.WorldCanvas.Draw(Globals.Sprite_Batch);

        DrawUI();
        
        // End drawing to the screen
        Globals.Sprite_Batch.End();
        Globals.UpdateGlobalTime(game_time);
    }

    private void ManageMouse()
    {
        int scroll_delta = (Globals.CurrentMouseState.ScrollWheelValue - Globals.PreviousMouseState.ScrollWheelValue) / 120;
        ScrollValue += scroll_delta * 2;
        if (ScrollValue < 2) { ScrollValue = 2; }
        else if (ScrollValue > 30) { ScrollValue = 30; }

        if (Globals.CurrentMouseState.LeftButton == ButtonState.Pressed && Globals.IsMouseInBounds)
        {
            PlaceCellsInGrid(Globals.MousePositionOnCanvas, ScrollValue);
        }
        if (Globals.CurrentMouseState.RightButton == ButtonState.Pressed && Globals.IsMouseInBounds)
        {
            EraceCellsInGrid(Globals.MousePositionOnCanvas, ScrollValue);
        }
    }

    private void SelectCellType()
    {
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D1) )
        {
            SelectedCellType = SelectableCellTypes.Water;
            return;
        }
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D2) )
        {
            SelectedCellType = SelectableCellTypes.Sand;
            return;
        }
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D3) )
        {
            SelectedCellType = SelectableCellTypes.Stone;
            return;
        }
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D4) )
        {
            SelectedCellType = SelectableCellTypes.Steam;
            return;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D5))
        {
            SelectedCellType = SelectableCellTypes.Lava;
            return;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D6))
        {
            SelectedCellType = SelectableCellTypes.Smoke;
            return;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D7))
        {
            SelectedCellType = SelectableCellTypes.Fire;
            return;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D8))
        {
            SelectedCellType = SelectableCellTypes.Wood;
            return;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D9))
        {
            SelectedCellType = SelectableCellTypes.Acid;
            return;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D0))
        {
            SelectedCellType = SelectableCellTypes.Tornado;
            return;
        }
    }

    public void OnClientSizeChanged(object sender, EventArgs e)
    {
        Globals.ClientSizeChanged(Window.ClientBounds.Width, Window.ClientBounds.Height, GameWorld.WorldCanvas);
    }

    /// <summary>
    /// Places cells in a grid centered on the given position on the canvas.
    /// </summary>
    private void PlaceCellsInGrid(Vector2 position, int grid_size)
    {
        // rows
        for (int y = (int)position.Y - grid_size/2; y < position.Y + grid_size/2; y++)
        {
            // columns
            for (int x = (int)position.X - grid_size/2; x < position.X + grid_size/2; x++)
            {
                Vector2 grid_coords = new Vector2(x, y);

                if ( (grid_coords.X >= Vector2.Zero.X && grid_coords.Y >= Vector2.Zero.Y) && (grid_coords.X < GameWorld.WorldCanvasSize.X && grid_coords.Y < GameWorld.WorldCanvasSize.Y) )
                {
                    switch (SelectedCellType)
                    {
                        case SelectableCellTypes.Water:
                            GameWorld.TryAddCell ( new Water(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Sand:
                            GameWorld.TryAddCell( new Sand(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Stone:
                            GameWorld.TryAddCell( new Stone(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Steam:
                            GameWorld.TryAddCell( new Steam(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Lava:
                            GameWorld.TryAddCell( new Lava(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Smoke:
                            GameWorld.TryAddCell( new Smoke(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Fire:
                            GameWorld.TryAddCell( new Fire(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Wood:
                            GameWorld.TryAddCell( new Wood(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Acid:
                            GameWorld.TryAddCell( new Acid(grid_coords, GameWorld) );
                            break;
                        case SelectableCellTypes.Tornado:
                            GameWorld.TryAddCell( new Tornado(grid_coords, GameWorld) );
                            break;
                    }
                }
            }
        }
    }

    private void EraceCellsInGrid(Vector2 position, int grid_size)
    {
        // rows
        for (int y = (int)position.Y - grid_size/2; y < position.Y + grid_size/2; y++)
        {
            // columns
            for (int x = (int)position.X - grid_size/2; x < position.X + grid_size/2; x++)
            {
                Vector2 grid_coords = new Vector2(x, y);
                Cell cell = GameWorld.GetCell(grid_coords);
                if ( (grid_coords.X >= Vector2.Zero.X && grid_coords.Y >= Vector2.Zero.Y) && (grid_coords.X < GameWorld.WorldCanvasSize.X && grid_coords.Y < GameWorld.WorldCanvasSize.Y) && cell != null)
                {
                    GameWorld.RemoveCell(cell);
                }
            }
        }
    }

    private void DrawUI()
    {
        Globals.Sprite_Batch.DrawString(Font, "FPS: " + Globals.FPS, UI_Position_0, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Selected: " + SelectedCellType.ToString(), UI_Position_2, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "" + GameWorld.GetCell(Globals.MousePositionOnCanvas), new Vector2(Globals.MousePosition.X + 20, Globals.MousePosition.Y - 8), UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Brush Size: " + ScrollValue/2, UI_Position_4, UI_Color);
        
        Globals.Sprite_Batch.DrawString(Font, "Water Cells: " + GameWorld.WaterCells.Count, UI_Position_6, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Sand Cells: " + GameWorld.SandCells.Count, UI_Position_7, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Stone Cells: " + GameWorld.StoneCells.Count, UI_Position_8, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Steam Cells: " + GameWorld.SteamCells.Count, UI_Position_9, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Lava Cells: " + GameWorld.LavaCells.Count, UI_Position_10, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Wood Cells: " + GameWorld.WoodCells.Count, UI_Position_11, UI_Color);
    }

}
