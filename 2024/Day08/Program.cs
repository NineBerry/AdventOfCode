// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day08\Full.txt";
#endif

    Grid grid = new(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetAntipodeLocations().Count;
}

long Part2(Grid grid)
{
    return grid.GetAntipodeLocationsWithResonantHarmonics().Count;
}

class Grid
{
    private List<Satellite> Satellites = [];
    private readonly int Height;
    private readonly int Width;

    public Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        InitSatellites(lines);
    }

    private void InitSatellites(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile != '.')
                {
                    Satellites.Add(new Satellite(tile, new Point(x, y)));
                }
            }
        }
    }

    public HashSet<Point> GetAntipodeLocations()
    {
        HashSet<Point> result = [];

        foreach (var first in Satellites)
        {
            foreach (var second in Satellites)
            {
                if (first == second) continue;
                if (first.Frequency != second.Frequency) continue;

                var differenceVector = first.Point.GetVectorTo(second.Point);
                Point antipode = second.Point.AddVector(differenceVector);

                if (IsInGrid(antipode)) result.Add(antipode);
            }
        }

        return result;
    }

    public HashSet<Point> GetAntipodeLocationsWithResonantHarmonics()
    {
        HashSet<Point> result = [];

        foreach (var first in Satellites)
        {
            foreach (var second in Satellites)
            {
                if(first == second) continue;
                if(first.Frequency != second.Frequency) continue;

                var differenceVector = first.Point.GetStandardVectorTo(second.Point);

                Point antipode = first.Point;
                while (IsInGrid(antipode))
                {
                    if (IsInGrid(antipode)) result.Add(antipode);
                    antipode = antipode.AddVector(differenceVector);
                }
            }
        }

        return result;
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


record Satellite(char Frequency, Point Point);

record Point(int X, int Y)
{
    public Point AddVector(Point vector)
    {
        return new Point(X + vector.X, Y + vector.Y);
    }

    public Point GetVectorTo(Point to)
    {
        int x = to.X - X;
        int y = to.Y - Y;

        return new Point(x, y);
    }

    public Point GetStandardVectorTo(Point to)
    {
        int x = to.X - X;
        int y = to.Y - Y;

        int xSign = Math.Sign(x);
        int ySign = Math.Sign(y);
        x = Math.Abs(x);
        y = Math.Abs(y);

        if (x == 0)
        {
            y = 1;
        }
        else if (y == 0)
        {
            x = 1;
        }
        else if (x == y)
        {
            x = 1; y = 1;
        }
        else
        {
            int gcd = GCD(x, y);
            if (gcd > 1)
            {
                x /= gcd;
                y /= gcd;
            }
        }

        return new Point(xSign * x, ySign * y);
    }

    private int GCD(int a, int b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}