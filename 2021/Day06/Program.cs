// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day06\Full.txt";
#endif

    int[] fish = File.ReadAllText(fileName).Split(',').Select(s => int.Parse(s)).ToArray();

    Console.WriteLine("Part 1: " + Solve(fish, 80));
    Console.WriteLine("Part 2: " + Solve(fish, 256));
    Console.ReadLine();
}

long Solve(int[] initialFish, int steps)
{
    long[] fish = new long[9];
    for(int i = 0; i <= 8; i++)
    {
        fish[i] = initialFish.Count(f => f == i);
    }

    foreach(int step in Enumerable.Range(1, steps))
    {
        Step(fish);
    }

    return fish.Sum();
}

void Step(long[] fish)
{
    long zeros = fish[0];

    for(int i = 0; i < 8; i++)
    {
        fish[i] = fish[i + 1];
    }

    fish[6] += zeros;
    fish[8] = zeros;
}