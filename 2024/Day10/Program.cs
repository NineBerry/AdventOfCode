// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day10\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Grid grid = new Grid(input);

    Console.WriteLine("Part 1: " + grid.GetTrailheadScores());
    Console.WriteLine("Part 2: " + grid.GetTrailheadRatings());
    Console.ReadLine();
}

class Grid
{
    private int Height;
    private int Width;
    private Dictionary<Point, int> Heights = [];

    // Foreach key point contains one path to a point
    // height 9 and that end point. End key point can
    // appear multiple times when there are multiple
    // paths there from the key point.
    private Dictionary<Point, List<Point>> PathsToEnd = [];

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

    public long GetTrailheadScores()
    {
        return
            Heights
            .Where(p => p.Value == 0)
            .Sum(p => GetReachablePaths(p.Key).Distinct().Count());
    }

    public long GetTrailheadRatings()
    {
        return
            Heights
            .Where(p => p.Value == 0)
            .Sum(p => GetReachablePaths(p.Key).Count);
    }

    public List<Point> GetReachablePaths(Point point)
    {
        if (!IsInGrid(point)) return [];
        if (Heights[point] == 9) return [point];

        if (PathsToEnd.TryGetValue(point, out var cached))
        {
            return cached;
        }

        List<Point> result = [];

        foreach (var neighbor in point.GetAllNeighboringPoints())
        {
            if (!IsInGrid(neighbor)) continue;
            if (Heights[neighbor] - Heights[point] != 1) continue;

            result.AddRange(GetReachablePaths(neighbor));
        }

        PathsToEnd[point] = result;
        return result;
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
}

record Point(int X, int Y)
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

    public Point[] GetAllNeighboringPoints()
    {
        return AllDirections.Select(d => GetNeightboringPoint(d)).ToArray();
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

enum Direction
{
    West,
    East,
    North,
    South,
}
