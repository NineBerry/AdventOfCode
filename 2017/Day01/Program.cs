// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day01\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    return Solve(input, 1);
}

long Part2(string input)
{
    return Solve(input, input.Length / 2);
}

long Solve(string input, int offset)
{
    input += input.Substring(0, offset);

    return
        input
        .Zip(input.Skip(offset))
        .Where(pair => pair.First == pair.Second)
        .Select(pair => pair.First - '0')
        .Sum();
}