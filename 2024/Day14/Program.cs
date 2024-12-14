// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day14\Sample.txt";
    int gridWidth = 11;
    int gridHeight = 7;
    bool doPart2 = false;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day14\Full.txt";
    int gridWidth = 101;
    int gridHeight = 103;
    bool doPart2 = true;
#endif
    string outputFileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day14\OutputPart2.txt";

    Robot[] robots = 
        File.ReadAllLines(fileName)
        .Select(s => new Robot(s))
        .ToArray();

    Grid grid = new() { Width = gridWidth, Height = gridHeight };

    Console.WriteLine("Part 1: " + Part1(robots, grid));
    if(doPart2) Console.WriteLine("Part 2: " + Part2(robots, grid, outputFileName)); 
    Console.ReadLine();
}

long Part1(Robot[] robots, Grid grid)
{
    var movedRobots = robots.Select(r => r.Move(100, grid)).ToArray();
    return grid.CalculateSafetyFactor(movedRobots);
}

long Part2(Robot[] robots, Grid grid, string outputFileName)
{
    File.Delete(outputFileName);

    long step = 0;

    while (true) 
    {
        step++;
        
        robots = robots.Select(r => r.Move(1, grid)).ToArray();

        if (grid.HasTree(robots))
        {
            File.AppendAllText(outputFileName, grid.DrawMap(robots, false));
            break;
        }
    }

    return step;
}

class Grid
{
    public int Width { get; init; }
    public int Height { get; init; }

    public long CalculateSafetyFactor(Robot[] robots)
    {
        long middleX = (Width / 2);
        long middleY = (Height / 2);

        var rangeLeftX = new InclusiveRange(0, middleX - 1);
        var rangeRightX = new InclusiveRange(middleX + 1, Width - 1);
        var rangeTopY = new InclusiveRange(0, middleY - 1);
        var rangeBottomY = new InclusiveRange(middleY + 1, Height - 1);

        return 1
            * CountRobotsInArea(robots, rangeLeftX, rangeTopY)
            * CountRobotsInArea(robots, rangeLeftX, rangeBottomY)
            * CountRobotsInArea(robots, rangeRightX, rangeTopY)
            * CountRobotsInArea(robots, rangeRightX,  rangeBottomY);
    }

    private long CountRobotsInArea(Robot[] robots, InclusiveRange xRange, InclusiveRange yRange)
    {
        return robots.Count(r => xRange.Contains(r.Point.X) && yRange.Contains(r.Point.Y));
    }

    public bool HasTree(Robot[] robots)
    {
        var allPoints = robots.Select(r => r.Point).ToHashSet();
        return allPoints.Any(p => HasTree(p, allPoints));
    }

    public bool HasTree(Point point, HashSet<Point> otherPoints)
    {
        const int findDepth = 4;

        return (CheckDiagonal(point, otherPoints, Direction.SouthWest, findDepth) 
            && CheckDiagonal(point, otherPoints, Direction.SouthEast, findDepth));
    }

    public bool CheckDiagonal(Point point, HashSet<Point> otherPoints, Direction direction, int depth)
    {
        if(depth == 0) return true;

        Point neighbor = point.GetNeightboringPoint(direction);

        if (otherPoints.Contains(neighbor))
        {
            return CheckDiagonal(neighbor, otherPoints, direction, depth - 1);
        }

        return false;
    }

    private char[]? emptyMap = null;
    public string DrawMap(Robot[] robots, bool showCount)
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat('.', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var robotGroup in robots.GroupBy(r => r.Point))
        {
            char show = showCount ? robotGroup.Count().ToString()[0] : '*';
            // +1 for line break
            map[robotGroup.Key.Y * (Width + 1) + robotGroup.Key.X] = show;
        }

        return new string(map);
    }
}

record Robot
{
    public Point Point;
    public Point Vector;

    public Robot(string input)
    {
        var numbers = Regex.Matches(input, @"-?\d+").Select(m => long.Parse(m.Value)).ToArray();
        Point = new(numbers[0], numbers[1]);
        Vector = new(numbers[2], numbers[3]);
    }

    public Robot Move(int steps, Grid grid)
    {
        return this with
        {
            Point = Point.AddVector(Vector, steps).Teleport(grid)
        };
    }
}

record struct Point(long X, long Y)
{
    public Point AddVector(Point vector, int mul = 1)
    {
        return new Point(X + vector.X * mul, Y + vector.Y * mul);
    }

    public Point Teleport(Grid grid)
    {
        return new(
            Tools.ActualMod(X, grid.Width),
            Tools.ActualMod(Y, grid.Height));
    }

    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },

            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    None,
    South,
    West,
    North,
    East,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}

record struct InclusiveRange(long start, long end)
{
    public bool Contains(long value) => value >= start && value <= end;
}

static class Tools
{
        public static long ActualMod(long k, long n) =>  ((k %= n) < 0) ? k + n : k;

}