// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day12\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Grid grid = new Grid(input);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetShortestPath(grid.Start);
}

long Part2(Grid grid)
{
    return 
        grid
        .GetAllPointsWithHeight(1)
        .Select(grid.GetShortestPath)
        .Min();
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

                if (ch == 'S')
                {
                    Start = point;
                    Heights[point] = 1;
                }
                else if (ch == 'E')
                {
                    End = point;
                    Heights[point] = 26;
                }
                else
                {
                    Heights[point] = (ch - 'a') + 1;
                }
            }
        }
    }

    public int Height;
    public int Width;
    public Point Start = new Point(0, 0);
    public Point End = new Point(0, 0);
    private Dictionary<Point, int> Heights = [];

    public int GetShortestPath(Point start)
    {
        HashSet<Point> visited = [];
        PriorityQueue<(Point Point, int Steps), int> todos = new();
        todos.Enqueue((start, 0), 0);

        while (todos.TryDequeue(out var current, out _))
        {
            if (visited.Contains(current.Point)) continue;
            visited.Add(current.Point);

            if (current.Point == End)
            {
                return current.Steps;
            }

            int currentHeight = GetHeight(current.Point);

            foreach (var neighbor in current.Point.GetAllNeighboringPoints())
            {
                int neighborHeight = GetHeight(neighbor);
                if(neighborHeight <= currentHeight + 1)
                {
                    todos.Enqueue((neighbor, current.Steps + 1), current.Steps + 1);
                }
            }
        }

        return int.MaxValue;
    }

    private int GetHeight(Point point)
    {
        if (!Heights.TryGetValue(point, out int value))
        {
            value = int.MaxValue;
        }
        return value;
    }

    public Point[] GetAllPointsWithHeight(int height)
    {
        return Heights.Where(p => p.Value == height).Select(p => p.Key).ToArray();
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

    public Point[] GetAllNeighboringPoints()
    {
        return AllDirections.Select(d => GetNeightboringPoint(d)).ToArray();
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
