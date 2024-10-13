using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WraithLib;
namespace PixelBox;

public class GameCore : Game
{   
    // Testing
    Water[,] watertest;

    // Properties
    public static Random Random {get; private set;}
    public World GameWorld {get; private set;}
    public Canvas WorldCanvas {get; set;}
    public static Vector2 WorldCanvasSize {get; private set;} = new Vector2(100, 100); // need to clamp world boundary based on this

    // Variables
    private GraphicsDeviceManager graphics;
    public const int FRAMES_PER_SECOND = 60;

    public GameCore()
    {   
        graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;

        TargetElapsedTime = TimeSpan.FromSeconds( 1.0 / FRAMES_PER_SECOND);
        IsFixedTimeStep = true;

        watertest = new Water[20, 50];
    }

    protected override void Initialize() { base.Initialize(); }

    protected override void LoadContent()
    {   
        base.LoadContent();
        // Init WraithLib and a random instance to use
        Globals.Initialize(graphics, this.Content);
        Random = new Random();

        CellStats.InitCellStats();
        GameWorld = new World();
        WorldCanvas = new Canvas((int)WorldCanvasSize.X, (int)WorldCanvasSize.Y);

        // Testing
        for (int i = 0; i < watertest.GetLength(0); i++)
        {
            for (int j = 0; j < watertest.GetLength(1); j++)
            {
                watertest[i,j] = new Water(new Vector2(i,j), GameWorld);
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        GameWorld.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        // Activate the Canvas, and begin drawing
        WorldCanvas.Activate(Color.DimGray);
        Globals.Sprite_Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
        
        GameWorld.Draw();

        Globals.Sprite_Batch.End();

        // Draw the Canvas to the screen, and anything else drawn is to the screen
        Globals.Sprite_Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
        WorldCanvas.Draw(Globals.Sprite_Batch);
        Globals.Sprite_Batch.End();
    }

    public void OnClientSizeChanged(object sender, EventArgs e)
    {
        Globals.ClientSizeChanged(Window.ClientBounds.Width, Window.ClientBounds.Height, WorldCanvas);
    }
}
