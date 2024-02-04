// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day06\Full.txt";
#endif

    string[] groups = 
        File.ReadAllText(fileName)
        .ReplaceLineEndings("\n")
        .Split("\n\n")
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(groups));
    Console.WriteLine("Part 2: " + Part2(groups));
    Console.ReadLine();
}

long Part1(string[] groups)
{
    return groups
        .Select(g => g.Replace("\n", ""))
        .Select(g => g.Distinct())
        .Select(g => g.Count())
        .Sum();
}

long Part2(string[] groups)
{
    return groups
        .Select(g => g.Split("\n").Select(s => s.AsEnumerable()))
        .Select(g => g.Aggregate((a, b) => a.Intersect(b)))
        .Select(g => g.Count())
        .Sum();
}

