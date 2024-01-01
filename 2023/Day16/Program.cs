using System.ComponentModel.DataAnnotations;
using VisitedCells = System.Collections.Generic.Dictionary<Cell, System.Collections.Generic.HashSet<Direction>>;

internal class Program
{
    // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day16\Sample.txt";
    private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day16\Full.txt";

    static void Main()
    {
        Grid grid = new Grid(GetLines());
            
        // Part 1
        int energizedCount = grid.TestConfiguration(new Cell(0, 0), Direction.East);
        Console.WriteLine("Part 1: " + energizedCount);

        // Part 2
        int maxEnergizedCount = grid.TestEdgeConfigurations();
        Console.WriteLine("Part 2: " + maxEnergizedCount);

        Console.ReadLine();
    }

    private static string[] GetLines()
    {
        return File.ReadAllLines(fileName);
    }
}

enum Tile
{
    Ground = '.',
    MirrorSWToNE = '/',
    MirrorNWToSE = '\\',
    SplitterWestEast = '-',
    SplitterNorthSouth = '|',
}

enum Direction
{
    None,
    South,
    West,
    North,
    East
}

record struct Cell(int X, int Y);

class Grid
{
    private string[] grid;

    public readonly int Width;
    public readonly int Height;

    public Grid(string[] text)
    {
        grid = [.. text];
        Width = grid.First().Length;
        Height = grid.Length;
    }

    public int TestEdgeConfigurations()
    {
        int curentMax = 0;

        for (int x = 0; x < Width; x++)
        {
            curentMax = Math.Max(curentMax, TestConfiguration(new Cell(x, 0), Direction.South));
            curentMax = Math.Max(curentMax, TestConfiguration(new Cell(x, Height - 1), Direction.North));
        }

        for (int y = 0; y < Height; y++)
        {
            curentMax = Math.Max(curentMax, TestConfiguration(new Cell(0, y), Direction.East));
            curentMax = Math.Max(curentMax, TestConfiguration(new Cell(Width - 1, y), Direction.West));
        }

        return curentMax;
    }

    public int TestConfiguration(Cell cell, Direction direction)
    {
        VisitedCells visitedCells = new();

        MoveBeamToCell(cell, direction, visitedCells);

        return visitedCells.Count;
    }


    private void MoveBeamToCell(Cell startCell, Direction startDirection, VisitedCells visitedCells)
    {
        Stack<(Cell cell, Direction direction)> todos = new();

        todos.Push((startCell, startDirection));

        while (todos.Count > 0)
        {
            (var cell, var direction) = todos.Pop();

            while (true)
            {

                // Stop outside map
                if (!IsInGrid(cell)) break;

                if (!visitedCells.TryGetValue(cell, out var visitedDirections))
                {
                    visitedDirections = new();
                    visitedCells.Add(cell, visitedDirections);
                }

                // Stop if we have already crossed this cell in this direction
                if (visitedDirections.Contains(direction)) break;

                // Remember we crossed this cell in this direction
                visitedDirections.Add(direction);

                Direction[] newDirections = ApplyTile(GetCellTile(cell), direction);

                if (newDirections.Length > 1)
                {
                    Cell newCell = GetNeightboringCell(cell, newDirections[1]);
                    todos.Push((newCell, newDirections[1]));
                }

                direction = newDirections[0];
                cell = GetNeightboringCell(cell, direction);
            }
        }
    }

    private static Direction[] ApplyTile(Tile tile, Direction direction)
    {
        return (tile, direction) switch
        {
            (Tile.MirrorSWToNE, Direction.North) => [Direction.East],
            (Tile.MirrorSWToNE, Direction.South) => [Direction.West],
            (Tile.MirrorSWToNE, Direction.East) => [Direction.North],
            (Tile.MirrorSWToNE, Direction.West) => [Direction.South],

            (Tile.MirrorNWToSE, Direction.North) => [Direction.West],
            (Tile.MirrorNWToSE, Direction.South) => [Direction.East],
            (Tile.MirrorNWToSE, Direction.East) => [Direction.South],
            (Tile.MirrorNWToSE, Direction.West) => [Direction.North],

            (Tile.SplitterNorthSouth, Direction.West or Direction.East) => [Direction.North, Direction.South],
            (Tile.SplitterWestEast, Direction.North or Direction.South) => [Direction.West, Direction.East],
            _ => [direction]
        };
    }

    public Tile GetCellTile(Cell cell)
    {
        if (!IsInGrid(cell)) throw new ArgumentException("Cell outside map");

        return (Tile)grid[cell.Y][cell.X];
    }

    public bool IsInGrid(Cell cell)
    {
        return
            cell.X >= 0
            && cell.X < Width
            && cell.Y >= 0
            && cell.Y < Height;
    }

    private static Cell GetNeightboringCell(Cell cell, Direction direction)
    {
        return direction switch
        {
            Direction.South => cell with { Y = cell.Y + 1 },
            Direction.West => cell with { X = cell.X - 1 },
            Direction.North => cell with { Y = cell.Y - 1 },
            Direction.East => cell with { X = cell.X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}



