using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public class Sand : Cell
{
    private World game_world;

    public Sand(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.SandColors.Count);
        return CellStats.SandColors[random_index];
    }
}