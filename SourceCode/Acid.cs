using Microsoft.Xna.Framework;

namespace PixelBox;

public class Acid : Water
{
    private World game_world;
    private bool should_delete;
    private bool should_remove;
    private bool should_dissolve;
    private bool should_water_dissolve;
    private bool should_spawn_fog;

    public Acid(Vector2 position, World world) : base(position, world)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.AcidColors.Count);
        return CellStats.AcidColors[random_index];
    }

    public override void Update()
    {
        base.Update();

        should_delete = GameCore.Random.Next(0, CellStats.ACID_DELETION_FACOR) == 0;
        should_remove = GameCore.Random.Next(0, CellStats.ACID_REMOVAL_FACTOR) == 0;
        should_dissolve = GameCore.Random.Next(0, CellStats.ACID_DISSOLVE_FACTOR) == 0;
        should_water_dissolve = GameCore.Random.Next(0, CellStats.ACID_WATER_DISSOLVE_FACTOR) == 0;
        should_spawn_fog = GameCore.Random.Next(0, CellStats.ACID_FOG_SPAWN_FACTOR) == 0;


        // Spawn poison fog
        if ( should_spawn_fog == true )
        {
            Vector2 position = Position;
            game_world.RemoveCell(this);
            game_world.AddCell( new PoisonFog(position, game_world) );
            return;
        }

        // Above
        // if (neighbor_above != null && neighbor_above is not Water && neighbor_above is not Tornado)
        // {
        //     if (should_delete == true)
        //     {
        //         game_world.RemoveCell(neighbor_above);
        //         if (should_remove == true)
        //         {
        //             game_world.RemoveCell(this);
        //         }
        //     }
        //     return;
        // }
        // else if ( neighbor_above is Water && neighbor_above is not Acid && (should_dissolve == true || should_water_dissolve == true) )
        // {
        //     if (should_dissolve == true)
        //     {
        //         game_world.RemoveCell(this);
        //         return;
        //     }
        //     if (should_water_dissolve == true)
        //     {
        //         game_world.RemoveCell(neighbor_above);
        //         return;
        //     }
        // }

        // Below
        if (neighbor_below != null && neighbor_below is not Water && neighbor_below is not Tornado)
        {
            if (should_delete == true)
            {
                game_world.RemoveCell(neighbor_below);
                if (should_remove == true)
                {
                    game_world.RemoveCell(this);
                }
            }
            return;
        }
        else if ( neighbor_below is Water && neighbor_below is not Acid && (should_dissolve == true || should_water_dissolve == true) )
        {
            if (should_dissolve == true)
            {
                game_world.RemoveCell(this);
                return;
            }
            if (should_water_dissolve == true)
            {
                game_world.RemoveCell(neighbor_below);
                return;
            }
        }

        // Left
        if (neighbor_left != null && neighbor_left is not Water && neighbor_left is not Tornado)
        {
            if (should_delete == true)
            {
                game_world.RemoveCell(neighbor_left);
                if (should_remove == true)
                {
                    game_world.RemoveCell(this);
                }
            }
            return;
        }
        else if ( neighbor_left is Water && neighbor_left is not Acid && (should_dissolve == true || should_water_dissolve == true) )
        {
            if (should_dissolve == true)
            {
                game_world.RemoveCell(this);
                return;
            }
            if (should_water_dissolve == true)
            {
                game_world.RemoveCell(neighbor_left);
                return;
            }
        }

        // Right
        if (neighbor_right != null && neighbor_right is not Water && neighbor_right is not Tornado)
        {
            if (should_delete == true)
            {
                game_world.RemoveCell(neighbor_right);
                if (should_remove == true)
                {
                    game_world.RemoveCell(this);
                }
            }
            return;
        }
        else if ( neighbor_right is Water && neighbor_right is not Acid && (should_dissolve == true || should_water_dissolve == true) )
        {
            if (should_dissolve == true)
            {
                game_world.RemoveCell(this);
                return;
            }
            if (should_water_dissolve == true)
            {
                game_world.RemoveCell(neighbor_right);
                return;
            }
        }
    }
    
}