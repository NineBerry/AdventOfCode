// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day07\Full.txt";
#endif

    int[] crabs = File.ReadAllText(fileName).Split(',').Select(int.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(crabs));
    Console.WriteLine("Part 2: " + Part2(crabs));

    Console.ReadLine();
}

long Part1(int[] crabs)
{
    int min = crabs.Min();
    int max = crabs.Max();
    int range = max - min + 1;
    
    return Enumerable.Range(min, range)
        .Select(crab => 
            crabs.Select(c => Math.Abs(c - crab))
            .Sum())
        .Min();
}

long Part2(int[] crabs)
{
    int min = crabs.Min();
    int max = crabs.Max(); 
    int range = max - min + 1;

    return Enumerable.Range(min, range)
        .Select(crab =>
            crabs.Select(c => MakeFuelForDistance(Math.Abs(c - crab)))
            .Sum())
        .Min();
}

long MakeFuelForDistance(long distance)
{
    if (distance <= 1) return distance;

    long baseSum = distance + 1;

    long sum = baseSum * (distance / 2);
    if (distance % 2 == 1) sum += (baseSum / 2);

    return sum; 
}