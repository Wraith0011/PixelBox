using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Audio;
using WraithLib;
namespace PixelBox;

public static class SoundManager
{
    public static SoundEffect IntroTheme {get; private set;}
    public static SoundEffectInstance IntroThemeInstance;

    public static SoundEffect LavaSound_0;
    public static SoundEffectInstance LavaSound_0_Instance;
    public static Timer LavaSoundTimer;
    public static bool LavaSoundShouldPlay;

    public static SoundEffect TornadoSound_0;
    public static SoundEffectInstance TornadoSound_0_Instance;
    public static Timer TornadoSoundTimer;
    public static bool TornadoSoundShouldPlay;

    public static SoundEffect LightningSound;
    public static List<SoundEffectInstance> ActiveLightningSounds;
    public static int MAX_LIGHTNING_SOUNDS = 2;

    public static void LoadContent(GameCore game_core)
    {
        LavaSoundTimer = new Timer();
        TornadoSoundTimer = new Timer();

        IntroTheme = game_core.Content.Load<SoundEffect>("Retro Music Loop - PV8 - NES Style 01");

        LavaSound_0 = game_core.Content.Load<SoundEffect>("Retro Explosion Short 01");
        LavaSound_0_Instance = LavaSound_0.CreateInstance();

        TornadoSound_0 = game_core.Content.Load<SoundEffect>("Retro Cinematic Wind 02");
        TornadoSound_0_Instance = TornadoSound_0.CreateInstance();

        LightningSound = game_core.Content.Load<SoundEffect>("Retro Impact 20");
        ActiveLightningSounds = new List<SoundEffectInstance>();

    }
    public static void PlayIntroTheme()
    {
        IntroThemeInstance = IntroTheme.CreateInstance();
        IntroThemeInstance.Volume = 0.5f;
        IntroThemeInstance.Play();
    }

    public static void PlaySoundEffects(GameCore game_core)
    {
        // If cells are in the world, play the appropriate sounds
        if (game_core.GameWorld.WorldCells.Count > 0 && game_core.IsActive == true)
        {
            // Lava
            if (game_core.GameWorld.LavaCells.Count > 0 && LavaSoundTimer.Active == false)
            {
                LavaSoundShouldPlay = GameCore.Random.Next(0, 20) == 0;
                if (LavaSoundShouldPlay == true)
                {
                    LavaSound_0_Instance.Volume = GameCore.Random.Next(2, 5) * 0.1f;
                    LavaSound_0_Instance.Pitch = -0.7f;
                    LavaSound_0_Instance.Play();
                    LavaSoundTimer.Start(GameCore.Random.Next(1, 20) * 0.1f);
                }
            }

            // Tornado
            if (game_core.GameWorld.TornadoCells.Count > 0 && TornadoSoundTimer.Active == false)
            {
                TornadoSoundShouldPlay = GameCore.Random.Next(0, 10) == 0;
                if (TornadoSoundShouldPlay == true)
                {
                    TornadoSound_0_Instance.Volume = GameCore.Random.Next(3, 7) * 0.1f;
                    TornadoSound_0_Instance.Pitch = -1f;
                    TornadoSound_0_Instance.Play();
                    TornadoSoundTimer.Start(GameCore.Random.Next(100, 150) * 0.1f);
                }
            }

        }
        // Update sound timers
        if (game_core.IsActive == true)
        {
            LavaSoundTimer.Update();
            TornadoSoundTimer.Update();
        }

        // Update max sounds
        if (ActiveLightningSounds.Count > MAX_LIGHTNING_SOUNDS)
        {
            ActiveLightningSounds.Clear();
        }
    }

}