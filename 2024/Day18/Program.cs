// #define Sample

using System.Text.RegularExpressions;
using Path = System.Collections.Generic.List<Point>;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day18\Sample.txt";
    int gridWidth = 7;
    int part1Time = 12;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day18\Full.txt";
    int gridWidth = 71;
    int part1Time = 1024;
#endif

    Point[] points = 
        File.ReadAllLines(fileName)
        .Select(Point.Parse)
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(points, gridWidth, part1Time));
    Console.WriteLine("Part 2: " + Part2(points, gridWidth));
    Console.ReadLine();
}

long Part1(Point[] points, int gridSize, int afterTime)
{
    var relevantPoints = points.Take(afterTime).ToHashSet();
    Grid grid = new Grid(gridSize, gridSize);
    return grid.GetShortestPath(relevantPoints).Count - 1;
}

string Part2(Point[] points, int gridSize)
{
    Grid grid = new Grid(gridSize, gridSize);
    
    HashSet<Point> corrupted = [];
    HashSet<Point> lastPathPoints = grid.GetShortestPath(corrupted).ToHashSet();

    foreach(var test in points.Index())
    {
        corrupted.Add(test.Item);
        
        // Only calculate new path when new point is in last path found
        if (lastPathPoints.Contains(test.Item))
        {
            lastPathPoints = grid.GetShortestPath(corrupted).ToHashSet();
        }

        if(!lastPathPoints.Any())
        {
            return points[test.Index].ToString();
        }
    }

    return "";
}

class Grid
{
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }   

    private int Width;
    private int Height;

    public Path GetShortestPath(HashSet<Point> corrupted)
    {
        Point start = new Point(0, 0);
        Point target = new Point(Width -1, Height - 1);
        
        HashSet<Point> visited = [];
        PriorityQueue<Path, long> queue = new();
        queue.Enqueue([start], 0);

        while(queue.TryDequeue(out var currentPath, out long distance))
        {
            Point currentPoint = currentPath.Last();
            if (visited.Contains(currentPoint)) continue;
            visited.Add(currentPoint);

            if (currentPoint == target) return currentPath;

            foreach (var nextPoint in AllDirections.Select(currentPoint.GetNeightboringPoint))
            {
                if (visited.Contains(nextPoint)) continue;
                if (!IsInGrid(nextPoint)) continue;
                if (corrupted.Contains(nextPoint)) continue;

                queue.Enqueue([.. currentPath, nextPoint], distance + 1);
            }
        }

        return [];
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    private static Direction[] AllDirections => [
        Direction.East,
        Direction.North,
        Direction.South,
        Direction.West
    ];

}

record struct Point(int X, int Y)
{
    public static Point Parse(string input)
    {
        var numbers = Tools.ParseIntegers(input);
        return new(numbers[0], numbers[1]);
    }

    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

enum Direction
{
    South,
    West,
    North,
    East
}

static class Tools
{
    public static int[] ParseIntegers(string input)
    {
        return
            Regex.Matches(input, @"\d+")
            .Select(m => int.Parse(m.Value))
            .ToArray();
    }
}