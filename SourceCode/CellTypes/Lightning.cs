using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PixelBox;
using WraithLib;

public class Lightning: Cell
{
    private World game_world;
    private bool left_bias;
    private bool right_bias;
    private int lifetime = CellStats.LIGHTNING_LIFETIME;
    private bool ground_search;
    private bool should_spawn;
    private int branch_offset;
    private bool offset_left_bias;
    private bool should_destroy_cells;


    public Lightning(Vector2 position, World world) : base(position)
    {
        CellColor = ChooseRandomColor();
        world.TryAddCell(this);
        game_world = world;
    }

    public void Update()
    {
        lifetime--;
        CellColor = ChooseRandomColor();
        left_bias = GameCore.Random.Next(0, CellStats.LIGHTNING_BRANCH_CHANCE) == 0;
        right_bias = GameCore.Random.Next(0, CellStats.LIGHTNING_BRANCH_CHANCE) == 0;
        
        ground_search = GameCore.Random.Next(0, CellStats.LIGHTNING_GROUNDSEARCH_FACTOR) == 0;
        should_spawn = GameCore.Random.Next(0, CellStats.LIGHTNING_SPAWNCHANCE) == 0;
        should_destroy_cells = GameCore.Random.Next(0, CellStats.LIGHTNING_DESTRUCTION_FACTOR) == 0;

        branch_offset = GameCore.Random.Next(0, 3);
        offset_left_bias = GameCore.Random.Next(0, 2) == 0;

        if (lifetime <= 0) { game_world.RemoveCell(this); }

        // Search for the ground
        if (ground_search == true /*&& left_bias == false && right_bias == false && ground_found == false*/)
        {
            for (int y_position = (int)Position.Y + 1; y_position < game_world.WorldCanvasSize.Y; y_position++)
            {
                // Get each cell along the way
                Cell cell = game_world.GetCell( new Vector2(Position.X, y_position) );
                if (cell is Steam)
                {
                    break;
                }
                if (cell is not Lightning && should_destroy_cells == false)
                {
                    break;
                }
                // Add cells along the way
                game_world.AddCell(new Lightning(new Vector2(Position.X, y_position), game_world));

                // Check and see if cell should be destroyed 
                if (cell != null && cell is not Lightning && should_destroy_cells == true)
                {
                    if (cell is Water || cell is Steam)
                    {
                        game_world.AddCell( new Steam(new Vector2(Position.X, y_position), game_world) );
                    }
                    else
                    {
                        game_world.AddCell( new Lightning(new Vector2(Position.X, y_position), game_world) );
                    }

                    // Lightning strike sound effect
                    bool should_play_sound = GameCore.Random.Next(0, 10) == 0;
                    if (should_play_sound == true)
                    {
                        SoundEffectInstance LightningSoundInstance = SoundManager.LightningSound.CreateInstance();
                        LightningSoundInstance.Volume = 0.6f;
                        SoundManager.ActiveLightningSounds.Add(LightningSoundInstance);
                        if (SoundManager.ActiveLightningSounds.Count < SoundManager.MAX_LIGHTNING_SOUNDS)
                        {
                            LightningSoundInstance.Play();
                        }
                    }
                    break;
                }

                // Handle Offsetting while traveling downwards
                // Right 
                if (should_spawn == true && offset_left_bias == false)
                {
                    if (cell == null)
                    {
                        Lightning lightning = new Lightning( new Vector2(Position.X + branch_offset, y_position), game_world );
                        game_world.AddCell(lightning);
                        lightning.ground_search = false;
                    }
                    else if (cell != null && cell is not Lightning && cell is not Steam && should_destroy_cells == true)
                    {
                        Lightning lightning = new Lightning( new Vector2(Position.X + branch_offset, y_position), game_world );
                        game_world.AddCell(lightning);
                        lightning.ground_search = false;
                    }
                }
                // Left
                else if (should_spawn == true && offset_left_bias == true)
                {
                    if (cell == null)
                    {
                        Lightning lightning = new Lightning( new Vector2(Position.X + branch_offset, y_position), game_world );
                        game_world.AddCell(lightning);
                        lightning.ground_search = false;
                    }
                    else if (cell != null && cell is not Lightning && cell is not Steam && should_destroy_cells == true)
                    {
                        Lightning lightning = new Lightning( new Vector2(Position.X + branch_offset, y_position), game_world );
                        game_world.AddCell(lightning);
                    }
                }
            }
            return;
        }

        // Handle branching horizontally
        // Left
        if (left_bias == true && right_bias == false)
        {
            for (int x = 1; x < CellStats.LIGHTNING_RANGE; x++)
            {
                if (should_destroy_cells == true)
                {
                    game_world.AddCell(new Lightning( new Vector2(Position.X - x, Position.Y + branch_offset), game_world ) );
                }
                else if (should_destroy_cells == false)
                {
                    game_world.TryAddCell(new Lightning( new Vector2(Position.X - x, Position.Y + branch_offset), game_world ) );
                }
            }
            left_bias = false;
            return;
        }

        // Right
        if (right_bias == true && left_bias == false)
        {
            for (int x = 1; x < CellStats.LIGHTNING_RANGE; x++)
            {
                if (should_destroy_cells == true)
                {
                   game_world.AddCell(new Lightning( new Vector2(Position.X + x, Position.Y + branch_offset), game_world ) );
                }
                else if (should_destroy_cells == false)
                {
                    game_world.TryAddCell(new Lightning( new Vector2(Position.X + x, Position.Y + branch_offset), game_world ) );
                }
            }
            right_bias = false;
            return;
        }

    }

    private Color ChooseRandomColor()
    {
        int random_index = GameCore.Random.Next(0, CellStats.LightningColors.Count);
        return CellStats.LightningColors[random_index];
    }
}