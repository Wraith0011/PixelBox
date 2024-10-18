using System;
using Microsoft.Xna.Framework;
using WraithLib;
namespace PixelBox;

public static class WeatherCycle
{        
    public static bool IsRaining {get; private set;}
    public static int RAIN_CHANCE = 1000; // Chance of rain starting. Higher values = less chance.
    public static int RAIN_DURATION = 20; // How long the rain should last in seconds
    public static Timer RainCooldownTimer;

    static WeatherCycle()
    {
        RainCooldownTimer = new Timer();
        RainCooldownTimer.TimerComplete += OnRainTimerTimeout;
    }

    public static void Update(World world)
    {
        // If it should rain and not on cooldown
        if (IsRaining == true && RainCooldownTimer.Active == false)
        {
            RainCooldownTimer.Start(RAIN_DURATION);
        }

        // If not raining, generate a chance for rain
        else if (IsRaining == false)
        {
            IsRaining = GameCore.Random.Next(0, RAIN_CHANCE) == 0;
        }

        // Update the rain cooldown timer while its raining
        if (RainCooldownTimer.Active == true)
        {
            RainCooldownTimer.Update();
        }
    }
    
    // When the time is up, the rain should stop.
    private static void OnRainTimerTimeout()
    {
        IsRaining = false;
    }
}