// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day09\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Grid grid = new Grid(input);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetRiskLevelOfLowPoints();
}

long Part2(Grid grid)
{
    var top3 = grid.GetBasinsOfLowPoints().OrderDescending().Take(3).ToArray();
    return top3.Aggregate((a, b) => a * b);
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
        if(!Heights.TryGetValue(point, out int value))
        {
            value = int.MaxValue;
        }
        return value;
    }

    private Point[] GetLowPoints()
    {
        return Heights.Keys.Where(IsLowPoint).ToArray();
    }

    private bool IsLowPoint(Point point)
    {
        int pointHeight = GetHeight(point);
        return point.GetAllNeighboringPoints().All(p => GetHeight(p) > pointHeight);
    }

    private int GetBasinSize(Point lowPoint)
    {
        HashSet<Point> basin = [];   
        Queue<Point> queue = [];
        queue.Enqueue(lowPoint);

        while(queue.TryDequeue(out var point))
        {
            int currentHeight = GetHeight(point);
            basin.Add(point);

            foreach(var neighbor in point.GetAllNeighboringPoints().Except(basin))
            {
                int neighborHeight = GetHeight(neighbor);
                if (neighborHeight > currentHeight && neighborHeight < 9) queue.Enqueue(neighbor);
            }
        }

        return basin.Count;
    }

    public long GetRiskLevelOfLowPoints()
    {
        return GetLowPoints().Sum(p => GetHeight(p) + 1);
    }
    
    public int[] GetBasinsOfLowPoints()
    {
        return GetLowPoints().Select(GetBasinSize).ToArray();
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

            Direction.South => this with { X = this.X , Y = this.Y + 1 },
            Direction.North => this with { X = this.X , Y = this.Y - 1 },
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
