// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day15\Sample.txt";
    int part1Row = 10;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day15\Full.txt";
    int part1Row = 2_000_000;
#endif


    var sensors = File.ReadAllLines(fileName).Select(s => new Sensor(s)).ToArray();
    int part2MaxCoordinate = 2 * part1Row;

    Console.WriteLine("Part 1: " + Part1(sensors, part1Row));
    Console.WriteLine("Part 2: " + Part2(sensors, part2MaxCoordinate));
    Console.ReadLine();
}

long Part1(Sensor[] sensors, int part1Row)
{
    int countBeacons = 
        sensors
        .Where(s => s.NearestBeacon.Y == part1Row)
        .Select(s => s.NearestBeacon)
        .Distinct()
        .Count();

    var ranges = GetRangesForRow(sensors, part1Row, int.MinValue, int.MaxValue);
    return ranges.First().Length - countBeacons;
}

long Part2(Sensor[] sensors, int part2MaxCoordinate)
{
    int startRow = part2MaxCoordinate / 2;
    for(int offset = 0; offset <= startRow; offset++)
    {
        Point? point = 
            FindBeaconInRow(sensors, startRow - offset, part2MaxCoordinate)
            ?? FindBeaconInRow(sensors, startRow + offset, part2MaxCoordinate);

        if (point.HasValue) return point.Value.TuningFrequency();
    }

    return 0;
}

Point? FindBeaconInRow(Sensor[] sensors, int row, int maxCoordinate)
{
    var ranges = GetRangesForRow(sensors, row, 0, maxCoordinate);

    if (ranges.Length > 1)
    {
        return new Point(ranges[0].MaxInclusive + 1 , row);
    }

    return null;
}

Range[] GetRangesForRow(Sensor[] sensors, int row, int minCoordinate, int maxCoordinate)
{
    var ranges = 
        sensors
        .Select(s => s.GetRangeForRow(row, minCoordinate, maxCoordinate))
        .Where(r => r != null)
        .Select(s => s!)
        .OrderBy(r => r.MinInclusive)
        .ToArray();
    ranges = Range.CombineRanges(ranges);
    return ranges;
}

public record struct Point(int X, int Y)
{
    public int ManhattanDistance(Point point)
    {
        var xDistance = Math.Abs(point.X - X);
        var yDistance = Math.Abs(point.Y - Y);
        return xDistance + yDistance;
    }

    public long TuningFrequency()
    {
        return (long)X * 4000000 + Y;
    }
}

public record class Range
{
    public Range(int minInclusive, int maxInclusive)
    {
        MinInclusive = minInclusive;
        MaxInclusive = maxInclusive;
    }

    public int MinInclusive;
    public int MaxInclusive;

    public static Range[] CombineRanges(Range[] ranges)
    {
        List<Range> result = [ranges[0]];

        foreach (var range in ranges)
        {
            Range last = result.Last();

            if (range.MinInclusive > last.MaxInclusive + 1)
            {
                result.Add(range);
            }
            else
            {
                last.MaxInclusive = Math.Max(last.MaxInclusive, range.MaxInclusive);
            }
        }

        return result.ToArray();
    }

    public int Length => MaxInclusive - MinInclusive + 1;
}

public class Sensor
{
    public Point SensorPoint;
    public Point NearestBeacon;
    public int DistanceToNearestBeacon;

    public Sensor(string input)
    {
        int[] values = Regex.Matches(input, "-?\\d+").Select(m => int.Parse(m.Value)).ToArray();
        SensorPoint = new Point(values[0], values[1]);
        NearestBeacon = new Point(values[2], values[3]);
        DistanceToNearestBeacon = SensorPoint.ManhattanDistance(NearestBeacon);
    }

    public Range? GetRangeForRow(int row, int limitLower, int limitUpper)
    {
        if(row < SensorPoint.Y - DistanceToNearestBeacon) return null;
        if (row > SensorPoint.Y + DistanceToNearestBeacon) return null;

        int rowDistance = Math.Abs(row - SensorPoint.Y);
        int colDistance = DistanceToNearestBeacon - rowDistance;

        int min = Math.Max(Math.Min(SensorPoint.X - colDistance, limitUpper), limitLower);
        int max = Math.Max(Math.Min(SensorPoint.X + colDistance, limitUpper), limitLower);

        return new Range(min, max);
    }
}