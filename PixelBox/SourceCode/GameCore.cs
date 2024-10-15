using System;
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
    public Vector2 WorldSize {get; private set;} = new Vector2(100, 100);
    public enum SelectableCellTypes { Water, Sand, Stone, Steam };
    public SelectableCellTypes SelectedCellType {get; private set;} = SelectableCellTypes.Water; // Default selected cell is water

    // Variables
    private GraphicsDeviceManager graphics;
    public const int FRAMES_PER_SECOND = 60;
    private SpriteFont Font {get; set;}
    private int CellCount {get; set;} = 0;
    private Vector2 UI_Position_0 = new Vector2(0, 0);
    private Vector2 UI_Position_1 = new Vector2(0, 50);

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

        // Draw debug info
        Globals.Sprite_Batch.DrawString(Font, CellCount.ToString(), UI_Position_0, Color.White);
        Globals.Sprite_Batch.DrawString(Font, "FPS: " + Globals.FPS, UI_Position_1, Color.White);
        
        // End drawing to the screen
        Globals.Sprite_Batch.End();
    }

    private void ManageMouse()
    {
        if (Globals.CurrentMouseState.LeftButton == ButtonState.Pressed && Globals.IsMouseInBounds)
        {
            switch (SelectedCellType)
            {
                case SelectableCellTypes.Water:
                    GameWorld.TryAddCell ( new Water(Globals.MousePositionOnCanvas, GameWorld) );
                    break;
                case SelectableCellTypes.Sand:
                    GameWorld.TryAddCell( new Sand(Globals.MousePositionOnCanvas, GameWorld) );
                    break;
                case SelectableCellTypes.Stone:
                    GameWorld.TryAddCell( new Stone(Globals.MousePositionOnCanvas, GameWorld) );
                    break;
                case SelectableCellTypes.Steam:
                    GameWorld.TryAddCell( new Steam(Globals.MousePositionOnCanvas, GameWorld) );
                    break;
            }
        }
        if (Globals.CurrentMouseState.RightButton == ButtonState.Pressed && Globals.IsMouseInBounds)
        {
            Cell cell = GameWorld.GetCell(Globals.MousePositionOnCanvas);
            GameWorld.RemoveCell(cell);
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
    }

    public void OnClientSizeChanged(object sender, EventArgs e)
    {
        Globals.ClientSizeChanged(Window.ClientBounds.Width, Window.ClientBounds.Height, GameWorld.WorldCanvas);
    }

    //private void PlaceCellsInGrid  ... or something
}
