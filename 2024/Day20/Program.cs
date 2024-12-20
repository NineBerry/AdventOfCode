// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day20\Sample.txt";
    bool fullOutput = true;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day20\Full.txt";
    bool fullOutput = false;
#endif

    Grid grid = new Grid(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Solve(grid, 2, fullOutput));
    Console.WriteLine("Part 2: " + Solve(grid, 20, fullOutput));
    Console.ReadLine();
}

int Solve(Grid grid, int maxCheatTime, bool fullOutput)
{
    var cheats = grid.GetCheats(maxCheatTime);  

    if (fullOutput)
    {
        var grouped = cheats.GroupBy(c => c.StepsSaved).OrderBy(g => g.Key);
        foreach (var group in grouped)
        {
            Console.WriteLine($"{group.Count()} save {group.Key}");
        }
    }

    return cheats.Count(c => c.StepsSaved >= 100);
}

class Grid
{
    private readonly int Height;
    private readonly int Width;

    private Point MapStartPoint;
    private HashSet<Point> Wall = [];

    // point => position in path
    private Dictionary<Point, int> ThePath;

    public Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;
        InitTiles(lines);
        ThePath = GetReachablePoints(MapStartPoint, int.MaxValue, obeyWalls: true).ToDictionary();
    }

    private void InitTiles(string[] lines)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Point point = new Point(x, y);
                char tile = lines[y][x];

                switch (tile)
                {
                    case '#':
                        Wall.Add(point);
                        break;
                    case 'S':
                        MapStartPoint = point;
                        break;
                }
            }
        }
    }

    private IEnumerable<(Point Point, int Distance)> GetReachablePoints(Point from, int maxDistance, bool obeyWalls)
    {
        List<(Point Point, int distance)> result = [];
        HashSet<Point> visited = [];
        PriorityQueue<List<Point>, int> queue = new();
        queue.Enqueue([from], 0);

        while (queue.TryDequeue(out var currentPath, out int distance))
        {
            Point currentPoint = currentPath.Last();
            if (visited.Contains(currentPoint)) continue;
            visited.Add(currentPoint);

            result.Add((currentPoint, distance));

            if (distance < maxDistance)
            {
                foreach (var nextPoint in AllDirections.Select(currentPoint.GetNeightboringPoint))
                {
                    if (visited.Contains(nextPoint)) continue;
                    if (!IsInGrid(nextPoint)) continue;
                    if (obeyWalls && Wall.Contains(nextPoint)) continue;

                    queue.Enqueue([.. currentPath, nextPoint], distance + 1);
                }
            }
        }

        return result;
    }

    public Cheat[] GetCheats(int maxCheatTime)
    {
        List<Cheat> cheats = [];
        
        foreach(var cheatStart in ThePath)
        {
            cheats.AddRange(GetCheats(cheatStart.Key, maxCheatTime));
        }

        return [.. cheats];
    }

    private IEnumerable<Cheat> GetCheats(Point point, int maxCheatTime)
    {
        List<Cheat> cheats = [];

        int originalPosition = ThePath[point];

        var reachablePoints = GetReachablePoints(point, maxCheatTime, obeyWalls: false);

        foreach (var reachable in reachablePoints)
        {
            if (ThePath.TryGetValue(reachable.Point, out int reachableOriginalPosition))
            {
                int reachableNewPosition = originalPosition + reachable.Distance;
                int saved = reachableOriginalPosition - reachableNewPosition;

                if (saved > 0)
                {
                    Cheat cheat = new Cheat(point, reachable.Point, saved);
                    cheats.Add(cheat);
                }
            }
        }

        return cheats;
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

record Cheat(Point Start, Point End, int StepsSaved);

enum Direction
{
    South, West, North, East
}

record struct Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = Y + 1 },
            Direction.West => this with { X = X - 1 },
            Direction.North => this with { Y = Y - 1 },
            Direction.East => this with { X = X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}