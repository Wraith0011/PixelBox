using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PixelBox;

public class World
{
    public Dictionary<Vector2, Cell> WorldCells {get; set;}

    public World()
    {
        WorldCells = new Dictionary<Vector2, Cell>();
    }

    /// <summary>
    /// Calls the update method on each cell in the game world. Some cells may be updated more than once per frame.
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
            for (int i = 0; i < CellStats.WATER_SIMULATION_SPEED; i++)
            {
                water_cell.Update();
            }
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
    public void AddCell(Cell cell)
    {
        WorldCells.TryAdd(cell.Position, cell);
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