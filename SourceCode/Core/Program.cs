using System;
using System.Diagnostics;

using var game = new PixelBox.GameCore();
try
{
    game.Run();
} catch (Exception exception)
{
    Debug.WriteLine(exception);
}
