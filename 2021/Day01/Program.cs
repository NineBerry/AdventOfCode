// #define Sample
{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day01\Full.txt";
#endif

    int[] numbers = File.ReadAllLines(fileName).Select(s => int.Parse(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(numbers));
    Console.WriteLine("Part 2: " + Part2(numbers));
    Console.ReadLine();
}

long Part1(int[] numbers)
{
    return numbers.Zip(numbers.Skip(1)).Count(pair => pair.Second > pair.First);
}

long Part2(int[] numbers)
{
    return
        Enumerable.Range(0, numbers.Length - 3)
        .Zip(Enumerable.Range(1, numbers.Length - 2))
        .Count(pair =>
            numbers[pair.Second] + numbers[pair.Second + 1] + numbers[pair.Second + 2] >
            numbers[pair.First] + numbers[pair.First + 1] + numbers[pair.First + 2]);
    
}