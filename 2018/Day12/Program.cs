// #define Sample

using System.Text;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day12\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    string initalState = lines.First().Substring(15);
    HashSet<string> potProducingRules = lines.Skip(2).Where(s => s.EndsWith('#')).Select(s => s.Substring(0, 5)).ToHashSet();

    Console.WriteLine("Part 1: " + Solve(initalState, potProducingRules, 20));
    Console.WriteLine("Part 2: " + Solve(initalState, potProducingRules, 50_000_000_000));
    Console.ReadLine();
}

long Solve(string initialState, HashSet<string> potProducingRules, long steps)
{
    string text = initialState;
    long leftOffset = 0;
    
    long previousSum = 0;
    long previousDiff = 0;

    for (long step = 1; step <= steps; step++)
    {
        PerformStep(potProducingRules, ref text, ref leftOffset);

        long sum = CalculateSum(leftOffset, text);
        long diff = sum - previousSum;

        if (previousDiff == diff) // Constant found
        {
            long remaining = steps - step;
            sum += (diff * remaining);
            return sum;
        }

        previousSum = sum;
        previousDiff = diff;
    }


    return previousSum;
}

long CalculateSum(long leftOffset, string text)
{
    long sum = 0;
    
    for(int i=0; i < text.Length; i++)
    {
        if (text[i] == '#') sum += (i - leftOffset);
    }

    return sum;
}

static void PerformStep(HashSet<string> potProducingRules, ref string text, ref long leftOffset)
{
    if (!text.StartsWith("....."))
    {
        text = "....." + text;
        leftOffset += 5;
    }

    if (!text.EndsWith("....."))
    {
        text = text + ".....";
    }

    StringBuilder sb = new StringBuilder("..");

    leftOffset -= 2;
    for (int i = 2; i < text.Length - 5; i++)
    {
        string test = text.Substring(i, 5);
        sb.Append(potProducingRules.Contains(test) ? '#' : '.');
    }

    text = sb.ToString();
}