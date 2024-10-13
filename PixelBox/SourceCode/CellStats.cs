using System;
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
    
    public static int WATER_FLOW_FACTOR = 50; // How easily water flows. Lower values = more flow.
    public static int WATER_SIMULATION_SPEED = 1; // How many times water cells will update each frame
    public static int WATER_MOVEMENT_SPEED = 8; // How fast the water cells will move left / right

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