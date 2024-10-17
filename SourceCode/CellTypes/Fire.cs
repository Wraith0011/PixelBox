using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public class Fire : Steam
{

    private World game_world;
    private int lifetime = CellStats.FIRE_LIFETIME;
    private bool should_delete = false;
    private bool should_spawn_smoke = false;

    public Fire(Vector2 position, World world) : base(position, world)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.FireColors.Count);
        return CellStats.FireColors[random_index];
    }

    public override void Update()
    {
        base.Update();
        Cell neighbor_above = game_world.GetCell( new Vector2(Position.X, Position.Y - 1) );
        Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y + 1) );
        Cell neighbor_left  = game_world.GetCell( new Vector2(Position.X - 1, Position.Y) );
        Cell neighbor_right = game_world.GetCell( new Vector2(Position.X + 1, Position.Y) );

        // Spawn smoke
        if (neighbor_above == null)
        {
            should_spawn_smoke = GameCore.Random.Next(0, CellStats.FIRE_SMOKE_CREATION_FACTOR) == 0;
            if (should_spawn_smoke == true)
            {
                game_world.AddCell( new Smoke(new Vector2(Position.X, Position.Y - 1), game_world) );
            }
        }

        // Remove fire
        should_delete = GameCore.Random.Next(0, CellStats.FIRE_DELETION_FACTOR) == 0;
        lifetime--;
        if (lifetime <= 0 && should_delete)
        {
            game_world.RemoveCell(this);
        }

        // Check for water
        if (neighbor_left is Water)
        {
            game_world.RemoveCell(this);
        }
        else if (neighbor_right is Water)
        {
            game_world.RemoveCell(this);
        }
        else if (neighbor_below is Water)
        {
            game_world.RemoveCell(this);
        }
        else if (neighbor_above is Water)
        {
            game_world.RemoveCell(this);
        }

    }
}