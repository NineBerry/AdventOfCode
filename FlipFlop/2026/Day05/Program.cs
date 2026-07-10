//#define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day05\Full.txt";
#endif

    Map map = new Map(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(map));
    Console.WriteLine("Part 2: " + Part2(map));
    Console.WriteLine("Part 3: " + Part3(map));

    Console.ReadLine();
}

long Part1(Map map)
{
    return map.GetUniquePath(0).Count;
}

long Part2(Map map)
{
    return map.GetMaxPathLengthAfterChange(0);
}

long Part3(Map map)
{
    return map.GetMaxPathLengthAfterChange(3);
}

class Map  
{
    private Dictionary<Point, Direction> points = new();
    private int Height { get; }
    private int Width { get; }

    public Map(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                Point point = new Point(x, y);

                points[point] = (Direction)tile;
            }
        }
    }

    public HashSet<Point> GetUniquePath(int illegalTurnsAllowed)
    {
        HashSet<Point> visited = [];

        var point = new Point(0, 0);

        while (!visited.Contains(point) || illegalTurnsAllowed > 0)
        {
            if (visited.Contains(point))
            {
                var direction = points[point].TurnRight();
                point = point.GetNeightboringPoint(direction);
                illegalTurnsAllowed--;
            }
            else
            {
                visited.Add(point);
                var direction = points[point];
                point = point.GetNeightboringPoint(direction);
            }
            
        }

        return visited;
    }

    public long GetMaxPathLengthAfterChange(int illegalTurnsAllowed)
    {
        long maxPathCount = 0;

        var originalPath = GetUniquePath(illegalTurnsAllowed);

        foreach (var point in originalPath)
        {
            if (!IsOnEdge(point))
            {
                var lOriginalDirection = points[point];

                try
                {
                    foreach (var direction in AllDirections)
                    {
                        if (direction != lOriginalDirection)
                        {
                            points[point] = direction;
                            var newPath = GetUniquePath(illegalTurnsAllowed);
                            maxPathCount = Math.Max(maxPathCount, newPath.Count);
                        }
                    }
                }
                finally
                {
                    points[point] = lOriginalDirection;
                }
            }
        }

        return maxPathCount;
    }


    private bool IsOnEdge(Point point)
    {
        return point.X == 0 || point.X == Width - 1 || point.Y == 0 || point.Y == Height - 1;
    }

    private static Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
}

record struct Point(int X, int Y)
{
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
}
enum Direction
{
    None = '\0',
    South = 'v',
    West = '<',
    North = '^',
    East = '>'
}

static class Extensions
{
    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}