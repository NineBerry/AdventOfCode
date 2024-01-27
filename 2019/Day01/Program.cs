// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day01\Full.txt";
#endif

    long[] values = File.ReadAllLines(fileName).Select(long.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(values));
    Console.WriteLine("Part 2: " + Part2(values));
    Console.ReadLine();
}

long Part1(long[] values)
{
    return values.Sum(CalcFuel);
}

long Part2(long[] values)
{
    return values.Sum(CalcFuelRecursively);
}

long CalcFuel(long mass)
{
    return Math.Max((mass / 3) - 2, 0);
}

long CalcFuelRecursively(long mass)
{
    long fuel = CalcFuel(mass);

    if (fuel > 0) fuel += CalcFuelRecursively(fuel);

    return fuel;
}
