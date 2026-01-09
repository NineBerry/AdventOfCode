// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day02\Full.txt";
#endif

    string rollerCoaster = File.ReadAllText(fileName);
    Console.WriteLine("Part 1: " + Part1(rollerCoaster));
    Console.WriteLine("Part 2: " + Part2(rollerCoaster));
    Console.WriteLine("Part 3: " + Part3(rollerCoaster));

    Console.ReadLine();
}


long Part1(string rollerCoaster)
{
    long maxHeight = 0;
    long currentHeight = 0;

    foreach(var ch in rollerCoaster)
    {
        if (ch == '^')
        {
            currentHeight++;
            maxHeight = Math.Max(maxHeight, currentHeight);
        }
        else currentHeight--;
    }

    return maxHeight;
}

long Part2(string rollerCoaster)
{
    long maxHeight = 0;
    long currentHeight = 0;
    long currentSpin = 0;

    foreach (var ch in rollerCoaster)
    {
        if (ch == '^')
        {
            if(currentSpin < 0)
            {
                currentSpin = 0;
            }
            currentSpin++;
            currentHeight += currentSpin;
            maxHeight = Math.Max(maxHeight, currentHeight);
        }
        else
        {
            if (currentSpin > 0)
            {
                currentSpin = 0;
            }
            currentSpin--;
            currentHeight += currentSpin;
        }
    }

    return maxHeight;
}

long Part3(string rollerCoaster)
{
    long maxHeight = 0;
    long currentHeight = 0;
    long currentSpin = 0;

    foreach (var ch in rollerCoaster)
    {
        if (ch == '^')
        {
            if (currentSpin < 0)
            {
                ApplySpin(currentSpin);
                currentSpin = 0;
            }
            currentSpin++;
        }
        else
        {
            if (currentSpin > 0)
            {
                ApplySpin(currentSpin);
                currentSpin = 0;
            }
            currentSpin--;
        }
    }
    ApplySpin(currentSpin);

    return maxHeight;

    void ApplySpin(long spin)
    {
        int sign = Math.Sign(spin);
        currentHeight += sign * Tools.Fib(Math.Abs(spin));
        maxHeight = Math.Max(maxHeight, currentHeight);
    }
}


static class Tools
{
    private static Dictionary<long, long> fibCache = [];
    public static long Fib(long input)
    {
        if (input == 0) return 0;
        if (input == 1) return 1;
        
        if (!fibCache.TryGetValue(input, out long result))
        {
           result =  Fib(input - 2) + Fib(input - 1);
            fibCache.Add(input, result);
        }
        
        return result;
    }
}
