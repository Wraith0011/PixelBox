using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public class Steam : Cell
{
    World game_world;
    public bool ShouldConvert {get; set;} = false;

    public Steam(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.SteamColors.Count);
        return CellStats.SteamColors[random_index];
    }

    public virtual void Update()
    {
        bool left_bias = GameCore.Random.Next(0, 2) == 0;
        bool should_flow = GameCore.Random.Next(0, CellStats.STEAM_FLOW_FACTOR) == 0;
        bool should_convert = GameCore.Random.Next(0, CellStats.STEAM_CONVERSION_CHANCE) == 0;
        bool should_disperse = GameCore.Random.Next(0, CellStats.STEAM_DISPERSAL_CHANCE) == 0;

        // Steam to water conversion
        if (should_convert == true && this is not Smoke)
        {
            Vector2 position = Position;
            game_world.AddCell( new Water(position, game_world) );
        }
        // Up
        Vector2 potential_position = new Vector2(Position.X, Position.Y -1);
        Cell neighbor_above = game_world.GetCell(potential_position);

        if ( potential_position.Y >= 0 && (neighbor_above == null || neighbor_above is Water || neighbor_above is Steam) )
        {
            if (neighbor_above == null)
            {
                game_world.MoveCell(this, potential_position);
            }
            else if (neighbor_above is Water)
            {
                game_world.SwapCell(this, neighbor_above);
            }
            else if (neighbor_above is Steam && should_flow == true)
            {
                game_world.SwapCell(this, neighbor_above);
            }
        }

        // Left
        for (int i = 1; i <= CellStats.STEAM_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X -1, Position.Y);
            Cell neighbor_left = game_world.GetCell(potential_position);

            if (neighbor_left != null && neighbor_left is not Steam && neighbor_left is not Water)
            {
                break;
            }

            if ( left_bias == true && potential_position.X >= 0 && (neighbor_left == null || neighbor_left is Steam || neighbor_left is Water) )
            {
                if (neighbor_left == null && should_disperse == true)
                {
                    game_world.MoveCell(this, potential_position);
                }
                else if (neighbor_left is Steam && should_flow == true)
                {
                    game_world.SwapCell(this, neighbor_left);
                }
                else if (neighbor_left is Water)
                {
                    game_world.SwapCell(this, neighbor_left);
                }
            }
        }

        // Right
        for (int i = 1; i <= CellStats.STEAM_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X +1, Position.Y);
            Cell neighbor_right = game_world.GetCell(potential_position);

            if (neighbor_right != null && neighbor_right is not Steam && neighbor_right is not Water)
            {
                break;
            }
            if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor_right == null || neighbor_right is Steam || neighbor_right is Water) )
            {
                if (neighbor_right == null && should_disperse == true)
                {
                    game_world.MoveCell(this, potential_position);
                }
                else if (neighbor_right is Steam && should_flow == true)
                {
                    game_world.SwapCell(this, neighbor_right);
                }
                else if (neighbor_right is Water)
                {
                    game_world.SwapCell(this, neighbor_right);
                }
            }
        }

    }
}