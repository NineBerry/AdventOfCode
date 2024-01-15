// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day18\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day18\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input, 10));
    Console.WriteLine("Part 2: " + Part2(input, 1_000_000_000));
    Console.ReadLine();
}

long Part1(string[] input, int steps)
{
    Grid grid = new Grid(input);

    foreach (var step in Enumerable.Range(1, steps))
    {
        grid.RunGameOfForrestLife();
    }

    return grid.LumberYardsCount * grid.WoodedAreasCount;
}

long Part2(string[] input, int steps)
{
    Dictionary<string, int> cache = [];

    Grid grid = new Grid(input);

    bool cacheHit = false;
    int step = 1;
    while(step <= steps)
    {
        grid.RunGameOfForrestLife();
        step++;

        if (!cacheHit)
        {
            string asString = grid.ToString();
            if (cache.TryGetValue(asString, out int previousStep))
            {
                cacheHit = true;

                int cycle = step - previousStep;
                int stepsLeft = steps - step;
                int cyclesToSkip = stepsLeft / cycle;
                step += cycle * cyclesToSkip;
            }
            else
            {
                cache.Add(asString, step);
            }
        }
    }

    return grid.LumberYardsCount * grid.WoodedAreasCount;
}

class Grid
{
    public Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == '#') LumberYards.Add(new Point(x, y));
                if (tile == '|') WoodedAreas.Add(new Point(x, y));
            }
        }
    }

    private HashSet<Point> WoodedAreas = new();
    private HashSet<Point> LumberYards = new();

    public readonly int Height;
    public readonly int Width;

    public void RunGameOfForrestLife()
    {
        var oldLumberYards = new HashSet<Point>(LumberYards);
        var oldWoodenAreas = new HashSet<Point>(WoodedAreas);
        LumberYards.Clear();
        WoodedAreas.Clear();

        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                Point point = new Point(x, y);

                int countLumberYards = CountNeighborOfType(point, oldLumberYards);
                int countWoodenAreas = CountNeighborOfType(point, oldWoodenAreas);

                if (oldWoodenAreas.Contains(point))
                {
                    // Is Wooden Area
                    if(countLumberYards >= 3)
                    {
                        LumberYards.Add(point);
                    }
                    else
                    {
                        WoodedAreas.Add(point);
                    }
                }
                else if(oldLumberYards.Contains(point))
                {
                    // Is Lumber Yard
                    if (countLumberYards >= 1 && countWoodenAreas >= 1)
                    {
                        LumberYards.Add(point);
                    }
                    else
                    {
                        // Becomes open
                    }
                }
                else
                {
                    // Is Open
                    if (countWoodenAreas >= 3)
                    {
                        WoodedAreas.Add(point);
                    }
                }
            }
        }
    }

    public int WoodedAreasCount => WoodedAreas.Count;
    public int LumberYardsCount => LumberYards.Count;

    private int CountNeighborOfType(Point point, HashSet<Point> objectsOfType)
    {
        return
            AllDirections()
            .Select(point.GetNeightboringPoint)
            .Count(objectsOfType.Contains);
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

        foreach (var obj in WoodedAreas)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = '|';
        }

        foreach (var obj in LumberYards)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = '#';
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