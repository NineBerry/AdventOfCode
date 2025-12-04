// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day04\Full.txt";
#endif

    Grid grid = new Grid(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.CountAllAccessiblePapers();
}

long Part2(Grid grid)
{
    return grid.RemoveAsManyPapersAsPossible();
}

class Grid
{
    private readonly HashSet<Point> Paper = [];

    public Grid(string[] input)
    {
        InitializePaper(input);
    }

    private void InitializePaper(string[] input)
    {
        int width = input.First().Length;
        int height = input.Length;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point point = new Point(x, y);
                
                if(input[y][x] == '@')
                {
                    Paper.Add(point);
                }
            }
        }
    }

    public long CountAllAccessiblePapers()
    {
        return GetAllAccessiblePapers().Count();
    }

    public long RemoveAsManyPapersAsPossible()
    {
        long removed = 0;

        while (true)
        {
            var toRemove = GetAllAccessiblePapers();

            if (!toRemove.Any()) break;

            removed += toRemove.Length;
            Paper.ExceptWith(toRemove);
        }

        return removed;
    }

    private Point[] GetAllAccessiblePapers()
    {
        return 
            Paper
            .Where(p => CountNeighboringPapers(p) < 4)
            .ToArray();
    }

    private long CountNeighboringPapers(Point point)
    {
        return 
            point
            .GetAllNeighboringPoints()
            .Count(Paper.Contains);
    }
}


record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.Left => this with { X = X - 1 },
            Direction.Right => this with { X = X + 1 },
            Direction.Bottom => this with { Y = Y + 1 },
            Direction.Top => this with { Y = Y - 1 },

            Direction.BottomLeft => this with { X = X - 1, Y = Y + 1 },
            Direction.BottomRight => this with { X = X + 1, Y = Y + 1 },
            Direction.TopRight => this with { X = X + 1, Y = Y - 1 },
            Direction.TopLeft => this with { X = X - 1, Y = Y - 1 },

            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeighboringPoints()
    {
        return [..AllDirections.Select(GetNeightboringPoint)];
    }

    public static Direction[] AllDirections = 
        [Direction.Left, Direction.Right, Direction.Top, Direction.Bottom, 
         Direction.TopLeft, Direction.TopRight, Direction.BottomLeft, Direction.BottomRight];
}

enum Direction
{
    Left,
    Right,
    Top,
    Bottom,
    TopRight,
    BottomRight,
    TopLeft,
    BottomLeft
}