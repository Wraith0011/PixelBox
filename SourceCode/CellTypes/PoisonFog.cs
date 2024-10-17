using Microsoft.Xna.Framework;

namespace PixelBox;

public class PoisonFog : Smoke
{
    private World game_world;
    private bool should_convert;

    public PoisonFog(Vector2 position, World world) : base(position, world)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    public override void Update()
    {
        base.Update();
        
        should_convert = GameCore.Random.Next(0, CellStats.POISON_FOG_ACID_RAIN_CHANCE) == 0;

        if (should_convert == true)
        {
            Vector2 position = Position;
            game_world.RemoveCell(this);
            game_world.AddCell( new Acid(position, game_world) );
            return;
        }
    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.AcidColors.Count);
        return CellStats.AcidColors[random_index];
    }
}