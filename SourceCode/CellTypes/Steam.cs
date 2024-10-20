using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public class Steam : Cell
{
    World game_world;
    public bool ShouldConvert {get; set;} = false;
    public bool ShouldSpawnLightning {get; set;} = false;

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
        bool should_spawn_lightning = GameCore.Random.Next(0, CellStats.STEAM_SPAWN_LIGHTNING_CHANCE) == 0;

        // Steam to water conversion
        if (should_convert == true && this is not Smoke && this is not Fire && WeatherCycle.IsRaining == true)
        {
            Vector2 position = Position;
            game_world.AddCell( new Water(position, game_world) );
        }

        // Steam to lightning conversion
        Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y + 1) );
        if (should_spawn_lightning && this is not Smoke && this is not Fire && WeatherCycle.IsRaining == true && neighbor_below == null)
        {
            Vector2 position = Position;
            game_world.AddCell( new Lightning(new Vector2(Position.X, Position.Y + 1), game_world) );
        }

        // Up movement
        Vector2 potential_position = new Vector2(Position.X, Position.Y -1);
        Cell neighbor_above = game_world.GetCell(potential_position);

        if ( potential_position.Y >= 0 && (neighbor_above == null || neighbor_above is Water || neighbor_above is Steam || neighbor_above is Tornado) )
        {
            if (neighbor_above == null)
            {
                game_world.MoveCell(this, potential_position);
            }
            if (neighbor_above is Water || neighbor_above is Lava || neighbor_above is Tornado)
            {
                game_world.SwapCell(this, neighbor_above);
            }
            else if (neighbor_above is Steam && should_flow == true)
            {
                game_world.SwapCell(this, neighbor_above);
            }
        }

        // Left movement
        for (int i = 1; i <= CellStats.STEAM_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X -i, Position.Y);
            Cell neighbor_left = game_world.GetCell(potential_position);

            if (neighbor_left != null && neighbor_left is not Steam && neighbor_left is not Water && neighbor_left is not Tornado)
            {
                break;
            }

            if ( left_bias == true && potential_position.X >= 0 && (neighbor_left == null || neighbor_left is Steam || neighbor_left is Water || neighbor_left is Tornado) )
            {
                if (neighbor_left == null && should_disperse == true)
                {           
                    game_world.MoveCell(this, potential_position);
                    return;
                }
                else if (neighbor_left is Steam && should_flow == true)
                {
                    game_world.SwapCell(this, neighbor_left);
                    return;
                }
                else if (neighbor_left is Water || neighbor_left is Tornado)
                {
                    game_world.SwapCell(this, neighbor_left);
                    return;
                }
            }
        }

        // Right
        for (int i = 1; i <= CellStats.STEAM_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X + i, Position.Y);
            Cell neighbor_right = game_world.GetCell(potential_position);

            if (neighbor_right != null && neighbor_right is not Steam && neighbor_right is not Water && neighbor_right is not Tornado)
            {
                break;
            }
            if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor_right == null || neighbor_right is Steam || neighbor_right is Water || neighbor_right is Tornado) )
            {
                if (neighbor_right == null && should_disperse == true)
                {
                    game_world.MoveCell(this, potential_position);
                    return;
                }
                else if (neighbor_right is Steam && should_flow == true)
                {
                    game_world.SwapCell(this, neighbor_right);
                    return;
                }
                else if (neighbor_right is Water || neighbor_right is Tornado)
                {
                    game_world.SwapCell(this, neighbor_right);
                    return;
                }
            }
        }

    }
}