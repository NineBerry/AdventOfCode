// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day17\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day17\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Solve<Point3>(input));
    Console.WriteLine("Part 2: " + Solve<Point4>(input));
    Console.ReadLine();
}

long Solve<TPoint>(string[] lines) where TPoint : Point<TPoint>, new()
{
    Grid<TPoint> grid = new(lines);
    foreach (var _ in Enumerable.Range(1, 6))
    {
        grid.RunGameOfCubes();
    }

    return grid.ActiveCount;
}


class Grid<TPoint> where TPoint : Point<TPoint>, new()
{
    public Grid(string[] lines)
    {
        InitCubes(lines);
    }

    private void InitCubes(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == '#') Active.Add(new TPoint { X = x, Y = y });
            }
        }
    }

    private HashSet<TPoint> Active = new();

    public void RunGameOfCubes()
    {
        var oldActive = new HashSet<TPoint>(Active);
        var oldInactive = oldActive.SelectMany(a => a.GetAllNeightboringPoints()).Except(oldActive).ToHashSet();
        Active.Clear();

        foreach (var active in oldActive)
        {
            if (CountNeighborOfType(active, oldActive) is 2 or 3)
            {
                Active.Add(active);
            }
        }

        foreach (var inactive in oldInactive)
        {
            if (CountNeighborOfType(inactive, oldActive) == 3)
            {
                Active.Add(inactive);
            }
        }
    }

    public int ActiveCount => Active.Count;

    private int CountNeighborOfType(TPoint point, HashSet<TPoint> objectsOfType)
    {
        return point.GetAllNeightboringPoints().Intersect(objectsOfType).Count();
    }
}

record class Point<TPoint> where TPoint : Point<TPoint>, new()
{
    public int X;
    public int Y;

    public virtual TPoint[] GetAllNeightboringPoints() => [];
}

record class Point3: Point<Point3>
{
    public int Z;

    public override Point3[] GetAllNeightboringPoints()
    {
        List<Point3> neighbors = new();
        for(int x=-1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    neighbors.Add(new Point3 { X = X + x, Y = Y + y, Z = Z + z });
                }
            }
        }
        return neighbors.ToArray();
    }
}

record class Point4 : Point<Point4>
{
    public int Z;
    public int W;

    public override Point4[] GetAllNeightboringPoints()
    {
        List<Point4> neighbors = new();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    for (int w = -1; w <= 1; w++)
                    {
                        if (x == 0 && y == 0 && z == 0 && w == 0) continue;

                        neighbors.Add(new Point4 { X = X + x, Y = Y + y, Z = Z + z, W = W + w });
                    }
                }
            }
        }
        return neighbors.ToArray();
    }
}
