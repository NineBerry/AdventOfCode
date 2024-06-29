// #define Sample

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
    Console.WriteLine("Part 2: " + Part2(nanobotsPart2));
    Console.ReadLine();
}

long Part1(Nanobot[] nanobots)
{
    var botWithLargestRadius = nanobots.MaxBy(b => b.SignalRadius)!;
    return nanobots.Count(botWithLargestRadius.IsInRange);
}

long Part2(Nanobot[] nanobots)
{
    Dictionary<long, int> counters = [];

    void AddCounter(long distance, int counter)
    {
        if (!counters.ContainsKey(distance))
        {
            counters.Add(distance, 0);
        }
        counters[distance] += counter;
    }

    foreach (Nanobot bot in nanobots)
    {
        long distance = bot.Point.ManhattanDistance(Point.Zero);
        
        AddCounter(Math.Max(0, distance - bot.SignalRadius), 1);
        AddCounter(distance + bot.SignalRadius + 1, -1);
    }

    int runningCount = 0;
    int maxCount = 0;
    long distanceWithHighestCount = 0;

    foreach (var distance in counters.Keys.Order())
    {
        int count = counters[distance];
        runningCount += count;

        if (runningCount > maxCount)
        {
            maxCount = runningCount;
            distanceWithHighestCount = distance;
        }
    }

    return distanceWithHighestCount;
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
        return IsInRange(other.Point);
    }

    public bool IsInRange(Point otherPoint)
    {
        return Point.ManhattanDistance(otherPoint) <= SignalRadius;
    }
}

public record Point(int X, int Y, int Z)
{
    public int ManhattanDistance(Point other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
    }

    public static Point Zero = new Point(0, 0, 0);
}

