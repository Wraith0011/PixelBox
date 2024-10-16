using System;
using Microsoft.Xna.Framework;
namespace PixelBox;

public class Smoke : Steam
{
    private World game_world;
    private int lifetime = CellStats.SMOKE_LIFETIME;

    public Smoke(Vector2 position, World world) : base(position, world)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }
    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.SmokeColors.Count);
        return CellStats.SmokeColors[random_index];
    }   

    public override void Update()
    {
        bool should_delete = GameCore.Random.Next(0, CellStats.SMOKE_DELETION_FACTOR) == 0;        
        lifetime--;
        if (lifetime <= 0 && should_delete)
        {
            game_world.RemoveCell(this);
        }
        base.Update();
    }
}