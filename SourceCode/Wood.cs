using Microsoft.Xna.Framework;

namespace PixelBox;

public class Wood : Cell
{
    private World game_world;
    public bool IsOnFire {get; private set;}
    private bool should_delete = false;
    private bool should_spawn_fire;

    public Wood(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    public void Update()
    {
        Cell neighbor_above = game_world.GetCell( new Vector2(Position.X, Position.Y - 1) );
        Cell neighbor_below = game_world.GetCell( new Vector2(Position.X, Position.Y + 1) );
        Cell neighbor_left  = game_world.GetCell( new Vector2(Position.X - 1, Position.Y) );
        Cell neighbor_right = game_world.GetCell( new Vector2(Position.X + 1, Position.Y) );

        // Check for fire
        if (IsOnFire == false)
        {
            if (neighbor_left is Fire || neighbor_left is Lava)
            {
                IsOnFire = true;
            }
            else if (neighbor_right is Fire || neighbor_right is Lava)
            {            
                IsOnFire = true;
            }
            else if (neighbor_below is Fire || neighbor_below is Lava)
            {
                IsOnFire = true;
            }
            else if (neighbor_above is Fire || neighbor_above is Lava)
            {
                IsOnFire = true;
            }
        }

        // Check for water
        if (IsOnFire == true)
        {
            if (neighbor_left is Water)
            {
                if (neighbor_left is Water) { IsOnFire = false; }
            }
            else if (neighbor_right is Water)
            {            
                if (neighbor_right is Water) { IsOnFire = false; }
            }
            else if (neighbor_below is Water)
            {
                if (neighbor_below is Water) { IsOnFire = false; }
            }
            else if (neighbor_above is Water)
            {
                if (neighbor_above is Water) { IsOnFire = false; }
            }
        }

        // Spawn fire when on fire, and delete
        if (IsOnFire == true)
        {
            should_spawn_fire = GameCore.Random.Next(0, CellStats.WOOD_FIRE_SPAWN_FACTOR) == 0;
            if (should_spawn_fire == true)
            {
                game_world.TryAddCell( new Fire(new Vector2(Position.X, Position.Y - 1), game_world) );
                game_world.TryAddCell( new Fire(new Vector2(Position.X, Position.Y + 1), game_world) );
                game_world.TryAddCell( new Fire(new Vector2(Position.X - 1, Position.Y), game_world) );
                game_world.TryAddCell( new Fire(new Vector2(Position.X + 1, Position.Y), game_world) );
            }
            should_delete = GameCore.Random.Next(0, CellStats.WOOD_DELETION_FACTOR) == 0;
        }

        // Delete when on fire
        if (IsOnFire == true && should_delete == true)
        {
            game_world.RemoveCell(this);
        }
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.WoodColors.Count);
        return CellStats.WoodColors[random_index];
    }
}