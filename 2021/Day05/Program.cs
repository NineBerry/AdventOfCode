// #define Sample

using System.Text.RegularExpressions;
using PointPair = (Point P1, Point P2);

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day05\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);
    PointPair[] pairs = lines.Select(ParseLine).ToArray();

    Console.WriteLine("Part 1: " + Part1(pairs));
    Console.WriteLine("Part 2: " + Part2(pairs));
    Console.ReadLine();
}

long Part1(PointPair[] pairs)
{
    return CountOverlaps(pairs, false);
}

long Part2(PointPair[] pairs)
{
    return CountOverlaps(pairs, true);
}

long CountOverlaps(PointPair[] pairs, bool allowDiagonal)
{
    List<Point> points = [];

    foreach(var pair in pairs)
    {
        int minX = Math.Min(pair.P1.X, pair.P2.X);
        int maxX = Math.Max(pair.P1.X, pair.P2.X);
        int minY = Math.Min(pair.P1.Y, pair.P2.Y);
        int maxY = Math.Max(pair.P1.Y, pair.P2.Y);

        if ((minX == maxX) || (minY == maxY))
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    points.Add(new Point(x, y));
                }
            }
        }
        else if(allowDiagonal)
        {
            int signX = (pair.P1.X < pair.P2.X) ? 1 : -1;
            int signY = (pair.P1.Y < pair.P2.Y) ? 1 : -1;

            for (int offset = 0; offset <= (maxX - minX); offset++)
            {
                points.Add(new Point(pair.P1.X + signX * offset, pair.P1.Y + signY * offset));
            }

        }
    }

    return points.GroupBy(p => p).Where(g => g.Count() > 1).Count();
}

PointPair ParseLine(string s)
{
    int[] values = Regex.Matches(s, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    return (new Point(values[0], values[1]), new Point(values[2], values[3]));
}

record struct Point(int X, int Y);