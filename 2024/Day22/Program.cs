// #define Sample
using System.Runtime.CompilerServices;

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2024\Day22\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2024\Day22\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2024\Day22\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2024\Day22\Full.txt";
#endif

    Console.WriteLine("Part 1: " + Part1(Tools.GetNumbers(fileNamePart1)));
    Console.WriteLine("Part 2: " + Part2(Tools.GetNumbers(fileNamePart2)));
    Console.ReadLine();
}

long Part1(long[] numbers)
{
    return numbers.Sum(secret => RNG.NextNthNumber(secret, 2000));
}

long Part2(long[] numbers)
{
    MonkeyMarket market = new MonkeyMarket(numbers);
    return market.GetMostBananas();
}

class MonkeyMarket
{
    private long[] Monkeys;

    public MonkeyMarket(long[] secrets)
    {
        Monkeys = [..secrets];
    }

    public long GetMostBananas()
    {
        var bananasAfterSequences = Monkeys.SelectMany(GetSequencesForMonkey);
        var groupedBySequence = bananasAfterSequences.GroupBy(m => m.Sequence);
        
        return groupedBySequence.Max(g => g.Select(m => m.Bananas).Sum());
    }

    private IEnumerable<(string Sequence, long Bananas)> GetSequencesForMonkey(long monkey)
    {
        Dictionary<string, long> result = [];

        IEnumerable<long> numbers = [monkey, .. RNG.NextNNumbers(monkey, 2000)];
        byte[] prizes = numbers.Select(n => (byte)(n % 10)).ToArray();

        sbyte[] differences = prizes.Zip(prizes.Skip(1)).Select(p => (sbyte)((p.Second) - (p.First))).ToArray();

        for (int pos = 0; pos < differences.Length - 3; pos++)
        {
            sbyte[] sequence = [differences[pos], differences[pos + 1], differences[pos + 2], differences[pos + 3]];
            byte bananas = prizes[pos + 4];
            string sequenceString = string.Join(',', sequence);

            if (!result.ContainsKey(sequenceString))
            {
                result.Add(sequenceString, bananas);
            }
        }

        return result.Select(p => (p.Key, p.Value));
    }
}

static class RNG
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Mix(long secret, long value)
    {
        return secret ^ value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Prune(long secret)
    {
        return secret & 0b11111111_11111111_11111111;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextNumber(long secret)
    {
        secret = Prune(Mix(secret, secret << 6));
        secret = Prune(Mix(secret, secret >> 5));
        secret = Prune(Mix(secret, secret << 11));

        return secret;
    }

    public static long NextNthNumber(long secret, int n)
    {
        foreach (var _ in Enumerable.Range(0, n))
        {
            secret = NextNumber(secret);
        }

        return secret;
    }

    public static long[] NextNNumbers(long secret, int n)
    {
        List<long> result = [];
        foreach (var _ in Enumerable.Range(0, n))
        {
            secret = NextNumber(secret);
            result.Add(secret);
        }
        return [.. result];
    }
}

public static class Tools
{
    public static long[] GetNumbers(string fileName)
    {
        return File.ReadAllLines(fileName).Select(long.Parse).ToArray();
    }
}