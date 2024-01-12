// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day01\Full.txt";
#endif

    int[] numbers = File.ReadAllLines(fileName).Select(int.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(numbers));
    Console.WriteLine("Part 2: " + Part2(numbers));
    Console.ReadLine();
}

long Part1(int[] numbers)
{
    return numbers.Sum();
}

long Part2(int[] numbers)
{
    int frequency = 0;
    HashSet<int> seen = [0];

    while (true)
    {
        foreach (int number in numbers)
        {
            frequency += number;
            if(seen.Contains(frequency)) return frequency;
            seen.Add(frequency);
        }
    }
}