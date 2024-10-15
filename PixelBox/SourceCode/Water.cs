using Microsoft.Xna.Framework;
namespace PixelBox;

public class Water : Cell
{   
    private World game_world;

    public Water(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.WaterColors.Count);
        return CellStats.WaterColors[random_index];
    }

    public void Update()
    { 
        bool left_bias = GameCore.Random.Next(0, 2) == 0; // random number will be 0 or 1, representing left or right directional bias
        bool should_flow = GameCore.Random.Next(0, CellStats.WATER_FLOW_FACTOR) == 0; // should the water attempt to flow this frame
        bool should_erode = GameCore.Random.Next(0, CellStats.STONE_EROSION_CHANCE) == 0;
        bool should_displace = GameCore.Random.Next(0, CellStats.SAND_DISPLACEMENT_CHANCE) == 0;
        bool should_convert = GameCore.Random.Next(0, CellStats.WATER_CONVERSION_CHANCE) == 0;

        // Water to steam conversion
        if (should_convert == true)
        {
            Vector2 position = Position;
            game_world.AddCell( new Steam(position, game_world) );
        }

        // Below Movement
        Vector2 potential_position = new Vector2(Position.X, Position.Y +1);
        Cell neighbor = game_world.GetCell(potential_position);

        if ( potential_position.Y < game_world.WorldCanvasSize.Y && (neighbor == null  || neighbor is Water) )
        {   
            if (neighbor is Water && should_flow)
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

        // Left Movement
        for (int i = 1; i <= CellStats.WATER_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X -i, Position.Y);
            neighbor = game_world.GetCell(potential_position);
            Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y +1) );

            if (neighbor != null && neighbor is not Water)
            {
                break;
            }
            
            if ( left_bias == true && potential_position.X >= 0 && (neighbor == null || neighbor is Water) )
            { 
                if (neighbor is Water && should_flow)
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

            // Stone Erosion
            if ( neighbor_below is Stone && should_erode == true)
            {
                Vector2 position = neighbor_below.Position;
                game_world.RemoveCell(neighbor_below);
                game_world.AddCell(new Sand(position, game_world));
            }

            // Sand Displacement
            if (neighbor_below is Sand && should_displace == true)
            {
                game_world.SwapCell(this, neighbor_below);
            }
        }

        // Right Movement
        for (int i = 1; i <= CellStats.WATER_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X +i, Position.Y);
            neighbor = game_world.GetCell(potential_position);
            Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y +1) );

            if (neighbor != null && neighbor is not Water)
            {
                break;
            }
            
            if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor == null  || neighbor is Water) ) 
            {   
                if ( neighbor is Water && should_flow)
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

            // Stone Erosion
            if ( neighbor_below is Stone && should_erode == true)
            {
                Vector2 position = neighbor_below.Position;
                game_world.RemoveCell(neighbor_below);
                game_world.AddCell(new Sand(position, game_world));
            }

            // Sand Displacement
            if (neighbor_below is Sand && should_displace == true)
            {
                game_world.SwapCell(this, neighbor_below);
            }
        }

    }

}