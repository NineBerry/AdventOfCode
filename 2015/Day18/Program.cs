// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day18\Sample.txt";
    int part1Steps = 4;
    int part2Steps = 5;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day18\Full.txt";
    int part1Steps = 100;
    int part2Steps = 100;
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input, part1Steps));
    Console.WriteLine("Part 2: " + Part2(input, part2Steps));
    Console.ReadLine();
}

long Part1(string[] input, int steps)
{
    Grid grid = new Grid(input, cornersAreBroken: false);
    
    foreach (var step in Enumerable.Range(1, steps))
    {
        grid.RunGameOfLife();
    }

    return grid.LightCount;
}

long Part2(string[] input, int steps)
{
    Grid grid = new Grid(input, cornersAreBroken: true);

    foreach (var step in Enumerable.Range(1, steps))
    {
        grid.RunGameOfLife();
    }

    return grid.LightCount;
}

class Grid
{
    private bool cornersAreBroken = false;

    public Grid(string[] lines, bool cornersAreBroken)
    {
        this.cornersAreBroken = cornersAreBroken;
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == '#') Lights.Add(new Point(x, y));
            }
        }

        AddCornersIfBroken();
    }

    private void AddCornersIfBroken()
    {
        if (cornersAreBroken) 
        {
            Lights.Add(new Point(0, 0));
            Lights.Add(new Point(0, Height -1));
            Lights.Add(new Point(Width - 1, 0));
            Lights.Add(new Point(Width - 1, Height - 1));
        }
    }

    private HashSet<Point> Lights = new();

    public readonly int Height;
    public readonly int Width;

    public void RunGameOfLife()
    {
        var oldLights = new HashSet<Point>(Lights);
        Lights.Clear();

        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                Point point = new Point(x, y);

                int count = CountNeighborLights(point, oldLights);

                bool turnOn = ((oldLights.Contains(point)) && (count == 2)) || count == 3;
                if(turnOn) Lights.Add(point);
            }
        }

        AddCornersIfBroken();
    }

    public int LightCount => Lights.Count;

    private int CountNeighborLights(Point point, HashSet<Point> lights)
    {
        return
            AllDirections()
            .Select(point.GetNeightboringPoint)
            .Count(lights.Contains);
    }

    private char[] emptyMap = null;
    public override string ToString()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat('.', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var light in Lights)
        {
            // +1 for line break
            map[light.Y * (Width + 1) + light.X] = '#';
        }

        return new string(map);
    }

    private static Direction[] AllDirections() => [
        Direction.East, 
        Direction.North, 
        Direction.South, 
        Direction.West,
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.NorthWest,
        Direction.SouthWest
    ];
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