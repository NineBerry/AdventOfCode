// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day08\Full.txt";
#endif

    string[] input = System.IO.File.ReadAllLines(fileName);
    Grid grid = new Grid(input);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetVisibleTrees().Count;
}

long Part2(Grid grid)
{
    return grid.GetMaxScenicScore();
}

public class Grid
{
    public Grid(string[] input)
    {
        Width = input.First().Length;
        Height = input.Length;

        InitializeHeights(input);
    }

    private void InitializeHeights(string[] input)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                char ch = input[y][x];
                Heights[point] = (int)char.GetNumericValue(ch);
            }
        }
    }

    public int Height;
    public int Width;

    private Dictionary<Point, int> Heights = [];

    private int GetHeight(Point point)
    {
        if (!Heights.TryGetValue(point, out int value))
        {
            value = int.MaxValue;
        }
        return value;
    }

    public HashSet<Point> GetVisibleTrees()
    {
        HashSet<Point> result = [];

        for (int x = 0; x < Width; x++)
        {
            DetermineVisibleTrees(new Point(x, 0), Direction.South, result);
            DetermineVisibleTrees(new Point(x, Height - 1), Direction.North, result);
        }

        for (int y = 0; y < Height; y++)
        {
            DetermineVisibleTrees(new Point(0, y), Direction.East, result);
            DetermineVisibleTrees(new Point(Width - 1, y), Direction.West, result);
        }

        return result;
    }

    private void DetermineVisibleTrees(Point tree, Direction direction, HashSet<Point> visibleTrees)
    {
        int previousHeighest = -1;

        while(GetHeight(tree) < 10)
        {
            if (GetHeight(tree) > previousHeighest)
            {
                visibleTrees.Add(tree);
            }

            previousHeighest = Math.Max(previousHeighest, GetHeight(tree));
            tree = tree.GetNeightboringPoint(direction);
        }
    }

    public int GetMaxScenicScore()
    {
        return 
            Heights.Keys
            .Where(p => 
                (p.X >= 0) && 
                (p.X < Width - 1) && 
                (p.Y > 0) && 
                (p.Y < Height - 1))
            .Max(GetScenicScore);
    }

    private int GetScenicScore(Point tree)
    {
        return 
            Point.AllDirections
            .Select(direction => DetermineSightWidth(tree, direction))
            .Aggregate((a, b) => a * b);
    }

    private int DetermineSightWidth(Point tree, Direction direction)
    {
        int myHeight = GetHeight(tree);
        int width = 0; 

        while (true)
        {
            tree = tree.GetNeightboringPoint(direction);

            if (GetHeight(tree) >= 10) break; // Outside grid
            width++;
            if (GetHeight(tree) >= myHeight) break;
        }

        return width;
    }
}


public record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.West => this with { X = this.X - 1 },
            Direction.East => this with { X = this.X + 1 },
            Direction.South => this with { X = this.X, Y = this.Y + 1 },
            Direction.North => this with { X = this.X, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

public enum Direction
{
    West,
    East,
    North,
    South,
}