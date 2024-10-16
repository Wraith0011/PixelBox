using Microsoft.Xna.Framework;
using System.Collections.Generic;
using WraithLib;

namespace PixelBox;

public class World
{
    public Dictionary<Vector2, Cell> WorldCells {get; set;}
    public List<Water> WaterCells {get; private set;}
    public List<Sand> SandCells {get; private set;}
    public List<Stone> StoneCells {get; private set;}
    public List<Steam> SteamCells {get; private set;}
    public List<Lava> LavaCells {get; private set;}
    public List<Wood> WoodCells {get; private set;}
    public List<Tornado> TornadoCells {get; private set;}

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
        // Keep track of what cells need to be updated
        WaterCells = new List<Water>();
        SandCells = new List<Sand>();
        StoneCells = new List<Stone>();
        SteamCells = new List<Steam>();
        LavaCells = new List<Lava>();
        WoodCells = new List<Wood>();
        TornadoCells = new List<Tornado>();
        
        // Get cells in the world, and add them to the list of that cell type
        foreach (Cell cell in WorldCells.Values)
        {
            switch (cell)
            {
                case Water water_cell:
                    WaterCells.Add(water_cell);
                    break;
                case Sand sand_cell:
                    SandCells.Add(sand_cell);
                    break;
                case Stone stone_cell:
                    StoneCells.Add(stone_cell);
                    break;
                case Steam steam_cell:
                    SteamCells.Add(steam_cell);
                    break;
                case Lava lava_cell:
                    LavaCells.Add(lava_cell);
                    break;
                case Wood wood_cell:
                    WoodCells.Add(wood_cell);
                    break;
                case Tornado tornado_cell:
                    TornadoCells.Add(tornado_cell);
                    break;
            }
        }

        // Update the cells by type
        foreach (Water water_cell in WaterCells)
        {
            water_cell.Update();
        }
        foreach (Sand sand_cell in SandCells)
        {
            sand_cell.Update();
        }
        foreach (Stone stone_cell in StoneCells)
        {
            stone_cell.Update();
        }
        foreach (Steam steam_cell in SteamCells)
        {
            steam_cell.Update();
        }
        foreach (Lava lava_cell in LavaCells)
        {
            lava_cell.Update();
        }
        foreach (Wood wood_cell in WoodCells)
        {
            wood_cell.Update();
        }
        foreach (Tornado tornado_cell in TornadoCells)
        {
            tornado_cell.Update();
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
    /// Moves a cell while keeping track of the WorldCells dictionary. This will override cells if they are present at the new location.
    /// </summary>
    public void MoveCell(Cell cell, Vector2 new_position)
    {
        // Remove the old position from the world dictionary
        WorldCells.Remove(cell.Position);
        // Update the current position
        cell.Position = new_position;
        // Update the world dictionary with the new position & cell
        WorldCells[cell.Position] = cell;
    }
    public void TryMoveCell(Cell cell, Vector2 new_position)
    {
        bool success;
        // Attempt to update the world dictionary with the new position & cell
        success = WorldCells.TryAdd(cell.Position, cell);
        if (success == true)
        {
            // Remove the old position from the world dictionary
            WorldCells.Remove(cell.Position);
            // Update the current position
            cell.Position = new_position;
        }
        else if (GetCell(new_position) == null)
        {
            MoveCell(cell, new_position);
        }
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