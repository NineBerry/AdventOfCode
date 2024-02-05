// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day09\Sample.txt";
    int preambleSize = 5;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day09\Full.txt";
    int preambleSize = 25;
#endif

    long[]values = File.ReadAllLines(fileName).Select(long.Parse).ToArray();

    long specialNumber = Part1(values, preambleSize);
    Console.WriteLine("Part 1: " + specialNumber);
    Console.WriteLine("Part 2: " + Part2(values, specialNumber));
    Console.ReadLine();
}

long Part1(long[] values, int preambleSize)
{
    for (int i = preambleSize; i < values.Length; i++)
    {
        var preamble = values.Skip(i - preambleSize).Take(preambleSize).ToHashSet();
        long check = values[i];

        if (!HasSummands(preamble, check)) return check;
    }

    return 0;
}

bool HasSummands(HashSet<long> preamble, long check)
{
    foreach (long num1 in preamble)
    {
        if (preamble.Contains(check - num1)) return true;
    }

    return false;
}


long Part2(long[] values, long specialNumber)
{
    for(int testStartIndex=0; testStartIndex < values.Length; testStartIndex++)
    {
        long sum = 0;

        for(int i=testStartIndex; i < values.Length; i++)
        {
            sum+= values[i];
            
            if (sum > specialNumber) break;
            if (sum == specialNumber)
            {
                var range = values.Skip(testStartIndex).Take((i + 1) - testStartIndex);

                return range.Max() + range.Min();
            }
        }
    }

    return 0;
}


