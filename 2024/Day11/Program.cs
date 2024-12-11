// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day11\Full.txt";
#endif

    long[] input = File.ReadAllText(fileName).Split(' ').Select(long.Parse).ToArray();

    Console.WriteLine("Part 1: " + Solve(input, 25));
    Console.WriteLine("Part 2: " + Solve(input, 75));
    Console.ReadLine();
}

long Solve(long[] input, int blink)
{
    StoneLineMagic magic = new StoneLineMagic();
    return input.Sum(l => magic.Blink(l, blink));
}

class StoneLineMagic
{
    // Maps remaining blinks and value on count of values expected
    private Dictionary<(int Blinks, long Value), long> Cache = [];

    public long Blink(long value, int blink)
    {
        if (Cache.TryGetValue((blink, value), out long cached))
        {
            return cached;  
        }

        if (blink == 0) return 1;

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