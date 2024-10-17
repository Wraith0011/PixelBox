using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WraithLib;
namespace PixelBox;

public class AwfulUI
{
    private SpriteFont Font;
    private World GameWorld;
    private GameCore game_core;

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
    private Vector2 UI_Position_12 = new Vector2(0, 240);
    private Vector2 UI_Position_13 = new Vector2(0, 280);
    private Vector2 UI_Position_14 = new Vector2(0, 300);
    private Vector2 UI_Position_15 = new Vector2(0, 340);
    private Vector2 UI_Position_16 = new Vector2(0, 360);

    public AwfulUI(GameCore game_core)
    {
        this.game_core = game_core;
        Font = game_core.Font;
        GameWorld = game_core.GameWorld;
    }

    public void Draw()
    {
        Globals.Sprite_Batch.DrawString(Font, "FPS: " + Globals.FPS, UI_Position_0, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Selected: " + game_core.SelectedCellType.ToString(), UI_Position_2, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "" + GameWorld.GetCell(Globals.MousePositionOnCanvas), new Vector2(Globals.MousePosition.X + 20, Globals.MousePosition.Y - 8), UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Brush Size: " + game_core.ScrollValue/2, UI_Position_4, UI_Color);
        
        Globals.Sprite_Batch.DrawString(Font, "Water Cells: " + GameWorld.WaterCells.Count, UI_Position_6, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Sand Cells: " + GameWorld.SandCells.Count, UI_Position_7, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Stone Cells: " + GameWorld.StoneCells.Count, UI_Position_8, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Steam Cells: " + GameWorld.SteamCells.Count, UI_Position_9, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Lava Cells: " + GameWorld.LavaCells.Count, UI_Position_10, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Wood Cells: " + GameWorld.WoodCells.Count, UI_Position_11, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, "Tornado Cells: " + GameWorld.TornadoCells.Count, UI_Position_12, UI_Color);

        Globals.Sprite_Batch.DrawString(Font, "Time: " + game_core.TimeCycle.CurrentTimeOfDay, UI_Position_13, UI_Color);
        Globals.Sprite_Batch.DrawString(Font, game_core.TimeCycle.GetNextTimeOfDay(game_core.TimeCycle.CurrentTimeOfDay) + " in: " + (int)(game_core.TimeCycle.CycleDuration - game_core.TimeCycle.Time)/60, UI_Position_14, UI_Color);

        Globals.Sprite_Batch.DrawString(Font, "Controlling Lag? " + game_core.CONTROLLING_LAG, UI_Position_15, UI_Color);
    }

}