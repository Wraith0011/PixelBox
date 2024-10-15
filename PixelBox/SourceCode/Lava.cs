using System;
using Microsoft.Xna.Framework;
namespace PixelBox;

public class Lava : Cell
{
    private World game_world;
    public Lava(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        Position = position;
        game_world = world;
    }

    // Choose random color upon creation
    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.LavaColors.Count);
        return CellStats.LavaColors[random_index];
    }

    public void Update()
    {
        bool left_bias = GameCore.Random.Next(0, 2) == 0;
        bool should_flow = GameCore.Random.Next(0, CellStats.LAVA_FLOW_FACTOR) == 0;
        bool should_disperse = GameCore.Random.Next(0, CellStats.LAVA_DISPERSAL_CHANCE) == 0;
        bool should_smoke_create = GameCore.Random.Next(0, CellStats.SMOKE_CREATION_CHANCE) == 0;
        
        Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y + 1) );
        Cell neighbor_above = game_world.GetCell( new Vector2(Position.X, Position.Y - 1) );
        Vector2 potential_position;

        // Create smoke
        if ( should_smoke_create == true && (neighbor_above == null || neighbor_above is Steam) )
        {
            if (neighbor_above == null)
            {
                game_world.TryAddCell( new Smoke(new Vector2(Position.X, Position.Y - 1), game_world) );
            }
            else
            {
                Vector2 position = neighbor_above.Position;
                game_world.AddCell( new Smoke(position, game_world) );
            }
        }

        // Evaporate water, Melt sand
        for (int y = (int)Position.Y - 1; y <= Position.Y + 1; y++)
        {
            for (int x = (int)Position.X - 1; x <= Position.X + 1; x++)
            {
                Vector2 position = new Vector2(x, y);
                Cell cell = game_world.GetCell(position);
                if (cell is Water)
                {
                    game_world.AddCell( new Steam(position, game_world) );
                }
                if (cell is Sand)
                {
                    game_world.AddCell( new Lava(position, game_world) );
                }
            }
        }
        
        // Below Movement
        potential_position = new Vector2(Position.X, Position.Y +1);

        if ( potential_position.Y < game_world.WorldCanvasSize.Y && (neighbor_below == null  || neighbor_below is Lava || neighbor_below is Steam) )
        {   
            if ( (should_flow && neighbor_below is Lava) || neighbor_below is Steam )
            {
                game_world.SwapCell(this, neighbor_below);
                return;
            }
            else if (neighbor_below == null)
            {
                game_world.MoveCell(this, potential_position); 
                return;
            }
        }

        // Left
        if (should_disperse == true)
        {
            for (int i = 1; i <= CellStats.LAVA_DISPERSAL_RATE; i++)
            {
                potential_position = new Vector2(Position.X -i, Position.Y);
                Cell neighbor = game_world.GetCell(potential_position);

                if (neighbor != null && neighbor is not Lava && neighbor is not Steam)
                {
                    break;
                }
                
                if ( left_bias == true && potential_position.X >= 0 && (neighbor == null || neighbor is Lava || neighbor is Steam) )
                { 
                    if ( should_flow && (neighbor is Lava || neighbor is Steam) )
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
            }
        }

        // Right Movement
        if (should_disperse == true)
        {
            for (int i = 1; i <= CellStats.LAVA_DISPERSAL_RATE; i++)
            {
                potential_position = new Vector2(Position.X +i, Position.Y);
                Cell neighbor = game_world.GetCell(potential_position);

                if (neighbor != null && neighbor is not Lava && neighbor is not Steam)
                {
                    break;
                }
                
                if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor == null  || neighbor is Lava) ) 
                {   
                    if ( should_flow && (neighbor is Lava || neighbor is Steam) )
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
            }
        }

    }

}