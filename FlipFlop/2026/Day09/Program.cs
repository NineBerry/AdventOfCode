// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day09\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    Map map = new Map(lines);

    Console.WriteLine("Part 1: " + Part1(map));
    Console.WriteLine("Part 2: " + Part2(map));
    Console.WriteLine("Part 3: " + Part3(map));

    Console.ReadLine();
}

long Part1(Map map)
{
    return map.GetShortestPath(canTeleportThroughCorridors: false, canTeleportThroughWalls: false);
}

long Part2(Map map)
{
    return map.GetShortestPath(canTeleportThroughCorridors: true, canTeleportThroughWalls: false);
}

long Part3(Map map)
{
    return map.GetShortestPath(canTeleportThroughCorridors: false, canTeleportThroughWalls: true);
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

    private bool IsCrossing(Point point)
    {
        return Point.AllDirections.All(d => !IsAWall(point.GetNeightboringPoint(d)));
    }

    internal long GetShortestPath(bool canTeleportThroughCorridors, bool canTeleportThroughWalls)
    {
        HashSet<(Point Point, bool WithPortal)> visited = [];
        PriorityQueue<(Point Point, bool WithPortal), int> todos = new();

        todos.Enqueue((StartPoint, false), 0);

        while (todos.TryDequeue(out var current, out var currentLength))
        {
            if (visited.Contains((current.Point, current.WithPortal))) continue;
            visited.Add((current.Point, current.WithPortal));

            if (current.Point == EndPoint) return currentLength;

            foreach(var direction in Point.AllDirections)
            {
                var nodes = GetNodesInDirection(current.Point, direction);

                if (nodes.Any())
                {
                    foreach (var next in nodes)
                    {
                        Enqueue(next.Point, next.Distance, false);
                    }
                    Enqueue(nodes.Last().Point, nodes.Last().Distance, true);
                }
            }

            void Enqueue(Point nextPoint, int distance, bool specialHandlingEndOfCorridor)
            {
                if (specialHandlingEndOfCorridor)
                {
                    if (canTeleportThroughCorridors)
                    {
                        todos.Enqueue((nextPoint, WithPortal: false), currentLength + 1);
                    }

                    if (canTeleportThroughWalls)
                    {
                        if (!IsCrossing(current.Point) && !IsCrossing(nextPoint))
                        {
                            int effort = current.WithPortal ? 2 : 3;
                            todos.Enqueue((nextPoint, WithPortal: true), currentLength + effort);
                        }
                    }
                }
                else 
                {
                    todos.Enqueue((nextPoint, WithPortal: false), currentLength + distance);
                }
            }
        }

        return 0;
    }

    private IEnumerable<(Point Point, int Distance)> GetNodesInDirection(Point point, Point.Direction direction)
    {
        List<(Point Point, int Distance)> result = [];

        int distance = 0;

        while (true)
        {
            point = point.GetNeightboringPoint(direction);

            if (IsAWall(point)) break;

            distance++;
            result.Add((point, distance));
        }

        return result;
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