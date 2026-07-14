// #define Sample

using System.Numerics;
using WallSideSegment = (Point WallSegment, Point CorridorSegment);

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day09\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.WriteLine("Part 3: " + Part3(lines));

    Console.ReadLine();
}

BigInteger Part1(string[] lines)
{
    Map map = new Map(lines);
    return map.GetShortestPath(map.StartPoint, map.EndPoint, canTeleportThroughCorridors: false, canTeleportThroughWalls: false);
}

BigInteger Part2(string[] lines)
{
    Map map = new Map(lines);
    return map.GetShortestPath(map.StartPoint, map.EndPoint, canTeleportThroughCorridors: true, canTeleportThroughWalls: false);
}

BigInteger Part3(string[] lines)
{
    Map map = new Map(lines);
    return map.GetShortestPath(map.StartPoint, map.EndPoint, canTeleportThroughCorridors: false, canTeleportThroughWalls: true);
}

class Map
{
    private Dictionary<Point, Tile> tiles = [];

    
    public Point StartPoint { get; }
    public Point EndPoint { get; }

    public Map(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Tile tile = (Tile)lines[y][x];
                Point point = new Point(x, y);
                tiles[point] = tile;

                if (tile == Tile.Start) StartPoint = point;
                if (tile == Tile.End) EndPoint = point;
            }
        }

        if (StartPoint == null || EndPoint == null) throw new ArgumentException("No Start or End found");
    }

    private bool IsAWall(Point point)
    {
        return tiles[point] is Tile.Wall;
    }

    internal BigInteger GetShortestPath(Point start, Point end, bool canTeleportThroughCorridors, bool canTeleportThroughWalls)
    {
        HashSet<(Point, WallSideSegment?, WallSideSegment?)> visited = [];
        PriorityQueue<(Point Point, WallSideSegment? Portal1, WallSideSegment? Portal2, bool ShallChangePortal1, bool ShallChangePortal2), int> todos = new();

        Dictionary<Point, bool> knownPathState= [];
        knownPathState.Add(end, true);

        todos.Enqueue((start, null, null, true, true), 0);

        int lastLog = 0;

        while (todos.TryDequeue(out var current, out var currentLength))
        {

            if (currentLength - lastLog > 10)
            {
                Console.WriteLine(currentLength);
                lastLog = currentLength;
            }

            if (visited.Contains((current.Point, current.Portal1, current.Portal2))) continue;

            visited.Add((current.Point, current.Portal1, current.Portal2));
            visited.Add((current.Point, current.Portal2, current.Portal1));

            if (current.Point == end) return currentLength;

            CheckEnqueue(current.Point.GetAllNeightboringPoints(), current.ShallChangePortal1, current.ShallChangePortal2);

            if (canTeleportThroughCorridors)
            {
                CheckEnqueue(GetCorridorTeleportingTargets(current.Point), current.ShallChangePortal1, current.ShallChangePortal2);
            }

            if (canTeleportThroughWalls)
            {
                CheckEnqueue(GetPossibleWallTeleportation(current.Point, current.Portal1, current.Portal2), true, current.ShallChangePortal2);
                CheckEnqueue(GetPossibleWallTeleportation(current.Point, current.Portal2, current.Portal1), current.ShallChangePortal1, true);
            }

            void CheckEnqueue(IEnumerable<Point> candidates, bool shallChangePortal1, bool shallChangePortal2)
            {
                foreach (var candidate in candidates)
                {
                    if (visited.Contains((candidate, current.Portal1, current.Portal2)) ) continue;
                    if (IsAWall(candidate)) continue;

                    todos.Enqueue((candidate, current.Portal1, current.Portal2, shallChangePortal1, shallChangePortal2), currentLength + 1);
                }
            }

            if (canTeleportThroughWalls)
            {
                var newPortals = Point.AllDirections.Select(d => GetWallSideSegmentInDirection(current.Point, d)).ToArray();
                foreach (var newPortal in newPortals)
                {
                    if (current.ShallChangePortal1)
                    {
                        if (newPortal.CorridorSegment != current.Portal2?.CorridorSegment)
                        {
                            if (!visited.Contains((current.Point, newPortal, current.Portal2)))
                            {
                                todos.Enqueue((current.Point, newPortal, current.Portal2, false, current.ShallChangePortal2), currentLength + 1);
                            }
                        }
                    }

                    if (current.ShallChangePortal2)
                    {
                        if (newPortal.CorridorSegment != current.Portal1?.CorridorSegment)
                        {
                            if (!visited.Contains((current.Point, current.Portal1, newPortal)))
                            {
                                todos.Enqueue((current.Point, current.Portal1, newPortal, current.ShallChangePortal2, false), currentLength + 1);
                            }
                        }
                    }
                }
            }
        }

        return 0;
    }

    private IEnumerable<Point> GetCorridorTeleportingTargets(Point point)
    {
        List<Point> result = [];

        foreach (var direction in Point.AllDirections)
        {
            result.Add(GetWallSideSegmentInDirection(point, direction).CorridorSegment);
        }

        return result;
    }

    private IEnumerable<Point> GetPossibleWallTeleportation(Point point, WallSideSegment? portalFrom, WallSideSegment? portalTo)
    {
        List<Point> result = [];

            foreach (var direction in Point.AllDirections)
            {
                var neighbor = point.GetNeightboringPoint(direction);

                if (neighbor == portalFrom?.WallSegment && portalTo != null) result.Add(portalTo.Value.CorridorSegment);
            }

        return result;
    }

    private WallSideSegment GetWallSideSegmentInDirection(Point point, Point.Direction direction)
    {
        Point endOfCorridor = point;
        Point next = endOfCorridor;

        while (!IsAWall(next))
        {
            endOfCorridor = next;
            next = endOfCorridor.GetNeightboringPoint(direction);
        }

        return(WallSegment: next, CorridorSegment: endOfCorridor);
    }

    public enum Tile
    {
        None = '\0',
        Start = 'S',
        End = 'E',
        Wall = '#',
        Space = '.'
    }
}

record class Point(int X, int Y)
{
    public Point[] GetAllNeightboringPoints()
    {
        var me = this;
        return [.. AllDirections.Select(me.GetNeightboringPoint)];
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

    public enum Direction
    {
        None = '\0',
        South = 'v',
        West = '<',
        North = '^',
        East = '>'
    }

    public static readonly Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
}

// 842 too high