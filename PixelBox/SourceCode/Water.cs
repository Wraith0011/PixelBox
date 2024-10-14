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

        // Below
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
                // Remove the old position from the world dictionary
                game_world.WorldCells.Remove(Position);
                // Update our current position to the new one 
                Position = potential_position;
                // Update our world dictionary with the new position & cell
                game_world.WorldCells[potential_position] = this; 
                return;
            }
        }

        // Left
        for (int i = 1; i <= CellStats.WATER_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X -i, Position.Y);
            neighbor = game_world.GetCell(potential_position);
            if ( left_bias == true && potential_position.X >= 0 && (neighbor == null || neighbor is Water) )
            { 
                if (neighbor is Water && should_flow)
                {
                    game_world.SwapCell(this, neighbor);
                    return;
                }
                else if (neighbor == null)
                {
                    game_world.WorldCells.Remove(Position);
                    Position = potential_position;
                    game_world.WorldCells.Add(potential_position, this);
                    return;
                }
            }
        }

        // Right
        for (int i = 1; i <= CellStats.WATER_DISPERSAL_RATE; i++)
        {
            potential_position = new Vector2(Position.X +i, Position.Y);
            neighbor = game_world.GetCell(potential_position);
            if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor == null  || neighbor is Water) ) 
            {   
                if ( neighbor is Water && should_flow)
                {
                    game_world.SwapCell(this, neighbor);
                    return;
                }
                else if (neighbor == null)
                {
                    game_world.WorldCells.Remove(Position);
                    Position = potential_position;
                    game_world.WorldCells.Add(potential_position, this);
                    return;
                }
            }
        }
    }

}