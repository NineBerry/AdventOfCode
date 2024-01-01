// #define Sample

using Path = System.Collections.Generic.HashSet<Point>;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day23\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day23\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + SolvePuzzle(lines, slippery: true));
    Console.WriteLine("Part 2: " + SolvePuzzle(lines, slippery: false));
    Console.ReadLine();
}

long SolvePuzzle(string[] input, bool slippery)
{
    Grid grid = new Grid(input, slippery);

    var possiblePaths = grid.GetPossiblePaths(grid.StartPoint, grid.EndPoint, Direction.South);
    var longestPath = possiblePaths.OrderByDescending(p => p.Distance).First();

    return longestPath.Distance;
}

class Grid
{
    private string[] grid;

    public readonly int Width;
    public readonly int Height;

    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    private bool Slippery { get; set; }

    public Grid(string[] text, bool slippery)
    {
        grid = [.. text];
        Width = grid.First().Length;
        Height = grid.Length;
        Slippery = slippery;    

        int startX = grid[0].IndexOf((char)Tile.Ground);
        StartPoint = new Point(startX, 0);

        int endX = grid[Height-1].IndexOf((char)Tile.Ground);
        EndPoint = new Point(endX, Height -1);
    }

    public IEnumerable<(Path Path, int Distance)> GetPossiblePaths(Point startPoint, Point endPoint, Direction incomingDirection)
    {
        List<(Path Path, int Distance)> foundPaths = new();
        Stack<(Point StartPoint, Path CurrentPath, Direction Direction, int Distance)> stack = new();

        stack.Push((startPoint, [], incomingDirection, 0));

        int currentLongest = 0;

        while (stack.TryPop(out var path))
        {
            // Never visit the same point twice
            if (path.CurrentPath.Contains(path.StartPoint)) continue;
            path.CurrentPath.Add(path.StartPoint);

            if (path.StartPoint == endPoint)
            {
                foundPaths.Add((path.CurrentPath, path.Distance));

                if(currentLongest < path.Distance)
                {
                    currentLongest = path.Distance;
                    // Console.WriteLine(currentLongest);
                }

                continue;
            }

            CheckNextStep(path.Direction);
            CheckNextStep(path.Direction.TurnLeft());
            CheckNextStep(path.Direction.TurnRight());

            void CheckNextStep(Direction nextDirection)
            {
                var nextNode = FindNextNode(path.StartPoint, nextDirection);

                if (nextNode.HasValue)
                {
                    stack.Push((nextNode.Value.nextNodePoint, path.CurrentPath.Clone(), nextNode.Value.nextNodeDirection, path.Distance + nextNode.Value.Distance));
                }
            }
        }

        return foundPaths;
    }

    Dictionary<(Point, Direction), (Point, Direction, int)?> cacheGetNextNode = new();

    (Point nextNodePoint, Direction nextNodeDirection, int Distance)? FindNextNode(Point currentNode, Direction currentDirection)
    {
        if(cacheGetNextNode.TryGetValue((currentNode, currentDirection), out var cachedResult))
        {
            return cachedResult;
        }

        (Point nextNodePoint, Direction nextNodeDirection, int Distance)? result;

        int distance = 0;
        Direction nextDirection = currentDirection;
        Point nextPoint = currentNode.GetNeightboringPoint(nextDirection);


        while (true)
        {
            distance++;

            if (nextPoint == this.StartPoint)
            {
                result = (this.StartPoint, Direction.North, distance);
                break;
            }
            
            if (nextPoint == this.EndPoint) 
            {
                result = (this.EndPoint, Direction.South, distance);
                break;

            }

            var possibleNextDirections = GetPossibleNextDirections(nextPoint, nextDirection);

            if (possibleNextDirections.Length == 0)
            {
                // Dead end
                result = null;
                break;
            }

            if (possibleNextDirections.Length >= 2)
            {
                // More than 1 possible next direction, we found an ode
                result = (nextPoint, nextDirection, distance);
                break;
            }

            // Exactly one next direction, continie path
            nextDirection = possibleNextDirections.First();
            nextPoint = nextPoint.GetNeightboringPoint(nextDirection);
        }
        
        cacheGetNextNode.Add((currentNode, currentDirection), result);
        return result;
    }

    Direction[] GetPossibleNextDirections(Point point, Direction incomingDirection)
    {
        if (!CanEnterPoint(point, incomingDirection)) return [];

        List<Direction> nextDirections = new();
        
        foreach(var direction in AllDirections())
        {
            if (direction == incomingDirection.Opposite()) continue;

            Point nextPoint = point.GetNeightboringPoint(direction);
            if(CanEnterPoint(nextPoint, direction))
            {
                nextDirections.Add(direction);
            }
        }

        return nextDirections.ToArray();
    }

    bool CanEnterPoint(Point point, Direction direction)
    {
        if(!IsInGrid(point)) return false;

        if (Slippery)
        {
            Tile tile = GetTile(point);
            return tile == Tile.Ground ||  tile == (Tile)direction;
        }
        else
        {
            return GetTile(point) != Tile.Forrest;
        }
    }

    public Tile GetTile(Point point)
    {
        if (!IsInGrid(point)) throw new ArgumentException("Point outside map");

        return (Tile)grid[point.Y][point.X];
    }

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
    private static Direction[] AllDirections() => [Direction.East, Direction.North, Direction.South, Direction.West];
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

enum Tile
{
    Ground = '.',
    Forrest = '#',
    South = Direction.South,
    West = Direction.West,
    North = Direction.North,
    East = Direction.East
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
    public static Direction TurnLeft(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.West => Direction.South,
            Direction.South => Direction.East,
            Direction.East => Direction.North,
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.None => Direction.None,
            _ => throw new ArgumentException("Direction", nameof(direction))
        };
    }

    public static Path Clone(this Path path)
    {
        return new Path(path);
    }
}
