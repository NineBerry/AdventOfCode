// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day02\Full.txt";
#endif

    var ranges = 
        File.ReadAllText(fileName)
        .Split(",")
        .Select(InclusiveRange.Parse)
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(ranges));
    Console.WriteLine("Part 2: " + Part2(ranges));
    Console.ReadLine();
}

long Part1(InclusiveRange[] ranges)
{
    return Solve(ranges, 2);
}

long Part2(InclusiveRange[] ranges)
{
    // Max value is 10 digits long, so
    // multiply not more than 10 times
    return Solve(ranges, 10);
}

long Solve(InclusiveRange[] ranges, int maxMultiply)
{
    return
        CreateInvalidNumbers(maxMultiply)
        .Where(num => ranges.Any(r => r.Contains(num)))
        .Distinct()
        .Sum();
}

IEnumerable<long> CreateInvalidNumbers(int maxMultiply)
{
    // Max value was determined once with 
    // 
    // long maxValue = ranges.Max(r => r.End);
    // Console.WriteLine("Maxvalue: " + maxValue);
    //
    // and is 95959_93435
    // 
    // So max value we need to duplicate is ~100_000
    // We will use that as upper bound.
    foreach (var multiply in Enumerable.Range(2, maxMultiply - 1))
    {
        foreach (var num in Enumerable.Range(1, 100_000))
        {
            yield return num.Multiply(multiply);
        }
    }

    // To increase performance when multiplying 2 times, 3 times and so forth,
    // we could calcuate a respective upper bound each time using log10(). 
}

public record InclusiveRange
{
    public InclusiveRange(long start, long end)
    {
        Start = start;
        End = end;
    }

    public bool Contains(long value)
    {
        return value >= Start && value <= End;
    }

    public static InclusiveRange Parse(string s)
    {
        var values = s.Split('-').Select(long.Parse).ToArray();
        return new InclusiveRange(values[0], values[1]);
    }

    public long Start;
    public long End;
}

public static class Tools
{
    public static long Multiply(this int number, int times)
    {
        string s = number.ToString();
        string multiplied = string.Concat(Enumerable.Repeat(s, times));

        if (!long.TryParse(multiplied, out var result))
        {
            result = long.MaxValue;
        }

        return result;
    }
}