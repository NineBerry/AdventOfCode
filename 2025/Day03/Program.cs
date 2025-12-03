// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day03\Full.txt";
#endif

    int[][] batteries = 
        File
        .ReadAllLines(fileName)
        .Select(Tools.DigitValues)
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(batteries));
    Console.WriteLine("Part 2: " + Part2(batteries));
    Console.ReadLine();
}

long Part1(int[][] batteries)
{
    return Solve(batteries, 2);
}

long Part2(int[][] batteries)
{
    return Solve(batteries, 12);
}

long Solve(int[][] batteries, int batteriesAllowed)
{
    return batteries.Sum(row => OptimiseVoltage(row, batteriesAllowed)); ;
}

long OptimiseVoltage(int[] batteries, int batteriesAllowed)
{
    long result = 0;
    int leftBorder = 0;

    for(int restAllowed = batteriesAllowed; restAllowed > 0; restAllowed--)
    {
        var spanToSearch = batteries[leftBorder..^(restAllowed - 1)];
        (var maxFound, var maxIndex) = FindFirstMax(spanToSearch);

        result *= 10;
        result += maxFound;
        leftBorder += maxIndex + 1;
    }

    return result;
}

(int Value, int index) FindFirstMax(Span<int> values)
{
    int maxSoFar = values[0];
    int indexSoFar = 0;

    for (int i = 0; i < values.Length; i++)
    {
        if (values[i] > maxSoFar)
        {
            maxSoFar = values[i];
            indexSoFar = i;
        }
    }

    return (maxSoFar, indexSoFar);
}

static class Tools
{
    public static int[] DigitValues(this string str)
    {
        return
            str
            .Select(Tools.DigitValue)
            .ToArray();
    }

    public static int DigitValue(this char ch)
    {
        return int.Parse(ch.ToString());
    }
}