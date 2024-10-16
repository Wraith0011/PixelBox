using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WraithLib;
namespace PixelBox;

/// <summary>
/// This class keeps track of Cell information such as color dictionaries for each cell type, and the shared 1x1 texture for all cells.
/// </summary>
public static class CellStats
{    
    public static Texture2D CellTexture {get; private set;}
    public static Dictionary<int, Color> WaterColors {get; private set;}
    public static Dictionary<int, Color> SandColors {get; private set;}
    public static Dictionary<int, Color> StoneColors {get; private set;}
    public static Dictionary<int, Color> SteamColors {get; private set;}
    public static Dictionary<int, Color> LavaColors {get; private set;}
    public static Dictionary <int, Color> SmokeColors {get; private set;}
    public static Dictionary<int, Color> FireColors {get; private set;}
    public static Dictionary<int, Color> WoodColors {get; private set;}
    public static Dictionary<int, Color> AcidColors {get; private set;}
    public static Dictionary<int, Color> TornadoColors {get; private set;}


    public static int WATER_FLOW_FACTOR = 150; // How often water flows into other water. Lower values = more flow.
    public static int WATER_DISPERSAL_RATE = 5; // How many additional neighboring cells to check for horizontal movement.
    public static int WATER_CONVERSION_CHANCE = 50000; // The chance that water will turn into steam. Higher values = less chance.
    
    public static int STONE_EROSION_CHANCE = 300; // How often water will erode stone. Higher values = less chance.
    public static int SAND_DISPLACEMENT_CHANCE = 8; //25 // How often sand will be displaced by water. Higher values = less chance.
    public static int SAND_COMPRESSION_FACTOR = 10; // How many sand cells have to be on top of each other in order to compress into stone.
    public static int SAND_COMPRESSION_CHANCE = 1000; // Chance that sand will compress into stone when conditions are met. Higher values = less chance.

    public static int STEAM_DISPERSAL_RATE = 3; // How many additional neighboring cells to check for horizontal movement.
    public static int STEAM_DISPERSAL_CHANCE = 5; // The chance that steam will seek to flow horizontally to empty spaces. Higher values = less chance.
    public static int STEAM_FLOW_FACTOR = 25; // How often steam flows into other steam. Lower values = more flow. (25)
    public static int STEAM_CONVERSION_CHANCE = 25000; // The chance that steam will turn into water. Higher values = less chance.
    
    public static int LAVA_FLOW_FACTOR = 800; // How often lava flows into other lava. Lower values = more flow.
    public static int LAVA_DISPERSAL_CHANCE = 10; // Chance for lava to move horizontally. Higher values = less chance.
    public static int LAVA_MELTING_CHANCE = 10; // Chance for lava to melt water and sand. Higher values = less chance.
    public static int LAVA_DISPERSAL_RATE = 1; // How many additional neighboring cells to check for horizontal movement.
    public static int LAVA_FIRE_CREATION_CHANCE = 40; // Chance for lava to create fire above it. Higher values = less chance.

    public static int SMOKE_LIFETIME = 1000; // Lifetime of the smoke before it disappears. Higher values = longer lifetime.
    public static int SMOKE_DELETION_FACTOR = 100; // Chance for smoke to be deleted once its lifetime is over. Higher values = less chance.

    public static int FIRE_LIFETIME = 30; // Lifetime of fire before it disappears. Higher values = longer lifetime.
    public static int FIRE_DELETION_FACTOR = 8; // Chance for fire to be deleted once its lifetime is over. Higher values = less chance.
    public static int FIRE_SMOKE_CREATION_FACTOR = 100; // Chance for fire to create smoke. Higher values = less chance.

    public static int WOOD_DELETION_FACTOR = 300; // Chance for wood to be deleted when on fire. Higher values = less chance.
    public static int WOOD_FIRE_SPAWN_FACTOR = 10; // Chance for wood to spawn fire at neighboring cells when on fire. Higher values = less chance

    public static int ACID_DELETION_FACOR = 70; // Chance for acid to delete other cells. Higher value = less chance. (4)
    public static int ACID_REMOVAL_FACTOR = 6; // Chance for acid to despawn upon deleting another cell. Higher value = less chance. (10)
    public static int ACID_DISSOLVE_FACTOR = 5000; // Chance for acid to dissolve in water. Higher values = less chance.
    public static int ACID_WATER_DISSOLVE_FACTOR = 1000; // Chance for acid to dissolve water cells. Higher values = less chance.
    public static int ACID_FOG_SPAWN_FACTOR = 1000; // Chance for acid to spawn poison fog. Higher value = less chance.
    
    public static int POISON_FOG_ACID_RAIN_CHANCE = 30000; // Chance for poison fog to turn back into acid. Higher values = less chance.

    public static int TORNADO_FLOW_FACTOR = 25; // How often the tornado will swap cells with other tornado cells. Higher values = less often.
    public static int TORNADO_GROUNDSEARCH_FACTOR = 20; // How often a tornado will attempt to search for the ground. Higher values = less chance.
    public static int TORNADO_ALTITUDE_FACTOR = 40; // How often a tornado will attempt to raise its altitude. Higher values = less chance.
    public static int TORNADO_DISPERSAL_FACTOR = 800; // How often a tornado will attempt to search horizontally for neighboring NON-TORNADO cells. Higher values = less chance
    public static int TORNADO_DESTRUCTION_FACTOR = 10; // How often a tornado will convert cells to steam. Higher values = less chance.
    public static int TORNADO_DESTRUCTION_RANGE = 10; // The range at which a tornado can convert cells under it into steam. Higher values = MORE range.
    public static int TORNADO_LIFETIME = 600; // The lifetime of a tornado in frames that it will be active. 60 frames = 1 second... hopefully
    public static int TORNADO_DELETION_FACTOR = 80; // The chance for a tornado to be deleted upon the end of its lifetime. Higher values = less chance.



    public static void InitCellStats()
    {
        CellTexture = CreateCellTexture();

        WaterColors = new Dictionary<int, Color>()
        {
            { 0, new Color(0, 0, 255) },
            { 1, new Color(0, 25, 255) },
            { 2, new Color(0, 50, 255) },
            { 3, new Color(0, 75, 255) }
        };
        SandColors = new Dictionary<int, Color>()
        {
            { 0, new Color(200, 150, 75) },
            { 1, new Color(220, 170, 80) },
            { 2, new Color(190, 180, 60) },
            { 3, new Color(230, 210, 90) }
        };
        StoneColors = new Dictionary<int, Color>()
        {
            { 0, new Color(90, 95, 100) },
            { 1, new Color(95, 90, 100) },
            { 2, new Color(95, 95, 100) },
            { 3, new Color(90, 90, 100) }
        };
        SteamColors = new Dictionary<int, Color>()
        {
            { 0, new Color(200, 200, 205) },
            { 1, new Color(200, 205, 210) },
            { 2, new Color(200, 205, 205) },
            { 3, new Color(200, 200, 210) }
        };
        LavaColors = new Dictionary<int, Color>()
        {
            { 0, new Color(170, 20, 25) },
            { 1, new Color(200, 80, 35) },
            { 2, new Color(190, 90, 25) },
            { 3, new Color(220, 150, 35) }
        };
        SmokeColors = new Dictionary<int, Color>()
        {
            { 0, new Color(50, 50, 50) },
            { 1, new Color(60, 50, 60) },
            { 2, new Color(50, 50, 60) },
            { 3, new Color(60, 50, 50) }
        };
        FireColors = new Dictionary<int, Color>()
        {
            { 0, new Color(240, 120, 80) },
            { 1, new Color(240, 160, 90) },
            { 2, new Color(250, 200, 90) },
            { 3, new Color(250, 250, 90) }
        };
        WoodColors = new Dictionary<int, Color>()
        {
            { 0, new Color(90, 80, 70) },
            { 1, new Color(100, 90, 80) },
            { 2, new Color(110, 100, 80) },
            { 3, new Color(90, 100, 80) }
        };
        AcidColors = new Dictionary<int, Color>()
        {
            { 0, new Color(70, 160, 100) },
            { 1, new Color(80, 170, 110) },
            { 2, new Color(70, 190, 100) },
            { 3, new Color(80, 180, 100) }
        };
        TornadoColors = new Dictionary<int, Color>()
        {
            { 0, new Color(180, 180, 185) },
            { 1, new Color(180, 185, 190) },
            { 2, new Color(180, 185, 185) },
            { 3, new Color(180, 180, 190) }
        };
    }

    /// <summary>
    /// Creates the base texture for all cells. A 1x1 white pixel
    /// </summary>
    public static Texture2D CreateCellTexture()
    {
        Texture2D texture = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
        Color[] color_data = new Color[1];
        color_data[0] = Color.White;
        texture.SetData(color_data);
        return texture;
    }
}