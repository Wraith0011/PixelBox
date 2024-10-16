using System;
using System.Data;
using Microsoft.Xna.Framework;

namespace PixelBox;

public class Tornado : Cell
{
    private World game_world;
    private int lifetime = CellStats.TORNADO_LIFETIME;

    public Tornado(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    public void Update()
    {
        lifetime--;
        if (lifetime <= 0)
        {
            game_world.RemoveCell(this);
        }

        bool left_bias = GameCore.Random.Next(0, 2) == 0;
        bool should_swap = GameCore.Random.Next(0, CellStats.TORNADO_FLOW_FACTR) == 0;
        bool should_search_for_ground = GameCore.Random.Next(0, CellStats.TORNADO_GROUNDSEARCH_FACTOR) == 0;
        bool should_raise_altitude = GameCore.Random.Next(0, CellStats.TORNADO_ALTITUDE_FACTOR) == 0;
        bool should_disperse = GameCore.Random.Next(0, CellStats.TORNADO_DISPERSAL_FACTOR) == 0;
        bool should_convert = GameCore.Random.Next(0, CellStats.TORNADO_DESTRUCTION_FACTOR) == 0;

        Vector2 ground_position;
        // Cell neighbor_left = game_world.GetCell( new Vector2(Position.X - 1, Position.Y) );
        // Cell neighbor_right = game_world.GetCell( new Vector2(Position.X + 1, Position.Y) );
        
        Cell immediate_neighbor_left = game_world.GetCell( new Vector2(Position.X - 1, Position.Y) );
        Cell immediate_neighbor_right = game_world.GetCell( new Vector2(Position.X + 1, Position.Y) );
        Cell neighbor_left;
        Cell neighbor_right;
        
        // Find the ground
        if (should_search_for_ground == true && should_disperse == false && should_raise_altitude == false)
        {
            for (int y_position = (int)Position.Y; y_position < game_world.WorldCanvasSize.Y; y_position++)
            {
                ground_position = new Vector2(Position.X, y_position);
                Cell cell = game_world.GetCell(ground_position);
                // If a cell is found below
                if (cell != null)
                {    
                    // Move towards the ground
                    if (Position.Y < ground_position.Y - 5)
                    {
                        if (cell is Tornado && should_swap)
                        {
                            // Swap with neighboring cells on occasion
                            game_world.SwapCell(this, cell);
                            return;
                        }
                        // Convert cells within a certain range of the bottom of the tornado to Steam
                        else if (cell != null && cell is not Tornado && cell is not Steam && (cell.Position.Y - Position.Y) < CellStats.TORNADO_DESTRUCTION_RANGE  && should_convert == true)
                        {
                            game_world.SwapCell(this, cell);
                            if (cell is not Water && cell is not Steam)
                            {
                                game_world.AddCell( new Steam(cell.Position, game_world) );
                                return;
                            }
                        }
                        else if(cell is not Steam)
                        {
                            game_world.TryMoveCell( this, new Vector2(Position.X, Position.Y + 1) );
                            return;
                        }
                    }
                }
            }
        }

        // Search for neighboring tornado cells and move towards them
        // Right
        if (left_bias == false && should_disperse == true)
        {
            for (int x_position = (int)Position.X; x_position < game_world.WorldCanvasSize.X; x_position++)
            {
                neighbor_right = game_world.GetCell( new Vector2(x_position, Position.Y) );
                if (neighbor_right is Tornado)
                {
                    game_world.TryMoveCell( this, new Vector2(Position.X + 1, neighbor_right.Position.Y) );
                    return;
                }
            }
        }

        // Left
        if (left_bias == true && should_disperse == true)
        {
            for (int x_position = (int)Position.X; x_position > 0; x_position--)
            {
                neighbor_left = game_world.GetCell( new Vector2(x_position, Position.Y) );
                if (neighbor_left is Tornado)
                {
                    game_world.TryMoveCell( this, new Vector2(Position.X - 1, neighbor_left.Position.Y) );
                    return;
                }
            }
        }

        // Raise altitude
        if (should_raise_altitude == true)
        {
            game_world.TryMoveCell( this, new Vector2(Position.X, Position.Y - 1) );

            Cell neighbor_above = game_world.GetCell( new Vector2(Position.X, Position.Y - 1) );
            if (neighbor_above is not Tornado && neighbor_above  != null)
            {
                game_world.SwapCell(this, neighbor_above);
            }
        }

    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.TornadoColors.Count);
        return CellStats.TornadoColors[random_index];
    }
}