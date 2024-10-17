using System;
using System.Collections.Generic;
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
    public Vector2 WorldSize {get; private set;} = new Vector2(300, 200);

    // Monitor lag
    public bool LAG_MONITOR = true; // Enable or disable the lag monitor
    public bool CONTROLLING_LAG = false;
    public bool WAS_ACTIVE = false; // Track previous window state 
    public Timer LagTimer;
    public int LAG_FPS_LIMIT = 30; // FPS limit before lag algorithm kicks in
    public int LAG_DELETION_RATE; // How many cells will be deleted when lag is detected. Dynamically sized based on the cell count of the game world.
    
    public enum SelectableCellTypes { Water, Sand, Stone, Steam, Lava, Smoke, Fire, Wood, Acid, PoisonFog, Tornado };
    public SelectableCellTypes SelectedCellType {get; private set;} = SelectableCellTypes.Water; // Default selected cell is water
   
    // Time cycle
    public DayNightCycle TimeCycle;
    public Dictionary<int, Color> TimeCycleColors {get; private set;}
    public int TimeCycleLength = 60; // Seconds

    // Variables & private properties
    private GraphicsDeviceManager graphics;
    public int ScrollValue {get; private set;} = 1;
    public const int FRAMES_PER_SECOND = 60; // 2 minimum
    public SpriteFont Font {get; private set;}

    // UI
    AwfulUI UI;

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
        LagTimer = new Timer();

        // Init general
        Font = Content.Load<SpriteFont>("Font");
        Random = new Random();

        // Init world
        CellStats.InitCellStats();
        GameWorld = new World( new Vector2(WorldSize.X, WorldSize.Y), new Color(100, 100, 180));

        // Init day and night cycle
        TimeCycleColors = new Dictionary<int, Color>()
        {
            { 0, new Color(100, 100, 180) },
            { 1, new Color(140, 140, 180) },
            { 2, new Color(170, 110, 100) },
            { 3, new Color(60, 60, 120) }
        };
        TimeCycle = new DayNightCycle(TimeCycleLength, GameWorld.WorldCanvas, TimeCycleColors[0], TimeCycleColors[1], TimeCycleColors[2], TimeCycleColors[3], DayNightCycle.TimeOfDay.Morning );

        // Init UI
        UI = new AwfulUI(this);
    }

    protected override void Update(GameTime game_time)
    {
        base.Update(game_time);
        LagTimer.Update();

        // Update WraithLib
        Globals.Update(game_time, GameWorld.WorldCanvas);

        // Update game world
        GameWorld.Update();
        TimeCycle.Update();

        // Allow user input
        if (LAG_MONITOR == true && Globals.FPS > LAG_FPS_LIMIT) { ManageMouse(); }
        SelectCellType();

        MonitorLag();
    }

    protected override void Draw(GameTime game_time)
    {
        base.Draw(game_time);
        
        // Activate the Canvas, then begin drawing
        GameWorld.WorldCanvas.Activate();
        Globals.Sprite_Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
        
        GameWorld.Draw();

        // End drawing on the canvas
        Globals.Sprite_Batch.End();
        
        // Draw the Canvas to the screen, and then anything else drawn is also to the screen
        Globals.Sprite_Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
        GameWorld.WorldCanvas.Draw(Globals.Sprite_Batch);

        // Draw the UI in screen space
        UI.Draw();
        
        // End drawing to the screen
        Globals.Sprite_Batch.End();

        // Keep the time
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
        KeyboardState keyboard_state = Globals.CurrentKeyboardState;
        Keys[] pressed_keys = keyboard_state.GetPressedKeys();
        foreach (Keys key in pressed_keys)
        {
            switch (key)
            {
                case Keys.D1:
                    SelectedCellType = SelectableCellTypes.Water;
                    return;
                case Keys.D2:
                    SelectedCellType = SelectableCellTypes.Sand;
                    return;
                case Keys.D3:
                    SelectedCellType = SelectableCellTypes.Stone;
                    return;
                case Keys.D4:
                    SelectedCellType = SelectableCellTypes.Steam;
                    return;
                case Keys.D5:
                    SelectedCellType = SelectableCellTypes.Lava;
                    return;
                case Keys.D6:
                    SelectedCellType = SelectableCellTypes.Smoke;
                    return;
                case Keys.D7:
                    SelectedCellType = SelectableCellTypes.Fire;
                    return;
                case Keys.D8:
                    SelectedCellType = SelectableCellTypes.Wood;
                    return;
                case Keys.D9:
                    SelectedCellType = SelectableCellTypes.Acid;
                    return;
                case Keys.D0:
                    SelectedCellType = SelectableCellTypes.PoisonFog;
                    return;
                case Keys.OemMinus:
                    SelectedCellType = SelectableCellTypes.Tornado;
                    return;
            }
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
                        case SelectableCellTypes.PoisonFog:
                            GameWorld.TryAddCell( new PoisonFog(grid_coords, GameWorld) );
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

    private void MonitorLag()
    {
        bool steam_cleared = false;
        LAG_DELETION_RATE = GameWorld.WorldCells.Count / 200;

        // Monitor the amount of lag

        // If screen is not active, Pause the timer and return
        if (IsActive == false || Globals.ResizingWindow == true)
        { 
            WAS_ACTIVE = false; 
            LagTimer.Pause();
            Console.WriteLine("Screen is not active. Timer Paused."); 
            return; 
        }
        // If the screen is just activated, wait to control the lag for a decent amount of time.
        if (WAS_ACTIVE == false && IsActive == true && Globals.ResizingWindow == false)
        {
            WAS_ACTIVE = true;
            LagTimer.Start(0.5f);
            Console.WriteLine("Screen activated. Starting Timer.");
        }

        // Control lag when needed, and the lag timer is inactive.
        if (IsActive == true && Globals.FPS <= LAG_FPS_LIMIT && LAG_MONITOR == true && LagTimer.Active == false)
        {        
            Console.WriteLine("Controlling lag. Timer is: " + LagTimer.Active + " " + LagTimer.ElapsedTime);
            CONTROLLING_LAG = true;

            // When user is active on screen and lag control is needed, add a delay to prevent deletion of excess cells
            if (WAS_ACTIVE == true) 
            { LagTimer.Start(0.01f); }

            // Remove all steam
            if (steam_cleared == false)
            {
                foreach (var kvp in GameWorld.WorldCells)
                {
                    Cell cell = kvp.Value;
                    if (cell is Steam)
                    {
                        GameWorld.RemoveCell(cell);
                        steam_cleared = true;
                    }
                }
            }

            // Remove cells until lag stops
            int count = 0;
            if (GameWorld.WorldCells.Count > LAG_DELETION_RATE)
            {
                foreach (var kvp in GameWorld.WorldCells) 
                {
                    Cell cell = kvp.Value;
                    if (count < LAG_DELETION_RATE)
                    {
                        GameWorld.RemoveCell(cell);
                        count++;
                    }
                }
            }
        }
        // When the timer is done, reset controlling lag variable. If no more lag control is needed, CONTROLLING_LAG will remain false.
        else if(LagTimer.Active == false) 
        {CONTROLLING_LAG = false;}
    }

}