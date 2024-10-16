using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public class Stone : Cell
{
    private World game_world;

    public Stone(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.StoneColors.Count);
        return CellStats.StoneColors[random_index];
    }

    public void Update()
    {
        
    }
}