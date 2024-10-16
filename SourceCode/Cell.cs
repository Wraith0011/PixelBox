using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

/// <summary>
/// This is a base class to desribe what the essentials of any cell are.
/// The base Cell class is not meant to be added to the world directly, as it has little properties on its own.
/// Child classes such as Water.cs will extend this class, and are meant to be in the game world.
/// </summary>
public class Cell
{
    public Vector2 Position {get; set;}
    public Color CellColor {get; set;}

    public Cell(Vector2 position)
    {
        Position = position;
        CellColor = Color.White;
    }

    public void Draw()
    {
        Globals.Sprite_Batch.Draw
        (
            CellStats.CellTexture, 
            new Rectangle
            (
                (int)Position.X, 
                (int)Position.Y, 
                CellStats.CellTexture.Width, 
                CellStats.CellTexture.Height
            ), 
            CellColor
        );
    }

}