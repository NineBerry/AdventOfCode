// #define Sample

using static System.Runtime.InteropServices.JavaScript.JSType;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day01\Full.txt";
#endif
    IEnumerable<(int Left, int Right)> numbers = 
        File
        .ReadAllLines(fileName)
        .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .Select(s => s.Select(n => int.Parse(n)))
        .Select(n => (n.First(), n.Last()));

    int[] left = numbers.Select(n => n.Left).ToArray();
    int[] right = numbers.Select(n => n.Right).ToArray();

    Console.WriteLine("Part 1: " + Part1(left, right));
    Console.WriteLine("Part 2: " + Part2(left, right));
    Console.ReadLine();
}

long Part1(int[] left, int[] right)
{
    Array.Sort(left);
    Array.Sort(right);

    return left
        .Zip(right)
        .Sum(a => Math.Abs(a.Second - a.First));
}

long Part2(int[] left, int[] right)
{
    var leftCounts = left.CountBy(i => i).ToDictionary();
    
    return right
        .Where(i => leftCounts.ContainsKey(i))
        .Sum(i => i * leftCounts[i]);
}