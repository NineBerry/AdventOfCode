// #define Sample

using System.Diagnostics;
using System.Numerics;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day11\Full.txt";
#endif

    long[] input = 
        File
        .ReadAllText(fileName)
        .Split(' ')
        .Select(long.Parse)
        .ToArray();

    var watch = Stopwatch.StartNew();
    StoneLineMagic magic = new();
    Console.WriteLine("Part 1: " + magic.Blink(input, 25));
    Console.WriteLine("Part 2: " + magic.Blink(input, 75));
    Console.WriteLine($"Took {watch.ElapsedMilliseconds} ms");
    Console.WriteLine();

    watch = Stopwatch.StartNew();
    StoneLineMagicWithLanternFish magicWithLanternFish = new();
    Console.WriteLine("Part 1: " + magicWithLanternFish.Blink(input, 25));
    Console.WriteLine("Part 2: " + magicWithLanternFish.Blink(input, 75));
    Console.WriteLine($"Took {watch.ElapsedMilliseconds} ms");

    Console.ReadLine();
}

class StoneLineMagic
{
    // Cache: Maps remaining blinks and value on count of values expected
    private Dictionary<(int Blinks, long Value), long> Cache = [];

    public long Blink(long[] input, int blink) => input.Sum(l => Blink(l, blink));

    public long Blink(long value, int blink)
    {
        if (blink == 0) return 1;
        
        if (Cache.TryGetValue((blink, value), out long cached))
        {
            return cached;  
        }

        long result;

        if (value == 0)
        {
            result = Blink(1, blink - 1);
        }
        else if (value.IsSplittableNumber(out long leftPart, out long rightPart))
        {
            long leftCount = Blink(leftPart, blink - 1);
            long rightCount = Blink(rightPart, blink - 1);
            result = leftCount + rightCount;
        }
        else
        {
            result = Blink(value * 2024, blink - 1);
        }

        Cache.Add((blink, value), result);

        return result;
    }
}

class StoneLineMagicWithLanternFish
{
    public long Blink(long[] input, int blink)
    {
        var counts = 
            input.CountBy(i => i)
            .ToDictionary(p => p.Key, p => (long)p.Value);

        foreach (var step in Enumerable.Range(1, blink))
        {
            counts = PerformStep(counts);
        }

        return counts.Values.Sum();
    }

    private Dictionary<long, long> PerformStep(Dictionary<long, long> input)
    {
        Dictionary<long, long> result = [];

        foreach((long value, long count) in input)
        {
            if (value == 0)
            {
                result.AddSum(1, count);
            }
            else if (value.IsSplittableNumber(out long leftPart, out long rightPart))
            {
                result.AddSum(leftPart, count);
                result.AddSum(rightPart, count);
            }
            else
            {
                result.AddSum(value * 2024, count);
            }
        }

        return result;
    }
}

static class Tools
{
    public static bool IsSplittableNumber(this long number, out long leftPart, out long rightPart)
    {
        string str = number.ToString();

        if (int.IsEvenInteger(str.Length))
        {
            int partLength = str.Length / 2;
            leftPart = long.Parse(str.Substring(0, partLength));
            rightPart = long.Parse(str.Substring(partLength));
            return true;
        }

        leftPart = rightPart = 0;
        return false;
    }

    public static void AddSum<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) 
        where TKey: IBinaryInteger<TKey>
        where TValue: IBinaryInteger<TValue>
    {
        if(dict.TryGetValue(key, out var current))
        {
            dict[key] = current + value;
        }
        else
        {
            dict[key] = value;
        }
    }
}