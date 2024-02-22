// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day18\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day18\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Grid grid = new Grid(input);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetShortestPath();
}

public class Grid
{
    public Grid(string[] input)
    {
        Width = input.First().Length;
        Height = input.Length;

        InitializeMaze(input);

        KeysCount = KeyPositions.Count;
    }

    private void InitializeMaze(string[] input)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                char ch = input[y][x];

                if (ch == '@')
                {
                    Start = point;
                }
                else if (ch == '#')
                {
                    Walls.Add(point);   
                }
                else if (IsKey(ch))
                {
                    KeyPositions[point] = ch;
                }
                else if (IsDoor(ch))
                {
                    DoorPositions[point] = ch;
                }
            }
        }
    }

    public int Height;
    public int Width;
    public Point Start = new Point(0, 0);

    private HashSet<Point> Walls = [];
    private Dictionary<Point, char> KeyPositions = [];
    private Dictionary<Point, char> DoorPositions = [];
    private int KeysCount;

    public int GetShortestPath()
    {
        HashSet<(Point Point, string FoundKeys)> visited = [];
        PriorityQueue<(Point Point, int Steps, string FoundKeys), int> todos = new();
        todos.Enqueue((Start, 0, ""), 0);

        while (todos.TryDequeue(out var current, out _))
        {
            if (visited.Contains((current.Point, current.FoundKeys))) continue;
            visited.Add((current.Point, current.FoundKeys));

            if(KeyPositions.TryGetValue(current.Point, out var key))
            {
                if (!current.FoundKeys.Contains(key))
                {
                    current.FoundKeys = new string((current.FoundKeys + key).Order().ToArray());
                }
            }

            if (current.FoundKeys.Length == KeysCount)
            {
                return current.Steps;
            }

            foreach (var neighbor in current.Point.GetAllNeighboringPoints())
            {
                if (CanEnter(neighbor, current.FoundKeys))
                {
                    todos.Enqueue((neighbor, current.Steps + 1, current.FoundKeys), current.Steps + 1);
                }
            }
        }

        return int.MaxValue;
    }

    private bool CanEnter(Point point, string foundKeys)
    {
        if(Walls.Contains(point)) return false;

        if(DoorPositions.TryGetValue(point, out var door))
        {
            char key = char.ToLower(door);
            return foundKeys.Contains(key);
        }

        return true;
    }

    private static bool IsKey(char ch)
    {
        return ch is >= 'a' and <= 'z';
    }
    private static bool IsDoor(char ch)
    {
        return ch is >= 'A' and <= 'Z';
    }
}

public record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.West => this with { X = this.X - 1 },
            Direction.East => this with { X = this.X + 1 },

            Direction.South => this with { X = this.X, Y = this.Y + 1 },
            Direction.North => this with { X = this.X, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeighboringPoints()
    {
        return AllDirections.Select(GetNeightboringPoint).ToArray();
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

public enum Direction
{
    West,
    East,
    North,
    South,
}
