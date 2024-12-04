// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day04\Full.txt";
#endif
    var lines = File.ReadAllLines(fileName);
    Grid grid = new Grid(lines);

    Console.WriteLine("Part 1: " + grid.CountXMAS());
    Console.WriteLine("Part 2: " + grid.CountCrossedMAS());
    Console.ReadLine();
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
                Point point = new Point(x, y);  
                char tile = lines[y][x];
                Cells[point] = tile;
            }
        }
    }

    public long CountXMAS()
    {
        return Cells.Sum(c => CountWordFrom(c.Key, "XMAS"));
    }

    private long CountWordFrom(Point point, string word)
    {
        return AllDirections.Sum(d => CountWord(point, word, d, 0));
    }

    private long CountWord(Point point, string word, Direction direction, int currentWordIndex)
    {
        if (GetCellContentOrNull(point) != word[currentWordIndex]) return 0;

        if (currentWordIndex == word.Length - 1) return 1;

        return CountWord(point.GetNeightboringPoint(direction), word, direction, currentWordIndex + 1);
    }

    public long CountCrossedMAS()
    {
        return Cells.Count(cell => IsCrossedMAS(cell.Key));
    }

    private bool IsCrossedMAS(Point centerPoint)
    {
        if (Cells[centerPoint] != 'A') return false;
        
        return 
            CheckAxisMS(centerPoint, Direction.NorthWest, Direction.SouthEast) 
            && CheckAxisMS(centerPoint, Direction.NorthEast, Direction.SouthWest);
    }

    private bool CheckAxisMS(Point centerPoint, Direction firstDirection, Direction secondDirection)
    {
        string axisEnds = ""
            + GetCellContentOrNull(centerPoint.GetNeightboringPoint(firstDirection))
            + GetCellContentOrNull(centerPoint.GetNeightboringPoint(secondDirection));

        return axisEnds is "MS" or "SM";
    }

    private char GetCellContentOrNull(Point point)
    {
        if (!IsInGrid(point)) return '\0';
        return Cells[point];
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    private Dictionary<Point, char> Cells = [];

    private readonly int Height;
    private readonly int Width;

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


