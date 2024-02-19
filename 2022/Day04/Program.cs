// #define Sample

using System.Text.RegularExpressions;
using RangeCouple = (InclusiveRange First, InclusiveRange Second);

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day04\Full.txt";
#endif

    RangeCouple[] couples = File.ReadAllLines(fileName).Select(Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(couples));
    Console.WriteLine("Part 2: " + Part2(couples));
    Console.ReadLine();
}

long Part1(RangeCouple[] couples)
{
    return couples.Count(c => c.First.FullyIncludes(c.Second) || c.Second.FullyIncludes(c.First));
}

long Part2(RangeCouple[] couples)
{
    return couples.Count(c => c.First.Overlaps(c.Second));
}


RangeCouple Parse(string line)
{
    int[] values = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    return (new InclusiveRange(values[0], values[1]), new InclusiveRange(values[2], values[3]));
}


public class InclusiveRange
{
    public InclusiveRange(int min, int max)
    {
        Min = min; 
        Max = max;
    }

    public int Min;
    public int Max;

    public bool FullyIncludes(InclusiveRange other)
    {
        return (Min <= other.Min) && (Max >= other.Max);
    }
    public bool Overlaps(InclusiveRange other)
    {
        return FullyIncludes(other) || (Min >= other.Min && Min <= other.Max) || (Max >= other.Min && Max <= other.Max);
    }
}

