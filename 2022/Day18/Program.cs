// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day18\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day18\Full.txt";
#endif

    Grid grid = new(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + grid.CountAllUncoveredSides());
    Console.WriteLine("Part 2: " + grid.CountAllOutsideSides());
    Console.ReadLine();
}

public class Grid
{
    public Grid(string[] lines)
    {
        AllPoints = lines.Select(Point.Factory).ToHashSet();

        MinX = AllPoints.Select(p => p.X).Min() - 1;
        MinY = AllPoints.Select(p => p.Y).Min() - 1;
        MinZ = AllPoints.Select(p => p.Z).Min() - 1;
        MaxX = AllPoints.Select(p => p.X).Max() + 1;
        MaxY = AllPoints.Select(p => p.Y).Max() + 1;
        MaxZ = AllPoints.Select(p => p.Z).Max() + 1;

        OutsidePoints = GetOutsidePoints();
    }

    private HashSet<Point> GetOutsidePoints()
    {
        HashSet<Point> result = [];

        Queue<Point> queue = [];
        queue.Enqueue(new Point { X = MinX, Y = MinY, Z = MinZ});

        while(queue.TryDequeue(out var point))
        {
            if (!IsInGrid(point)) continue;
            if (result.Contains(point)) continue;
            if (AllPoints.Contains(point)) continue;

            result.Add(point);  

            foreach(var neighbor in point.GetAllNeightboringPoints())
            {
                queue.Enqueue(neighbor);
            }
        }

        return result;
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= MinX
            && point.X <= MaxX
            && point.Y >= MinY
            && point.Y <= MaxY
            && point.Z >= MinZ
            && point.Z <= MaxZ;
    }

    HashSet<Point> AllPoints;
    HashSet<Point> OutsidePoints;

    int MinX;
    int MaxX;
    int MinY;
    int MaxY;
    int MinZ;
    int MaxZ;   

    public int CountAllUncoveredSides()
    {
        return AllPoints.Select(p => p.CountNeighborsExcept(AllPoints)).Sum();
    }

    public int CountAllOutsideSides()
    {
        return AllPoints.Select(p => p.CountNeighborsIntersect(OutsidePoints)).Sum();
    }
}


record class Point
{
    public int X;
    public int Y;
    public int Z;

    public virtual Point[] GetAllNeightboringPoints()
    {
        return
        [
            this with { X = X - 1 },
            this with { X = X + 1 },
            this with { Y = Y - 1 },
            this with { Y = Y + 1 },
            this with { Z = Z - 1 },
            this with { Z = Z + 1 },
        ];
    }

    public int CountNeighborsExcept(HashSet<Point> points)
    {
        return GetAllNeightboringPoints().Except(points).Count();
    }

    public int CountNeighborsIntersect(HashSet<Point> points)
    {
        return GetAllNeightboringPoints().Intersect(points).Count();
    }

    public static Point Factory(string s)
    {
        var values = Regex.Matches(s, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        return new Point { X = values[0], Y = values[1], Z = values[2] };
    }
}
