using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WraithLib;
namespace PixelBox;

public class GameCore : Game
{   
    // Properties
    public static Random Random {get; private set;}
    public World GameWorld {get; private set;}
    public Vector2 WorldSize {get; private set;} = new Vector2(150, 150);
    public enum SelectableCellTypes { Water, Sand, Stone, Steam, Lava, Smoke };
    public SelectableCellTypes SelectedCellType {get; private set;} = SelectableCellTypes.Water; // Default selected cell is water

    // Variables
    private GraphicsDeviceManager graphics;
    private int ScrollValue {get; set;} = 1;
    public const int FRAMES_PER_SECOND = 60;
    private SpriteFont Font {get; set;}
    private int CellCount {get; set;} = 0;
    private Vector2 UI_Position_0 = new Vector2(0, 0);
    private Vector2 UI_Position_1 = new Vector2(0, 50);
    private Vector2 UI_Position_2 = new Vector2(0, 100);
    private Vector2 UI_Position_3 = new Vector2(0, 150);

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

        // Init
        Font = Content.Load<SpriteFont>("Font");
        Random = new Random();

        // Init world
        CellStats.InitCellStats();
        GameWorld = new World( new Vector2(WorldSize.X, WorldSize.Y) );
    }

    protected override void Update(GameTime game_time)
    {
        base.Update(game_time);

        Globals.Update(game_time, GameWorld.WorldCanvas);
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
        else if (ScrollValue > 60) { ScrollValue = 60; }

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
        }
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D2) )
        {
            SelectedCellType = SelectableCellTypes.Sand;
        }
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D3) )
        {
            SelectedCellType = SelectableCellTypes.Stone;
        }
        if ( Globals.CurrentKeyboardState.IsKeyDown(Keys.D4) )
        {
            SelectedCellType = SelectableCellTypes.Steam;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D5))
        {
            SelectedCellType = SelectableCellTypes.Lava;
        }
        if (Globals.CurrentKeyboardState.IsKeyDown(Keys.D6))
        {
            SelectedCellType = SelectableCellTypes.Smoke;
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
        Globals.Sprite_Batch.DrawString(Font, "Cell Count: " + CellCount.ToString(), UI_Position_0, Color.White);
        Globals.Sprite_Batch.DrawString(Font, "FPS: " + Globals.FPS, UI_Position_1, Color.White);
        Globals.Sprite_Batch.DrawString(Font, "Selected: " + SelectedCellType.ToString(), UI_Position_2, Color.White);
        Globals.Sprite_Batch.DrawString(Font, "Brush Size: " + ScrollValue/2, UI_Position_3, Color.Aquamarine);
    }

}
