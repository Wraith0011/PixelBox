using Microsoft.Xna.Framework;
using System.Collections.Generic;
using WraithLib;

namespace PixelBox;

public class World
{
    public Dictionary<Vector2, Cell> WorldCells {get; set;}

    public Canvas WorldCanvas {get; set;}
    public Vector2 WorldCanvasSize {get; private set;} // World boundary for cells is based on this. This is the native size of the game, not the scaled canvas size.

    public World(Vector2 world_canvas_size)
    {
        WorldCells = new Dictionary<Vector2, Cell>();
        WorldCanvas = new Canvas((int)world_canvas_size.X, (int)world_canvas_size.Y);
        WorldCanvasSize = world_canvas_size;
    }

    /// <summary>
    /// Calls the update method on each cell in the game world.
    /// </summary>
    public void Update()
    {   
        // Get cells in the world
        List<Water> water_cells = new List<Water>();
        foreach (Cell cell in WorldCells.Values)
        {
            if (cell is Water water_cell)
            {
                water_cells.Add(water_cell);
            }
        }
        // Update the cells
        foreach (Water water_cell in water_cells)
        {
            water_cell.Update();
        }
    }

    /// <summary>
    /// Calls the draw method on each cell in the game world.
    /// </summary>
    public void Draw()
    {
        foreach (Cell cell in WorldCells.Values)
        {
            cell.Draw();
        }
    }

    /// <summary>
    /// Attempts to add a cell to the world dictionary, will not replace existing cells.
    /// </summary>
    public void TryAddCell(Cell cell)
    {
        WorldCells.TryAdd(cell.Position, cell);
    }

    /// <summary>
    /// Adds the given cell to the world dictionary, and replaces any cell that was there beforehand.
    /// </summary>
    public void AddCell(Cell cell)
    {
        WorldCells[cell.Position] = cell;
    }

    /// <summary>
    /// Removes the given cell from the world dictionary.
    /// </summary>
    public void RemoveCell(Cell cell)
    {
        if (cell != null)
        {
            WorldCells.Remove(cell.Position);
        }
    }
    
    /// <summary>
    /// Swaps a cell and its position with a neighboring cell. 
    /// </summary>
    public void SwapCell(Cell cell, Cell neighbor)
    {
        // Remove both positions from the dictionary
        WorldCells.Remove(cell.Position);
        WorldCells.Remove(neighbor.Position);

        // Swap positions
        Vector2 original_position = cell.Position;
        cell.Position = neighbor.Position;
        neighbor.Position = original_position;

        // Add both cells back to the dictionary
        WorldCells.Add(cell.Position, cell);
        WorldCells.Add(neighbor.Position, neighbor);
    }

    /// <summary>
    /// Returns the cell at a given position. Returns null if no cell exists.
    /// </summary>
    public Cell GetCell(Vector2 position)
    {
        if ( WorldCells.TryGetValue(position, out Cell cell) )
        {
            return cell;
        }
        else { return null; }
    }
}