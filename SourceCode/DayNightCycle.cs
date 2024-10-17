using System;
using Microsoft.Xna.Framework;
using WraithLib;

namespace PixelBox;

public class DayNightCycle
{    
    public Canvas WorldCanvas {get; private set;}
    public int CycleDuration {get; private set;}
    public Color[] CycleColors;

    public enum TimeOfDay {Morning, Day, Evening, Night};
    public TimeOfDay[] TimesOfDay = { TimeOfDay.Morning, TimeOfDay.Day, TimeOfDay.Evening, TimeOfDay.Night };
    public TimeOfDay CurrentTimeOfDay;

    public float Time{get; private set;}

    public DayNightCycle(int cycle_duration, Canvas canvas, Color morning, Color day, Color evening, Color night, TimeOfDay current_time)
    {   
        CycleDuration = cycle_duration * 60; // set time cycle duration in seconds
        WorldCanvas = canvas;
        CycleColors = new Color[] { morning, day, evening, night };
        Time = 0.0f;
        CurrentTimeOfDay = current_time;
    }

    public void Update()
    {
        Time++;
        if (Time >= CycleDuration)
        {
            CurrentTimeOfDay = GetNextTimeOfDay(CurrentTimeOfDay);
            SetCanvasColor();
            Time = 0.0f;
        }
    }

    public TimeOfDay GetNextTimeOfDay(TimeOfDay current_time_of_day)
    {
        int index = (int)current_time_of_day + 1;
        index = index % TimesOfDay.Length;
        return TimesOfDay[index];
    }

    private void SetCanvasColor()
    {
        WorldCanvas.CurrentColor = CycleColors[(int)CurrentTimeOfDay];
    }
}