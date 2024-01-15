// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day25\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    Point[] points = lines.Select((s, i) => new Point(s, i)).ToArray();

    Console.WriteLine("Part 1: " + Part1(points));
    Console.ReadLine();
}

int Part1(Point[] points)
{
    for(int i = 0; i < points.Length - 1; i++)
    {
        for (int j = i + 1; j < points.Length; j++)
        {
            Point p1 = points[i];
            Point p2 = points[j];

            if((p1.Constellation != p2.Constellation) && (p1.ManhattanDistance(p2) <= 3))
            {
                ReplaceConstellation(points, p2.Constellation, p1.Constellation);
            }
        }
    }

    return points.Select(p => p.Constellation).Distinct().Count();
}

void ReplaceConstellation(Point[] points, int from, int to)
{
    foreach (Point p in points)
    {
        if (p.Constellation == from) p.Constellation = to;
    }
}

public record Point
{
    public Point(string line, int constellation)
    {
        int[] values = Regex.Matches(line, "-?\\d+").Select(m => int.Parse(m.Value)).ToArray();

        X = values[0];
        Y = values[1];
        Z = values[2];
        T = values[3];
        
        Constellation = constellation;
    }

    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    public readonly int T;

    public int ManhattanDistance(Point other)
    {
        return
            Math.Abs(other.X - X) +
            Math.Abs(other.Y - Y) +
            Math.Abs(other.Z - Z) +
            Math.Abs(other.T - T);
    }

    public int Constellation { get; set; }
}