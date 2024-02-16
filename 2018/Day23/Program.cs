#define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2018\Day23\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2018\Day23\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2018\Day23\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2018\Day23\Full.txt";
#endif

    Nanobot[] nanobotsPart1 = File.ReadAllLines(fileNamePart1).Select(l => new Nanobot(l)).ToArray();
    Console.WriteLine("Part 1: " + Part1(nanobotsPart1));

    Nanobot[] nanobotsPart2 = File.ReadAllLines(fileNamePart2).Select(l => new Nanobot(l)).ToArray();
    Console.WriteLine("Part 1: " + Part2(nanobotsPart2));
    Console.ReadLine();
}

long Part1(Nanobot[] nanobots)
{
    var botWithLargestRadius = nanobots.MaxBy(b => b.SignalRadius)!;
    return nanobots.Count(botWithLargestRadius.IsInRange);
}

long Part2(Nanobot[] nanobots)
{
    // Idea: Separate for X, Y, Z coordinates:
    // build ranges that are in reach of nanobots.
    // When combining ranges, extract the overlapping
    // parts into new ranges.
    // For each range, keep note of how many bots are in reach.
    // Then take the ranges with mit nanobots in range for each
    // coordinate, select the value closest to 0 for that coordinate
    
    // Hm, maybe separate does not work, but we need to consider spheres?
    
    return 0;
}


public record class Nanobot
{
    public Nanobot(string line)
    {
        int[] values = Regex.Matches(line, "-?\\d+").Select(m => int.Parse(m.Value)).ToArray();
        Point = new Point(values[0], values[1], values[2]);
        SignalRadius = values[3];
    }

    public int SignalRadius;
    public Point Point;

    public bool IsInRange(Nanobot other)
    {
        return Point.ManhattanDistance(other.Point) <= SignalRadius;
    }
}

public record Point(int X, int Y, int Z)
{
    public int ManhattanDistance(Point other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
    }
}
