// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day24\Full.txt";
#endif

    Direction[][] paths = File.ReadAllLines(fileName).Select(l => StringToDirections(l)).ToArray();
    var initialBlackTiles = ApplyPaths(paths);

    Console.WriteLine("Part 1: " + Part1(initialBlackTiles));
    Console.WriteLine("Part 2: " + Part2(initialBlackTiles));
    Console.ReadLine();
}

long Part1(HashSet<Point> blackTiles)
{
    return blackTiles.Count;
}

long Part2(HashSet<Point> blackTiles)
{
    Grid grid = new Grid(blackTiles);

    foreach(var step in Enumerable.Range(1, 100))
    {
        grid.RunGameOfTiles();
    }

    return grid.BlackCount;
}

HashSet<Point> ApplyPaths(Direction[][] paths)
{
    HashSet<Point> blackTiles = [];

    foreach (var path in paths)
    {
        Point endPoint = GetEndPointOfPath(path);
        if (blackTiles.Contains(endPoint))
        {
            blackTiles.Remove(endPoint);
        }
        else
        {
            blackTiles.Add(endPoint);
        }
    }

    return blackTiles;
}

Point GetEndPointOfPath(Direction[] path)
{
    Point point = new Point(0, 0);

    foreach(var direction in path)
    {
        point = point.GetNeightboringPoint(direction);
    }

    return point;
}


Direction[] StringToDirections(string s)
{
    return Regex.Matches(s, "e|w|ne|nw|se|sw").Select(m => StringToDirection(m.Value)).ToArray();
}

Direction StringToDirection(string s)
{
    return s switch
    {
        "e" => Direction.East,
        "ne" => Direction.NorthEast,
        "nw" => Direction.NorthWest,
        "w" => Direction.West,
        "sw" => Direction.SouthWest,
        "se" => Direction.SouthEast,
        _ => throw new ApplicationException("invalid input")
    };
}

class Grid
{
    public Grid(HashSet<Point> initialBlack)
    {
        Black = new(initialBlack);
    }

    private HashSet<Point> Black;

    public void RunGameOfTiles()
    {
        var oldBlack = new HashSet<Point>(Black);
        var oldWhite = oldBlack.SelectMany(a => a.GetAllNeighboringPoints()).Except(oldBlack).ToHashSet();
        Black.Clear();

        foreach (var black in oldBlack)
        {
            if (CountNeighborOfType(black, oldBlack) is 1 or 2)
            {
                Black.Add(black);
            }
        }

        foreach (var white in oldWhite)
        {
            if (CountNeighborOfType(white, oldBlack) == 2)
            {
                Black.Add(white);
            }
        }
    }

    public int BlackCount => Black.Count;

    private int CountNeighborOfType(Point point, HashSet<Point> objectsOfType)
    {
        return point.GetAllNeighboringPoints().Intersect(objectsOfType).Count();
    }
}


record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.West => this with { X = this.X - 2 },
            Direction.East => this with { X = this.X + 2 },

            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeighboringPoints()
    {
        return AllDirections.Select(d => GetNeightboringPoint(d)).ToArray();
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.NorthEast, Direction.NorthWest, Direction.SouthEast, Direction.SouthWest];
}

enum Direction
{
    West,
    East,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}
