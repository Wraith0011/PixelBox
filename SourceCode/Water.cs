using System;
using Microsoft.Xna.Framework;
namespace PixelBox;

public class Water : Cell // 36K limit
{   
    private World game_world;

    public Water(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    // Choose random color upon creation
    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.WaterColors.Count);
        return CellStats.WaterColors[random_index];
    }

    public virtual void Update()
    { 
        bool left_bias = GameCore.Random.Next(0, 2) == 0;
        bool should_flow = GameCore.Random.Next(0, CellStats.WATER_FLOW_FACTOR) == 0;
        bool should_erode = GameCore.Random.Next(0, CellStats.STONE_EROSION_CHANCE) == 0;
        bool should_displace = GameCore.Random.Next(0, CellStats.SAND_DISPLACEMENT_CHANCE) == 0;
        bool should_displace_offset = GameCore.Random.Next (0, 2) == 0;
        bool should_convert = GameCore.Random.Next(0, CellStats.WATER_CONVERSION_CHANCE) == 0;

        Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y + 1) );
        Cell neighbor_left  = game_world.GetCell( new Vector2(Position.X - 1, Position.Y) );
        Cell neighbor_right = game_world.GetCell( new Vector2(Position.X + 1, Position.Y) );
        Vector2 potential_position;

        // Water to steam conversion
        if (should_convert == true && this is not Acid)
        {
            Vector2 position = Position;
            game_world.AddCell( new Steam(position, game_world) );
        }

        // Below Movement
        if ( neighbor_below is Water && should_flow && neighbor_left is not Steam && neighbor_right is not Steam )
        {
            game_world.SwapCell(this, neighbor_below);
            return;
        }
        else if ( neighbor_below == null && Position.Y + 1 < game_world.WorldCanvasSize.Y)
        {
            game_world.MoveCell( this, new Vector2(Position.X, Position.Y + 1) );
            return;
        }
    
        // Stone Erosion
        if ( neighbor_below is Stone && should_erode == true)
        {
            Vector2 position = neighbor_below.Position;
            game_world.RemoveCell(neighbor_below);
            game_world.AddCell(new Sand(position, game_world));
            return;
        }

        // Left Movement
        for (int i = 1; i <= CellStats.WATER_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X -i, Position.Y);
            Cell neighbor = game_world.GetCell(potential_position);

            if (neighbor != null && neighbor is not Water && neighbor is not Steam && neighbor is not Sand)
            {
                break;
            }
            
            if ( left_bias == true && potential_position.X >= 0 && (neighbor == null || neighbor is Water || neighbor is Steam) )
            { 
                if ( should_flow && (neighbor is Water || neighbor is Steam) )
                {
                    game_world.SwapCell(this, neighbor);
                    return;
                }
                else if (neighbor == null)
                {
                    game_world.MoveCell(this, potential_position);
                    return;
                }
            }

            // Sand Displacement
            if (neighbor is Sand && should_displace == true && should_displace_offset == true && i == 1)
            {
                game_world.SwapCell(this, neighbor);
                return;
            }
        }

        // Right Movement
        for (int i = 1; i <= CellStats.WATER_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X +i, Position.Y);
            Cell neighbor = game_world.GetCell(potential_position);

            if (neighbor != null && neighbor is not Water && neighbor is not Steam && neighbor is not Sand)
            {
                break;
            }
            
            if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor == null  || neighbor is Water || neighbor is Steam) ) 
            {   
                if ( should_flow && (neighbor is Water || neighbor is Steam) )
                {
                    game_world.SwapCell(this, neighbor);
                    return;
                }
                else if (neighbor == null)
                {
                    game_world.MoveCell(this, potential_position);
                    return;
                }
            }

            // Sand Displacement
            if (neighbor is Sand && should_displace == true && should_displace_offset == false && i == 1)
            {
                game_world.SwapCell(this, neighbor);
                return;
            }
        }

    }

}