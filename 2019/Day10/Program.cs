// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day10\Full.txt";
#endif

    Grid grid = new Grid(File.ReadAllLines(fileName));
    var monitoringStation =
        grid
        .GetAllPossibleMonitoringStations()
        .MaxBy(m => m.MonitoredSatellites.Count)!;

    Console.WriteLine("Part 1: " + Part1(monitoringStation));
    Console.WriteLine("Part 2: " + Part2(grid, monitoringStation));
    Console.ReadLine();
}

long Part1(MonitoringStation monitoringStation)
{
    return monitoringStation.MonitoredSatellites.Count;
}

long Part2(Grid grid, MonitoringStation monitoringStation)
{
    var vaporized = grid.VaporizeSatellites(monitoringStation);
    var vaporized200 = vaporized[199];
    return vaporized200.X * 100 + vaporized200.Y;
}

class Grid
{
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
                if (tile == '#') Satellites.Add(new Point(x, y));
            }
        }
    }

    public MonitoringStation[] GetAllPossibleMonitoringStations()
    {
        List<MonitoringStation> result =  [];

        foreach(var satellite in Satellites)
        {
            result.Add(CheckMonitoringStation(satellite));
        }

        return result.ToArray();
    }

    public MonitoringStation CheckMonitoringStation(Point point)
    {
        MonitoringStation monitoringStation = new MonitoringStation { Point = point};
        
        HashSet<Point> invisibleSpots = new HashSet<Point>();

        for(int yOffset = 0; yOffset < Height; yOffset++)
        {
            for(int xOffset = 0; xOffset < Width; xOffset++)
            {
                HashSet<Point> pointsToCheck = [
                    new Point(point.X - xOffset, point.Y - yOffset),
                    new Point(point.X + xOffset, point.Y - yOffset),
                    new Point(point.X - xOffset, point.Y + yOffset),
                    new Point(point.X + xOffset, point.Y + yOffset),
                ];

                foreach(var pointToCheck in pointsToCheck)
                {
                    if (pointToCheck == point) continue;
                    if (!IsInGrid(pointToCheck)) continue;
                    if (!Satellites.Contains(pointToCheck)) continue;
                    if (invisibleSpots.Contains(pointToCheck))
                    {
                        monitoringStation.MissingSatellites.Add(pointToCheck);
                        continue;
                    }

                    monitoringStation.MonitoredSatellites.Add(pointToCheck);
                    Point vector = MarkInvisibleSpots(point, pointToCheck, invisibleSpots);
                    monitoringStation.StandardVectorsUsed.Add(vector);
                }
            }
        }

        return monitoringStation;
    }

    private Point MarkInvisibleSpots(Point station, Point satellite, HashSet<Point> invisibleSpots)
    {
        Point vector = station.GetStandardVectorTo(satellite);

        Point point = satellite.AddVector(vector);
        while (IsInGrid(point))
        {
            invisibleSpots.Add(point);
            point = point.AddVector(vector);
        }

        return vector;
    }

    public Point[] VaporizeSatellites(MonitoringStation monitoringStation)
    {
        // Idea: Store Standard vectors with Monitoring Station, sort by some criteria to be determined
        // and then rotate laser by rotating through that list of standard vectors
        
        List<Point> vaporized = [];
        HashSet<Point>  remainingSatellites = monitoringStation.MissingSatellites.Union(monitoringStation.MonitoredSatellites).ToHashSet();

        VectorComparer vectorComparer = new VectorComparer();
        List<Point> vectors = monitoringStation.StandardVectorsUsed.Order(vectorComparer).ToList();
        int vectorIndex = vectors.IndexOf(new Point(0, -1)); // Start facing north

        while (remainingSatellites.Any())
        {
            Point vector = vectors[vectorIndex];
            Point? nextVaporized = GetNextVaporizedSatellite(monitoringStation.Point, vector, remainingSatellites);

            if (nextVaporized.HasValue)
            {
                vaporized.Add(nextVaporized.Value);
                remainingSatellites.Remove(nextVaporized.Value);
                vectorIndex++;
            }
            else
            {
                vectors.Remove(vector);
            }
            if (vectorIndex >= vectors.Count) vectorIndex = 0;
        }

        return vaporized.ToArray();
    }

    private Point? GetNextVaporizedSatellite(Point from, Point vector, HashSet<Point> remainingSatellites)
    {
        Point point = from.AddVector(vector);

        while (IsInGrid(point)) 
        {
            if (remainingSatellites.Contains(point)) return point;
            point = point.AddVector(vector);
        }

        return null;        
    }

    private HashSet<Point> Satellites = new();

    public readonly int Height;
    public readonly int Width;

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
}

class MonitoringStation
{
    public Point Point;
    public HashSet<Point> MonitoredSatellites = [];
    public HashSet<Point> MissingSatellites = [];
    public HashSet<Point> StandardVectorsUsed = [];
}

record struct Point(int X, int Y)
{
    public Point AddVector(Point vector)
    {
        return new Point(X + vector.X, Y + vector.Y);
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

class VectorComparer : IComparer<Point>
{
    public int Compare(Point a, Point b)
    {
        // Turn vector into angle, then sort
        double aVal = Math.Atan2(a.Y, a.X);
        double bVal = Math.Atan2(b.Y, b.X);
        return aVal.CompareTo(bVal);
    }
}