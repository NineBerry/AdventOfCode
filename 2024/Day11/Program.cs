// #define Sample

using System.Reflection.PortableExecutable;

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
    
    StoneLineMagic magic = new StoneLineMagic();

    Console.WriteLine("Part 1: " + magic.Blink(input, 25));
    Console.WriteLine("Part 2: " + magic.Blink(input, 75));
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
        else if (IsSplittableNumber(value, out long leftPart, out long rightPart))
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

    bool IsSplittableNumber(long number, out long leftPart, out long rightPart)
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
}