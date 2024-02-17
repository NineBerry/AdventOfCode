// #define Sample

using System;
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
    // Idea: Get median of each Coordinate, then test a certain box around that point.
    var rangesX = GetCoordinateRanges(nanobots, b => b.Point.X);
    var rangesY = GetCoordinateRanges(nanobots, b => b.Point.Y);
    var rangesZ = GetCoordinateRanges(nanobots, b => b.Point.Z);


    var xRange = rangesX.MaxBy(r => r.Counter)!;
    var yRange = rangesY.MaxBy(r => r.Counter)!;
    var zRange = rangesZ.MaxBy(r => r.Counter)!;

    Console.WriteLine($"Ranges Counter is {xRange.Counter},{yRange.Counter},{zRange.Counter}");
    Console.WriteLine($"Ranges Size is {xRange.Size},{yRange.Size},{zRange.Size}");

    Dictionary<Point, int> pointsAroundMedian = [];

    // Point testPoint = new Point(medianX + x, medianY + y, medianZ + z);
    // pointsAroundMedian.Add(testPoint, CountNanoBots(nanobots, testPoint));

    // int maxCount = pointsAroundMedian.Select(pair => pair.Value).Max();
    // Console.WriteLine($"Max Count is {maxCount}");

    // var pointsWithMaxCount = pointsAroundMedian.Where(pair => pair.Value == maxCount).Select(pair => pair.Key).ToArray();
    // Console.WriteLine($"Points with MaxCount: {pointsWithMaxCount.Length}");

    Point zero = new Point(0, 0, 0);   
    var nearestPoint = new Point(xRange.Min, yRange.Min, zRange.Min);

    Console.WriteLine($"Nearest Point: {nearestPoint}");

    Console.WriteLine($"Nearest Point in Range : {CountNanoBots(nanobots, nearestPoint)}");

    int distanceToZero = nearestPoint.ManhattanDistance(zero);
    Console.WriteLine($"Distance: {distanceToZero}");

    int requiredCounter =   Math.Min(xRange.Counter, Math.Min(yRange.Counter, zRange.Counter));
    Console.WriteLine($"Required Counter: {requiredCounter}");


    var specificRangesX = rangesX.Where(r => r.Counter == requiredCounter).OrderBy(r => r.Min).ToArray();
    var specificRangesY = rangesY.Where(r => r.Counter == requiredCounter).OrderBy(r => r.Min).ToArray();
    var specificRangesZ = rangesZ.Where(r => r.Counter == requiredCounter).OrderBy(r => r.Min).ToArray();

    Console.WriteLine($"Specific Ranges Count is {specificRangesX.Count()},{specificRangesY.Count()},{specificRangesZ.Count()}");

    nearestPoint = new Point(specificRangesX.First().Min, specificRangesY.First().Min, specificRangesZ.First().Min);
    Console.WriteLine($"Nearest Point: {nearestPoint}");
    Console.WriteLine($"Nearest Point in Range : {CountNanoBots(nanobots, nearestPoint)}");
    distanceToZero = nearestPoint.ManhattanDistance(zero);
    Console.WriteLine($"Distance: {distanceToZero}");

    Console.WriteLine($"Ranges Size is {specificRangesX.First().Size},{specificRangesY.First().Size},{specificRangesZ.First().Size}");



    for(int x = 0; x < int.MaxValue; x++)
    {
                nearestPoint = new Point(specificRangesX.First().Min + x, specificRangesY.First().Min + x, specificRangesZ.First().Min + x);
                // Console.WriteLine($"Nearest Point: {nearestPoint}");
                // Console.WriteLine($"Nearest Point in Range : {CountNanoBots(nanobots, nearestPoint)}");
                
                if(CountNanoBots(nanobots, nearestPoint) == requiredCounter)
                {
                    distanceToZero = nearestPoint.ManhattanDistance(zero);
                    Console.WriteLine($"Distance: {distanceToZero}");
                    break;
                }
    }


    // Idea: Separate for X, Y, Z coordinates:
    // build ranges that are in reach of nanobots.
    // When combining ranges, extract the overlapping
    // parts into new ranges.
    // For each range, keep note of how many bots are in reach.
    // Then take the ranges with mit nanobots in range for each
    // coordinate, select the value closest to 0 for that coordinate

    // Hm, maybe separate does not work, but we need to consider spheres?

    // Plan did not work. Try something else. Maybe use Cuboids and combine them instead of ranges

    return distanceToZero;
}

// 154331300 is too high
// 151031545 is too high
// 107137375 is too low (Min XRange, YRange, ZRange)

List<RangeWithCounter> GetCoordinateRanges(Nanobot[] nanobots, Func<Nanobot, int> callback)
{
    Ranges ranges = new Ranges();

    foreach(var nanobot in nanobots)
    {
        ranges.AddRange(callback(nanobot) - nanobot.SignalRadius, callback(nanobot) + nanobot.SignalRadius);
    }

    return ranges.RangesList;
}

int CountNanoBots(Nanobot[] nanobots, Point point)
{
    return nanobots.Count(b => b.IsInRange(point));
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
}

public class RangeWithCounter
{
    public RangeWithCounter(int min, int max, int counter)
    {
        Min = min; 
        Max = max; 
        Counter = counter;
    }

    public int Min;
    public int Max;
    public int Counter;
    public int Size => (Max - Min) + 1;

    public override string ToString()
    {
        return $"[{Min},{Max}]x{Counter}";
    }
}

public class Ranges
{
    public List<RangeWithCounter> RangesList = [];

    public void AddRange(int min, int max)
    {
        int listPosition = 0;

        while (true)
        {
            if (listPosition == RangesList.Count)
            {
                RangesList.Add(new RangeWithCounter(min, max, 1));
                break;
            }

            if (min >= max) return;

            var currentItem = RangesList[listPosition];

            if (min < currentItem.Min)
            {
                int currentEnd = Math.Min(max, currentItem.Min);
                RangesList.Insert(listPosition, new RangeWithCounter(min, currentEnd, 1));
                min = currentEnd;
                listPosition++;
                continue;
            }

            if (min >= currentItem.Max)
            {
                listPosition++;
                continue;
            }

            if (min == currentItem.Min && max == currentItem.Max)
            {
                currentItem.Counter++;
                listPosition++;
                continue;
            }

            if (min >= currentItem.Min)
            {
                RangesList.Remove(currentItem);

                if (min > currentItem.Min)
                {
                    RangesList.Insert(listPosition, new RangeWithCounter(currentItem.Min, min, currentItem.Counter));
                    listPosition++;
                }

                int currentEnd = Math.Min(currentItem.Max, max);
                RangesList.Insert(listPosition, new RangeWithCounter(min, currentEnd, currentItem.Counter + 1));
                listPosition++;

                // If some of current item remaining
                if (currentItem.Max > max)
                {
                    RangesList.Insert(listPosition, new RangeWithCounter(max, currentItem.Max, currentItem.Counter));
                    listPosition++;
                }

                min = currentItem.Max;
            }
        }
    }
}

// Consider code created by ChatGPT for cuboid intersection: 


class Cuboid
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }

    public Cuboid(int x, int y, int z, int width, int height, int depth)
    {
        X = x;
        Y = y;
        Z = z;
        Width = width;
        Height = height;
        Depth = depth;
    }
}

class CuboidIntersection
{
    public List<Cuboid> Intersection { get; set; }
    public List<Cuboid> Remainder { get; set; }
}

class Sample
{
    static CuboidIntersection IntersectCuboids(Cuboid cuboid1, Cuboid cuboid2)
    {
        // Calculate intersection
        int xMin = Math.Max(cuboid1.X, cuboid2.X);
        int yMin = Math.Max(cuboid1.Y, cuboid2.Y);
        int zMin = Math.Max(cuboid1.Z, cuboid2.Z);

        int xMax = Math.Min(cuboid1.X + cuboid1.Width, cuboid2.X + cuboid2.Width);
        int yMax = Math.Min(cuboid1.Y + cuboid1.Height, cuboid2.Y + cuboid2.Height);
        int zMax = Math.Min(cuboid1.Z + cuboid1.Depth, cuboid2.Z + cuboid2.Depth);

        if (xMax > xMin && yMax > yMin && zMax > zMin)
        {
            // Cuboids intersect
            List<Cuboid> intersection = new List<Cuboid>
            {
                new Cuboid(xMin, yMin, zMin, xMax - xMin, yMax - yMin, zMax - zMin)
            };

            // Calculate remainders
            List<Cuboid> remainder = new List<Cuboid>();

            // Remainder of cuboid1
            if (cuboid1.X < xMin)
                remainder.Add(new Cuboid(cuboid1.X, cuboid1.Y, cuboid1.Z, xMin - cuboid1.X, cuboid1.Height, cuboid1.Depth));
            if (cuboid1.X + cuboid1.Width > xMax)
                remainder.Add(new Cuboid(xMax, cuboid1.Y, cuboid1.Z, cuboid1.X + cuboid1.Width - xMax, cuboid1.Height, cuboid1.Depth));

            if (cuboid1.Y < yMin)
                remainder.Add(new Cuboid(xMin, cuboid1.Y, cuboid1.Z, intersection[0].Width, yMin - cuboid1.Y, cuboid1.Depth));
            if (cuboid1.Y + cuboid1.Height > yMax)
                remainder.Add(new Cuboid(xMin, yMax, cuboid1.Z, intersection[0].Width, cuboid1.Y + cuboid1.Height - yMax, cuboid1.Depth));

            if (cuboid1.Z < zMin)
                remainder.Add(new Cuboid(xMin, yMin, cuboid1.Z, intersection[0].Width, intersection[0].Height, zMin - cuboid1.Z));
            if (cuboid1.Z + cuboid1.Depth > zMax)
                remainder.Add(new Cuboid(xMin, yMin, zMax, intersection[0].Width, intersection[0].Height, cuboid1.Z + cuboid1.Depth - zMax));

            // Remainder of cuboid2
            if (cuboid2.X < xMin)
                remainder.Add(new Cuboid(cuboid2.X, cuboid2.Y, cuboid2.Z, xMin - cuboid2.X, cuboid2.Height, cuboid2.Depth));
            if (cuboid2.X + cuboid2.Width > xMax)
                remainder.Add(new Cuboid(xMax, cuboid2.Y, cuboid2.Z, cuboid2.X + cuboid2.Width - xMax, cuboid2.Height, cuboid2.Depth));

            if (cuboid2.Y < yMin)
                remainder.Add(new Cuboid(xMin, cuboid2.Y, cuboid2.Z, intersection[0].Width, yMin - cuboid2.Y, cuboid2.Depth));
            if (cuboid2.Y + cuboid2.Height > yMax)
                remainder.Add(new Cuboid(xMin, yMax, cuboid2.Z, intersection[0].Width, cuboid2.Y + cuboid2.Height - yMax, cuboid2.Depth));

            if (cuboid2.Z < zMin)
                remainder.Add(new Cuboid(xMin, yMin, cuboid2.Z, intersection[0].Width, intersection[0].Height, zMin - cuboid2.Z));
            if (cuboid2.Z + cuboid2.Depth > zMax)
                remainder.Add(new Cuboid(xMin, yMin, zMax, intersection[0].Width, intersection[0].Height, cuboid2.Z + cuboid2.Depth - zMax));

            CuboidIntersection cuboidIntersection = new CuboidIntersection
            {
                Intersection = intersection,
                Remainder = remainder
            };

            return cuboidIntersection;
        }
        else
        {
            // Cuboids do not intersect
            return new CuboidIntersection
            {
                Intersection = new List<Cuboid>(),
                Remainder = new List<Cuboid> { cuboid1, cuboid2 }
            };
        }
    }

    static void Main()
    {
        // Example usage
        Cuboid cuboid1 = new Cuboid(0, 0, 0, 5, 5, 5);
        Cuboid cuboid2 = new Cuboid(3, 3, 3, 5, 5, 5);

        CuboidIntersection result = IntersectCuboids(cuboid1, cuboid2);

        Console.WriteLine("Intersection:");
        foreach (var cuboid in result.Intersection)
        {
            Console.WriteLine($"({cuboid.X}, {cuboid.Y}, {cuboid.Z}, {cuboid.Width}, {cuboid.Height}, {cuboid.Depth})");
        }

        Console.WriteLine("\nRemainder:");
        foreach (var cuboid in result.Remainder)
        {
            Console.WriteLine($"({cuboid.X}, {cuboid.Y}, {cuboid.Z}, {cuboid.Width}, {cuboid.Height}, {cuboid.Depth})");
        }
    }
}
