using WraithLib;
namespace PixelBox;

public class LagMonitor
{
    public bool ENABLED; // Enable or disable the lag monitor
    public bool CONTROLLING_LAG = false;
    public bool WAS_ACTIVE = false; // Track previous window state 
    public Timer LagTimer;
    public int LAG_FPS_LIMIT = 30; // FPS limit before lag algorithm kicks in
    public int LAG_DELETION_RATE; // How many cells will be deleted when lag is detected. Dynamically sized based on the cell count of the game world.
    public int LAG_DELETION_FACTOR = 150; // 200 // Divides the world cell count by this amount in order to determine how many cells will be deleted. Lower values = more cells deleted at once.

    private GameCore Game;
    private World GameWorld;

    public LagMonitor(GameCore game, World world)
    {
        Game = game;
        GameWorld = world;
        LagTimer = new Timer();
    }

    public void MonitorLag()
    {
        bool steam_cleared = false;
        bool water_cleared = false;
        LAG_DELETION_RATE = GameWorld.WorldCells.Count / LAG_DELETION_FACTOR;

        // Monitor the amount of lag
        
        // Enable the lag monitor when the window is not resizing
        if (Globals.ResizingWindow == false && ENABLED == false) { ENABLED = true; }

        // If screen is not active, Pause the timer and return
        if (Game.IsActive == false || Globals.ResizingWindow == true)
        { 
            WAS_ACTIVE = false; 
            LagTimer.Pause();
            return; 
        }
        // If the screen is just activated, wait to control the lag for a decent amount of time.
        if (WAS_ACTIVE == false && Game.IsActive == true && Globals.ResizingWindow == false)
        {
            WAS_ACTIVE = true;
            LagTimer.Start(40); // Timer is distorted because of lag. Timer counts faster
        }

        // Control lag when needed, and the lag timer is inactive.
        if (Game.IsActive == true && Globals.FPS <= LAG_FPS_LIMIT && ENABLED == true && LagTimer.Active == false)
        {        
            // When user is active on screen and lag control is needed, add a delay to prevent deletion of excess cells
            CONTROLLING_LAG = true;
            if (WAS_ACTIVE == true) 
            { LagTimer.Start(0.2f); } // 0.3

            // Remove steam until lag stops
            int steam_count = 0;
            if (GameWorld.WorldCells.Count > LAG_DELETION_RATE)
            {
                foreach (var kvp in GameWorld.WorldCells)
                {
                    Cell cell = kvp.Value;
                    if (steam_count < LAG_DELETION_RATE && cell is Steam && cell is not Tornado)
                    {
                        GameWorld.RemoveCell(cell);
                        steam_count++;
                    }
                    if (GameWorld.SteamCells.Count == 0) { steam_cleared = true; }
                }
            }

            // Remove water until lag stops
            int water_count = 0;
            if (GameWorld.WorldCells.Count > LAG_DELETION_RATE)
            {
                foreach (var kvp in GameWorld.WorldCells)
                {
                    Cell cell = kvp.Value;
                    if (water_count < LAG_DELETION_RATE && cell is Water)
                    {
                        GameWorld.RemoveCell(cell);
                        water_count++;
                    }
                    if (GameWorld.WaterCells.Count == 0) { water_cleared = true; }
                }
            }

            // Remove cells until lag stops
            int count = 0;
            if (GameWorld.WorldCells.Count > LAG_DELETION_RATE && water_cleared == true && steam_cleared == true)
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