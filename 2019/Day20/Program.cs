// #define Sample

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day20\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day20\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day20\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day20\Full.txt";
#endif

    Grid gridPart1 = new Grid(File.ReadAllLines(fileNamePart1), allowMultiLevel: false);
    Console.WriteLine("Part 1: " + gridPart1.GetShortestPath());

    Grid gridPart2 = new Grid(File.ReadAllLines(fileNamePart2), allowMultiLevel: true);
    Console.WriteLine("Part 2: " + gridPart2.GetShortestPath());

    Console.ReadLine();
}

public class Grid
{
    public Grid(string[] input, bool allowMultiLevel)
    {
        AllowMultiLevel = allowMultiLevel;
        Width = input.First().Length;
        Height = input.Length;

        InitializeMaze(input);
    }

    private char GetTile(string[] input, Point point)
    {
        if (!IsInGrid(point)) return ' ';
        return input[point.Y][point.X];
    }

    private void InitializeMaze(string[] input)
    {
        List<(Point Point, Direction Direction, string Name, int LevelModifier)> portalEnds = [];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                char ch = GetTile(input, point);

                if (ch == '#')
                {
                    Walls.Add(point);
                }
                
                if (IsLetter(ch))
                {
                    char left = GetTile(input, point.GetNeightboringPoint(Direction.West));
                    char right = GetTile(input, point.GetNeightboringPoint(Direction.East));
                    char top = GetTile(input, point.GetNeightboringPoint(Direction.North));
                    char bottom = GetTile(input, point.GetNeightboringPoint(Direction.South));

                    if (IsLetter(left) && right == '.')
                    {
                        string name = "" + left + ch;
                        int levelModifier = x == 1 ? -1 : +1;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.East), Direction.West, name, levelModifier));
                        Walls.Add(point);
                    }
                    else if (IsLetter(right) && left == '.')
                    {
                        string name = "" + ch + right;
                        int levelModifier = x == Width - 2 ? -1 : +1;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.West), Direction.East, name, levelModifier));
                        Walls.Add(point);
                    }
                    else if (IsLetter(top) && bottom == '.')
                    {
                        string name = "" + top + ch;
                        int levelModifier = y == 1 ? -1 : +1;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.South), Direction.North, name, levelModifier));
                        Walls.Add(point);
                    }
                    else if (IsLetter(bottom) && top == '.')
                    {
                        string name = "" + ch + bottom;
                        int levelModifier = y == Height - 2 ? -1 : +1;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.North), Direction.South, name, levelModifier));
                        Walls.Add(point);
                    }
                }
            }
        }

        Start = portalEnds.Where(e => e.Name == "AA").Single().Point;
        End = portalEnds.Where(e => e.Name == "ZZ").Single().Point;

        portalEnds.RemoveAll(p => p.Name is "AA" or "ZZ");

        foreach (var portalEnd in portalEnds) 
        {
            Portals.Add(
                (portalEnd.Point, portalEnd.Direction), 
                portalEnds
                .Where(p => (p.Name == portalEnd.Name) && p.Point != portalEnd.Point)
                .Select(p => (p.Point, -p.LevelModifier))
                .Single());
        }
    }

    public bool AllowMultiLevel;
    public int Height;
    public int Width;
    public Point Start = new Point(0, 0);
    public Point End = new Point(0, 0);

    private HashSet<Point> Walls = [];
    private Dictionary<(Point Point, Direction Direction), (Point Point, int LevelModifier)> Portals = [];

    public int GetShortestPath()
    {
        HashSet<PointWithLevel> visited = [];
        PriorityQueue<(PointWithLevel PointWithLevel, int Steps), int> todos = new();
        todos.Enqueue((new PointWithLevel(Start, 0), 0), 0);

        while (todos.TryDequeue(out var current, out _))
        {
            if (visited.Contains(current.PointWithLevel)) continue;
            visited.Add(current.PointWithLevel);

            if (current.PointWithLevel.Point == End && current.PointWithLevel.Level == 0)
            {
                return current.Steps;
            }

            foreach (var neighbor in GetAllNeighboringPoints(current.PointWithLevel, Portals))
            {
                if (CanEnter(neighbor))
                {
                    if (!visited.Contains(neighbor))
                    {
                        todos.Enqueue((neighbor, current.Steps + 1), current.Steps + 1);
                    }
                }
            }
        }

        return int.MaxValue;
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    private PointWithLevel[] GetAllNeighboringPoints(PointWithLevel pointWithLevel, 
        Dictionary<(Point Point, Direction Direction), (Point Point, int LevelModifier)> portals)
    {
        List<PointWithLevel> result = [];
        foreach (var direction in Point.AllDirections)
        {
            if (portals.TryGetValue((pointWithLevel.Point, direction), out var portalInformation))
            {
                int levelModifier = AllowMultiLevel ? portalInformation.LevelModifier : 0;
                result.Add(new PointWithLevel(portalInformation.Point, pointWithLevel.Level + levelModifier));
            }
            else
            {
                result.Add(new PointWithLevel(pointWithLevel.Point.GetNeightboringPoint(direction), pointWithLevel.Level));
            }
        }
        return result.ToArray();
    }


    private bool CanEnter(PointWithLevel pointWithLevel)
    {
        if (Walls.Contains(pointWithLevel.Point)) return false;
        if (pointWithLevel.Level < 0) return false;
        if (pointWithLevel.Point == Start && pointWithLevel.Level != 0) return false;
        if (pointWithLevel.Point == End && pointWithLevel.Level != 0) return false;

        return true;
    }

    private static bool IsLetter(char ch)
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

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

public record PointWithLevel(Point Point, int Level);

public enum Direction
{
    West,
    East,
    North,
    South,
}


