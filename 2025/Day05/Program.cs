// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day05\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);
    
    var ranges = 
        lines
        .TakeWhile(s => s != "")
        .Select(InclusiveRange.Parse)
        .ToArray();

    var ids = 
        lines
        .SkipWhile(s => s != "").Skip(1)
        .Select(long.Parse)
        .ToArray();

    var combinedRanges = InclusiveRange.CombineRanges(ranges);

    Console.WriteLine("Part 1: " + Part1(combinedRanges, ids));
    Console.WriteLine("Part 2: " + Part2(combinedRanges));
    Console.ReadLine();
}

long Part1(InclusiveRange[] freshRanges, long[] foodIds)
{
    return foodIds.Count(id => freshRanges.Any(range => range.Contains(id)));
}

long Part2(InclusiveRange[] ranges)
{
    return ranges.Sum(range => range.Length);
}


public record InclusiveRange
{
    public InclusiveRange(long start, long end)
    {
        Start = start;
        End = end;
    }

    public long Length 
    {
        get => End - Start + 1;
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

    public static InclusiveRange[] CombineRanges(InclusiveRange[] ranges)
    {
        ranges = [..ranges.OrderBy(range => range.Start)];

        List<InclusiveRange> result = [ranges.First()];

        foreach (var range in ranges)
        {
            var last = result.Last();

            if (range.Start > last.End + 1)
            {
                result.Add(range);
            }
            else
            {
                last.End = Math.Max(last.End, range.End);
            }
        }

        return result.ToArray();
    }
}

