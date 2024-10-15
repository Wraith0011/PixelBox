using System;
using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public class Sand : Cell
{

    private World game_world;

    public bool IsFalling {get; private set;} = false;

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

    public void Update()
    {
        bool left_bias = GameCore.Random.Next(0, 2) == 0;

        // Below
        Vector2 potential_position = new Vector2(Position.X, Position.Y +1);
        Cell neighbor_below = game_world.GetCell(potential_position);

        if ( potential_position.Y < game_world.WorldCanvasSize.Y && (neighbor_below == null || neighbor_below is Water || neighbor_below is Steam) )
        {
            if (neighbor_below is Water || neighbor_below is Steam)
            {
                IsFalling = true;
                game_world.SwapCell(this, neighbor_below);
                return;
            }
            else
            {
                IsFalling = true;
                game_world.MoveCell(this, potential_position);
                return;
            }
        }
        else if ( potential_position.Y >= game_world.WorldCanvasSize.Y || neighbor_below != null)
        {
            IsFalling = false;
        }
        else { IsFalling = false; }

        // Left
        potential_position = new Vector2 (Position.X -1, Position.Y);
        Cell neighbor_left = game_world.GetCell(potential_position);
        Cell neighbor_above = game_world.GetCell( new Vector2(Position.X, Position.Y -1) );

        if ( left_bias == true && potential_position.X >= 0 && (neighbor_left == null) )
        {
            if (neighbor_above is Sand)
            {
                game_world.MoveCell(this, potential_position);
                return;
            }
        }
        if (neighbor_above is Sand && neighbor_left is Water)
        {
            game_world.SwapCell(this, neighbor_left);
            return;
        }

        // Right
        potential_position = new Vector2 (Position.X +1, Position.Y);
        Cell neighbor_right = game_world.GetCell(potential_position);
        neighbor_above = game_world.GetCell( new Vector2(Position.X, Position.Y -1) );
        
        if ( left_bias == false && potential_position.X < game_world.WorldCanvasSize.X && (neighbor_right == null) )
        {
            if (neighbor_above is Sand)
            {
                game_world.MoveCell(this, potential_position);
                return;
            }
        }

        if (neighbor_above is Sand && neighbor_right is Water)
        {
            game_world.SwapCell(this, neighbor_right);
            return;
        }

        // Compression (Into Stone)
        int sand_above_count = 0;
        for (int i = 1; i <= CellStats.SAND_COMPRESSION_FACTOR; i++)
        {
            potential_position = new Vector2(Position.X, Position.Y - i);
            neighbor_above = game_world.GetCell(potential_position);

            if (neighbor_above is Sand  || neighbor_above is Stone && IsFalling == false)
            {
                sand_above_count++;
            }
            if ( neighbor_above is Sand sand && sand.IsFalling == false && (sand_above_count == CellStats.SAND_COMPRESSION_FACTOR) )
            {
                Vector2 position = Position;
                game_world.RemoveCell(this);
                game_world.AddCell(new Stone(position, game_world) );
                return;
            }
        }
        if (neighbor_above is Stone && neighbor_left is Stone && neighbor_right is Stone && neighbor_below is Stone || neighbor_above is Stone && neighbor_left is Stone && neighbor_right is Stone && neighbor_below == null)
        {
                Vector2 position = Position;
                game_world.RemoveCell(this);
                game_world.AddCell(new Stone(position, game_world) );
                return;
        }

    }
}