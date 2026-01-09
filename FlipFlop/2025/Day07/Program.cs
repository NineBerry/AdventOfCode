// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day07\Full.txt";
#endif

    var values = File.ReadAllLines(fileName).Select(Combinatorium.ParseLine).ToArray();
    Combinatorium combinatorium = new();

    Console.WriteLine("Part 1: " + Part1(values, combinatorium));
    Console.WriteLine("Part 2: " + Part2(values, combinatorium));
    Console.WriteLine("Part 3: " + Part3(values, combinatorium));

    Console.ReadLine();
}

long Part1((long, long)[] values, Combinatorium combinatorium)
{
    return values.Sum(v => combinatorium.CountCombinations([v.Item1 - 1, v.Item2 - 1]));
}

long Part2((long, long)[] values, Combinatorium combinatorium)
{
    return values.Sum(v => combinatorium.CountCombinations([v.Item1 - 1, v.Item2 -1 , v.Item1 - 1]));
}

long Part3((long, long)[] values, Combinatorium combinatorium)
{
    return values.Sum(v => combinatorium.CountCombinations(MakeNDimensional(v)));

    long[] MakeNDimensional((long, long) input)
    {
        long[] result = new long[input.Item1];
        for(int i=0; i < result.Length; i++) result[i] = input.Item2 - 1;
        return result;
    }
}

class Combinatorium
{
    Dictionary<string, long> Cache = [];

    public long CountCombinations(long[] from)
    {
        string cacheKey = string.Join(",", from);
        if (Cache.TryGetValue(cacheKey, out long result)) return result;

        if (from.All(s => s == 0)) return 1;

        result = 0;

        for (int i = 0; i < from.Length; i++)
        {
            if (from[i] > 0)
            {
                long[] copy = [..from];
                copy[i] = copy[i] - 1;
                result += CountCombinations(copy);
            }
        }

        Cache[cacheKey] = result;
        return result;
    }

    public static (long, long) ParseLine(string line)
    {
        long[] values = line.Split(' ').Select(long.Parse).ToArray();
        return (values[0], values[1]);
    }
}