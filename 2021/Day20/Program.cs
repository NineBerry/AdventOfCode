// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day20\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day20\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Solve(input, 2));
    Console.WriteLine("Part 2: " + Solve(input, 50));
    Console.ReadLine();
}

long Solve(string[] input, int steps)
{
    Grid grid = new Grid(input);

    foreach(var _ in Enumerable.Range(1, steps))
    {
        grid.RunGameOfImageEnhancement();
    }

    return grid.CountLights();
}

class Grid
{
    private string Algorithm;
    private bool InfinityIsLight = false;

    public Grid(string[] lines)
    {
        Algorithm = lines[0];
        InitLightsAndDarks(lines.Skip(2).ToArray());
    }

    private void InitLightsAndDarks(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                Point point = new Point { X = x, Y = y };
                
                if (tile == '#')
                {
                    KnownLights.Add(point);
                }
                else
                {
                    KnownDarks.Add(point);
                }
            }
        }
    }

    private HashSet<Point> KnownLights = [];
    private HashSet<Point> KnownDarks = [];

    public void RunGameOfImageEnhancement()
    {
        var oldLights = new HashSet<Point>(KnownLights);
        var oldDarks = new HashSet<Point>(KnownDarks);
        var allPointsToConsider = oldLights.Union(oldDarks).SelectMany(a => a.GetSuroundingBoxPoints()).ToHashSet();
        KnownLights.Clear();
        KnownDarks.Clear();

        foreach (var point in allPointsToConsider)
        {
            int numberOfThePoint = GetNumberOfThePoint(point, oldLights, oldDarks);
            if (Algorithm[numberOfThePoint] == '#')
            {
                KnownLights.Add(point);
            }
            else
            {
                KnownDarks.Add(point);
            }
        }

        int infinityBoxesNumber = InfinityIsLight ? 0b_1111_1111_1 : 0b_0000_0000_0;
        InfinityIsLight = Algorithm[infinityBoxesNumber] == '#';
    }

    private int GetNumberOfThePoint(Point center, HashSet<Point> oldLights, HashSet<Point> oldDarks)
    {
        int result = 0;

        foreach (var point in center.GetSuroundingBoxPoints()) 
        {
            result <<= 1;
            
            bool pointIsLight;

            if (oldLights.Contains(point))
            {
                pointIsLight = true;
            }
            else if (oldDarks.Contains(point))
            {
                pointIsLight = false;
            }
            else
            {
                pointIsLight = InfinityIsLight;
            }

            result += pointIsLight ? 1 : 0;
        }

        return result;
    }

    public int CountLights()
    {
        if (InfinityIsLight) throw new ApplicationException("Inifity");
        return KnownLights.Count;
    }
}

public record class Point 
{
    public int X;
    public int Y;

    public Point[] GetSuroundingBoxPoints()
    {
        List<Point> neighbors = new();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                neighbors.Add(new Point { X = X + x, Y = Y + y });
            }
        }
        return neighbors.ToArray();
    }
}