// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day20\Sample.txt";
    long upperLimit = 9;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day20\Full.txt";
    long upperLimit = 4294967295;
#endif

    string[] input = File.ReadAllLines(fileName);
    Range[] ranges = input.Select(s => new Range(s)).OrderBy(r => r.MinInclusive).ToArray();
    Range[] combinedRanges = CombineRanges(ranges);

    Console.WriteLine("Part 1: " + Part1(combinedRanges));
    Console.WriteLine("Part 2: " + Part2(combinedRanges, upperLimit));
    Console.ReadLine();
}


long Part1(Range[] ranges)
{
    return ranges[0].MaxInclusive + 1;
}

long Part2(Range[] ranges, long upperLimit)
{
    long result = 0;

    foreach(var pair in ranges.Zip(ranges.Skip(1)))
    {
        result += (pair.Second.MinInclusive - pair.First.MaxInclusive - 1);
    }

    result += upperLimit - ranges.Last().MaxInclusive;

    return result;
}


Range[] CombineRanges(Range[] ranges)
{
    List<Range> result = [ranges[0]];

    foreach(var range in ranges)
    {
        Range last = result.Last();

        if(range.MinInclusive > last.MaxInclusive + 1)
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

public class Range
{
    public Range(string s)
    {
        long[] values = Regex.Matches(s, "\\d+").Select(m => long.Parse(m.Value)).ToArray();  
        MinInclusive = values[0];
        MaxInclusive = values[1];
    }

    public long MinInclusive;
    public long MaxInclusive;
}