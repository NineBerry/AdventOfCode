// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day06\Full.txt";
#endif
    var lines = File.ReadAllLines(fileName);
    Grid grid = new Grid(lines);

    var path = grid.DoWalk(grid.MapStartPoint, grid.MapStartDirection, []);

    Console.WriteLine("Part 1: " + path.Distinct().Count());
    Console.WriteLine("Part 2: " + grid.CountPossibleBlocksToCreateLoop(path));
    Console.ReadLine();
}

class Grid
{
    private HashSet<Point> Wall = [];

    private readonly int Height;
    private readonly int Width;

    public readonly Direction MapStartDirection;
    public readonly Point MapStartPoint;

    public Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Point point = new Point(x, y);
                char tile = lines[y][x];

                switch (tile)
                {
                    case '.':
                        // Just ground, do nothing
                        break;
                    case '#':
                        Wall.Add(point);
                        break;
                    case (char)Direction.North:
                    case (char)Direction.South:
                    case (char)Direction.West:
                    case (char)Direction.East:
                        MapStartDirection = (Direction) tile;
                        MapStartPoint = point;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public Point[] DoWalk(
        Point start, Direction startDirection, HashSet<Point> additionalWall)
    {
        HashSet<(Point,Direction)> visited = [];
        List<Point> path = [];

        Point currentPoint = start;
        Direction currentDirection = startDirection;

        while (IsInGrid(currentPoint))
        {
            // Cycle detected
            if (visited.Contains((currentPoint, currentDirection))) return [];
            visited.Add((currentPoint, currentDirection));

            Point nextPoint = currentPoint.GetNeightboringPoint(currentDirection);

            if (Wall.Contains(nextPoint) || additionalWall.Contains(nextPoint))
            {
                currentDirection = currentDirection.TurnRight();
            }
            else
            {
                path.Add(currentPoint);
                currentPoint = nextPoint;
            }
        }

        return [..path];
    }

    public long CountPossibleBlocksToCreateLoop(Point[] path)
    {
        HashSet<Point> possibleBlocks = [];
        HashSet<Point> pathSoFar = [];

        foreach (var point in path)
        {
            if (pathSoFar.Contains(point)) continue;
            if (possibleBlocks.Contains(point)) continue;
            if (point == MapStartPoint) continue;

            // New idea for improving performance:
            // Returned path contains direction
            // Build up visited directions so far here and pass
            // them to DoWalk to init visited there and start
            // dowalk with previous path position

            if (CheckBlockCreatesCycle(point, MapStartPoint, MapStartDirection))
            {
                possibleBlocks.Add(point);
            }

            pathSoFar.Add(point);
        }

        return possibleBlocks.Count;
    }

    private bool CheckBlockCreatesCycle(Point block, Point currentPoint, Direction currentDirection)
    {
        return !DoWalk(currentPoint, currentDirection, [block]).Any();
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
enum Direction
{
    None,
    South = 'v',
    West = '<',
    North = '^',
    East = '>',
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
