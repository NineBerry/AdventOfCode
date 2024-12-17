#define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day18\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day18\Full.txt";
#endif

    File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1());
    // Console.WriteLine("Part 2: " + Part2());
    Console.ReadLine();
}

long Part1()
{
    return 0;
}
