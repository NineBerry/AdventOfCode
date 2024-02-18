// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day01\Full.txt";
#endif

    int[][] input = 
        File.ReadAllText(fileName)
        .ReplaceLineEndings("\n")
        .Split("\n\n")
        .Select(s => s.Split("\n").Select(int.Parse).ToArray())
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(int[][] input)
{
    return input.Select(a => a.Sum()).Max();
}

long Part2(int[][] input)
{
    return input.Select(a => a.Sum()).OrderDescending().Take(3).Sum();
}
