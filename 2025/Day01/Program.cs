// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day01\Full.txt";
#endif

    var steps =
        File.ReadAllLines(fileName)
        .Select(l => l.Replace("R", ""))
        .Select(l => l.Replace("L", "-"))
        .Select(int.Parse).
        ToArray();

    Console.WriteLine("Part 1: " + Part1(steps));
    Console.WriteLine("Part 2: " + Part2(steps));
    Console.ReadLine();
}

long Part1(int[] steps)
{
    int zeroHit = 0;
    int current = 50;

    foreach(var step in steps)
    {
        current += step;
        if (current % 100 == 0) zeroHit++;
    }

    return zeroHit;
}

long Part2(int[] steps)
{
    // Numbers are low, so it is okay to just simulate 
    // turning the dial.
    long zeroHit = 0;
    int current = 50;

    foreach (var step in steps)
    {
        foreach (var _ in Enumerable.Range(1, Math.Abs(step)))
        {
            current += Math.Sign(step);
            if (current % 100 == 0) zeroHit++;
        }
    }

    return zeroHit;
}

