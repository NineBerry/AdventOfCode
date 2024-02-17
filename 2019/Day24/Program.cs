// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day24\Sample.txt";
    int part2Minutes = 10;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day24\Full.txt";
    int part2Minutes = 200;
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input, part2Minutes));
    Console.ReadLine();
}

long Part1(string[] input)
{
    HashSet<string> seen = [];

    Grid grid = new Grid(input, isInfinite: false);
    string representation = grid.ToString();

    while (!seen.Contains(representation))
    {
        grid.RunGameOfBugs();

        seen.Add(representation);
        representation = grid.ToString();
    }

    return grid.BiodiversityRating();
}

long Part2(string[] input, int minutes)
{
    Grid grid = new Grid(input, isInfinite: true);

    foreach(var _ in Enumerable.Range(1, minutes))
    {
        grid.RunGameOfBugs();
    }

    return grid.BugCount();
}

public class Grid
{
    public int Width;
    public int Height;
    private bool IsInfinite; 

    public Grid(string[] lines, bool isInfinite)
    {
        Width = lines[0].Length;
        Height = lines.Length;
        IsInfinite = isInfinite;

        InitBugs(lines);
    }

    private void InitBugs(string[] lines)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char tile = lines[y][x];

                if (tile == '#')
                {
                    Point point = new Point { X = x, Y = y };
                    Bugs.Add(point);
                }
            }
        }
    }

    private HashSet<Point> Bugs = [];

    public void RunGameOfBugs()
    {
        var oldBugs = new HashSet<Point>(Bugs);
        var oldEmpty = 
            Bugs
            .SelectMany(b => b.GetAllNeighboringPoints(this, IsInfinite))
            .Where(IsInGrid)
            .Except(Bugs)
            .ToHashSet();
        Bugs.Clear();

        foreach (var point in oldBugs)
        {
            if (CountNeighborBugs(point, oldBugs) is 1)
            {
                Bugs.Add(point);
            }
        }

        foreach (var point in oldEmpty)
        {
            if (CountNeighborBugs(point, oldBugs) is 1 or 2)
            {
                Bugs.Add(point);
            }
        }
    }

    private int CountNeighborBugs(Point point, HashSet<Point> bugs)
    {
        return
            point.GetAllNeighboringPoints(this, IsInfinite)
            .Count(bugs.Contains);
    }

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
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

        foreach (var obj in Bugs)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = '#';
        }

        return new string(map);
    }

    internal long BiodiversityRating()
    {
        long rating = 0;
        foreach(var bug in Bugs)
        {
            int position = bug.Y * Width + bug.X;
            rating += (1 << position);
        }

        return rating;
    }

    public long BugCount()
    {
        return Bugs.Count;
    }
}

public record class Point
{
    public int X;
    public int Y;
    public int Level;

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

    public Point[] GetAllNeighboringPoints(Grid grid, bool isInfinite)
    {
        if (isInfinite)
        {
            return GetAllInfiniteNeighboringPoints(grid);
        }
        else
        {
            return AllDirections.Select(GetNeightboringPoint).Where(grid.IsInGrid).ToArray();
        }
    }

    private Point[] GetAllInfiniteNeighboringPoints(Grid grid)
    {
        List<Point> result = [.. GetAllNeighboringPoints(grid, false)];

        // Remove middle because that is the other dimension
        result.RemoveAll(p => p.X == 2 && p.Y == 2);

        // Add outer dimensions
        if (X == 0) result.Add(new Point { X = 1, Y = 2, Level = Level - 1 });
        if (X == 4) result.Add(new Point { X = 3, Y = 2, Level = Level - 1 });
        if (Y == 0) result.Add(new Point { X = 2, Y = 1, Level = Level - 1 });
        if (Y == 4) result.Add(new Point { X = 2, Y = 3, Level = Level - 1 });

        // Add inner dimensions
        if (X == 1 && Y == 2)
        {
            result.Add(new Point { X = 0, Y = 0, Level = Level + 1 });
            result.Add(new Point { X = 0, Y = 1, Level = Level + 1 });
            result.Add(new Point { X = 0, Y = 2, Level = Level + 1 });
            result.Add(new Point { X = 0, Y = 3, Level = Level + 1 });
            result.Add(new Point { X = 0, Y = 4, Level = Level + 1 });
        }

        if (X == 3 && Y == 2)
        {
            result.Add(new Point { X = 4, Y = 0, Level = Level + 1 });
            result.Add(new Point { X = 4, Y = 1, Level = Level + 1 });
            result.Add(new Point { X = 4, Y = 2, Level = Level + 1 });
            result.Add(new Point { X = 4, Y = 3, Level = Level + 1 });
            result.Add(new Point { X = 4, Y = 4, Level = Level + 1 });
        }

        if (X == 2 && Y == 1)
        {
            result.Add(new Point { X = 0, Y = 0, Level = Level + 1 });
            result.Add(new Point { X = 1, Y = 0, Level = Level + 1 });
            result.Add(new Point { X = 2, Y = 0, Level = Level + 1 });
            result.Add(new Point { X = 3, Y = 0, Level = Level + 1 });
            result.Add(new Point { X = 4, Y = 0, Level = Level + 1 });
        }

        if (X == 2 && Y == 3)
        {
            result.Add(new Point { X = 0, Y = 4, Level = Level + 1 });
            result.Add(new Point { X = 1, Y = 4, Level = Level + 1 });
            result.Add(new Point { X = 2, Y = 4, Level = Level + 1 });
            result.Add(new Point { X = 3, Y = 4, Level = Level + 1 });
            result.Add(new Point { X = 4, Y = 4, Level = Level + 1 });
        }

        return result.ToArray();
    }

    private static Direction[] AllDirections = [
        Direction.East,
        Direction.North,
        Direction.South,
        Direction.West
    ];

}

public enum Direction
{
    South,
    West,
    North,
    East,
}