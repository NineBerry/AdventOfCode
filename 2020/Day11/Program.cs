// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day11\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] input)
{
    return Solve(input, g => g.RunGameOfSeating(4, false));
}

long Part2(string[] input) 
{
    return Solve(input, g => g.RunGameOfSeating(5, true));
}

long Solve(string[] input, Action<Grid> performStep)
{
    HashSet<string> seen = [];

    Grid grid = new Grid(input);
    string representation = grid.ToString();

    while (!seen.Contains(representation))
    {
        performStep(grid);

        seen.Add(representation);
        representation = grid.ToString();
    }

    return grid.OccupiedCount;
}


class Grid
{
    public readonly int Height;
    public readonly int Width;

    public Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        InitSeats(lines);
    }

    private void InitSeats(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == 'L') Empty.Add(new Point(x, y));
                if (tile == '#') Occupied.Add(new Point(x, y));
                if (tile == '.') Floor.Add(new Point(x, y));
            }
        }
    }

    private HashSet<Point> Empty = new();
    private HashSet<Point> Occupied = new();
    private HashSet<Point> Floor = new();

    public void RunGameOfSeating(int emptyWhenNeightborsOccupied, bool seeFurther)
    {
        var oldEmpty = new HashSet<Point>(Empty);
        var oldOccupied = new HashSet<Point>(Occupied);
        Empty.Clear();
        Occupied.Clear();

        foreach (var empty in oldEmpty)
        {
            if (CountNeighborOfType(empty, oldOccupied, seeFurther) == 0)
            {
                Occupied.Add(empty);
            }
            else
            {
                Empty.Add(empty);
            }
        }

        foreach (var occupied in oldOccupied)
        {
            if (CountNeighborOfType(occupied, oldOccupied, seeFurther) >= emptyWhenNeightborsOccupied)
            {
                Empty.Add(occupied);
            }
            else
            {
                Occupied.Add(occupied);
            }
        }
    }

    public int OccupiedCount => Occupied.Count;

    private int CountNeighborOfType(Point point, HashSet<Point> objectsOfType, bool seeFurther)
    {
        int sum = 0;

        foreach(var direction in AllDirections)
        {
            Point neighbor = GetFirstSeat(point, direction, seeFurther);
            if(objectsOfType.Contains(neighbor)) sum++;
        }

        return sum;
    }

    private Point GetFirstSeat(Point from, Direction direction, bool seeFurther)
    {
        do
        {
            from = from.GetNeightboringPoint(direction);
        } while (Floor.Contains(from) && seeFurther);

        return from;
    }

    private char[]? emptyMap = null;
    public override string ToString()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat('.', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var obj in Empty)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = 'L';
        }

        foreach (var obj in Occupied)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = '#';
        }

        return new string(map);
    }

    private static Direction[] AllDirections => [
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