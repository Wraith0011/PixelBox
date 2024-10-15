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
    
    public static int WATER_FLOW_FACTOR = 150; // How often water flows into other water. Lower values = more flow.
    public static int WATER_DISPERSAL_RATE = 3; // How many additional neighboring cells to check for horizontal movement.
    public static int WATER_CONVERSION_CHANCE = 50000; // The chance that water will turn into steam. Higher values = less chance.
    public static int STONE_EROSION_CHANCE = 1000; // How often water will erode stone. Higher values = less chance.
    public static int SAND_DISPLACEMENT_CHANCE = 25; // How often sand will be displaced by water. Higher values = less chance.
    public static int SAND_COMPRESSION_FACTOR = 20; // How many sand cells have to be on top of each other in order to compress into stone.
    public static int STEAM_DISPERSAL_RATE = 2; // How many additional neighboring cells to check for horizontal movement.
    public static int STEAM_DISPERSAL_CHANCE = 5; // The chance that steam will seek to flow horizontally to empty spaces. Higher values = less chance.
    public static int STEAM_FLOW_FACTOR = 25; // How often steam flows into other steam. Lower values = more flow.
    public static int STEAM_CONVERSION_CHANCE = 25000; // The chance that steam will turn into water. Higher values = less chance.

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