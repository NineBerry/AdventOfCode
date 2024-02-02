// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day24\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    Grid grid = new Grid(lines);


    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

int Part1(Grid grid)
{
    return grid.GetShortestPathFromZeroToAllNodes(endAtZero: false);
}

int Part2(Grid grid)
{
    return grid.GetShortestPathFromZeroToAllNodes(endAtZero: true);
}

class Grid
{
    private string[] grid;

    public readonly int Width;
    public readonly int Height;

    public Grid(string[] text)
    {
        grid = [.. text];
        Width = grid.First().Length;
        Height = grid.Length;

        InitializeNodes();
        InitializeNodePaths();
    }

    private void InitializeNodes()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                Tile tile = GetTile(point);

                if (tile != Tile.Ground && tile != Tile.Wall)
                {
                    Nodes.Add(new Node { Value = (char)tile, Point = point });
                }
            }
        }
    }

    private void InitializeNodePaths()
    {
        foreach (var node in Nodes)
        {
            InitializeNodePathsForNode(node);
        }
    }

    private void InitializeNodePathsForNode(Node node)
    {
        HashSet<Point> visited = [];
        
        Queue<(Point Point, Direction IncomingDirection, int Length)> queue = new();
        queue.Enqueue((node.Point, Direction.None, 0));

        while (queue.TryDequeue(out var current))
        {
            if (visited.Contains(current.Point)) continue;
            visited.Add(current.Point); 

            if(current.Point != node.Point)
            {
                Tile tile = GetTile(current.Point);

                if(tile != Tile.Ground)
                {
                    Node connected = Nodes.Single(n => n.Value == (char)tile);
                    if(node.Connections.TryGetValue(connected, out int length))
                    {
                        node.Connections[connected] = Math.Min(current.Length, length);
                    }
                    else
                    {
                        node.Connections[connected] = current.Length;
                    }

                    continue;
                }
            }

            var nextDirections = GetPossibleNextDirections(current.Point, current.IncomingDirection);

            foreach (var nextDirection in nextDirections)
            {
                queue.Enqueue((current.Point.GetNeightboringPoint(nextDirection), nextDirection, current.Length + 1));
            }
        }
    }

    public int GetShortestPathFromZeroToAllNodes(bool endAtZero)
    {
        PriorityQueue<(Node Node, HashSet<Node> SeenNodes, string Path, int Length), int> queue = new();
        Node zero = Nodes.Single(n => n.Value == '0');
        queue.Enqueue((zero, [zero], "0", 0), 0);

        while(queue.TryDequeue(out var current, out var _))
        {
            // We trim the tree search by detecting a pattern in the form
            // ABAB (unnecessary loop) and then stop this branch.
            if(current.Path.Length >= 4)
            {
                if (current.Path[^1] == current.Path[^3] && current.Path[^2] == current.Path[^4])
                {
                    continue;
                }
            }

            if (current.SeenNodes.Count == Nodes.Count)
            {
                if (!endAtZero)
                {
                    return current.Length;
                }
                if (current.Node == zero)
                {
                    return current.Length;
                }
            }

            foreach(var connection in current.Node.Connections)
            {
                queue.Enqueue((connection.Key, [..current.SeenNodes, connection.Key], current.Path + connection.Key.Value, current.Length + connection.Value), current.Length + connection.Value);
            }
        }

        return 0;
    }

    public List<Node> Nodes = [];

    Direction[] GetPossibleNextDirections(Point point, Direction incomingDirection)
    {
        if (!CanEnterPoint(point)) return [];

        List<Direction> nextDirections = new();

        foreach (var direction in AllDirections())
        {
            if (direction == incomingDirection.Opposite()) continue;

            Point nextPoint = point.GetNeightboringPoint(direction);
            if (CanEnterPoint(nextPoint))
            {
                nextDirections.Add(direction);
            }
        }

        return nextDirections.ToArray();
    }

    bool CanEnterPoint(Point point)
    {
        if (!IsInGrid(point)) return false;
        return GetTile(point) != Tile.Wall;
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

public record Node
{
    public char Value;
    public Point Point;

    public Dictionary<Node, int> Connections = [];
}

public record struct Point(int X, int Y)
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

public enum Direction
{
    None,
    South,
    West,
    North,
    East
}

public enum Tile
{
    Ground = '.',
    Wall = '#',
}


public static class Extensions
{
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
}
