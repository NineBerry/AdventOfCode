// #define Sample
using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day06\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Dictionary<char, int>[] counters = CreateStatistics(input);

    Console.WriteLine("Part 1: " + Part1(counters));
    Console.WriteLine("Part 2: " + Part2(counters));
    Console.ReadLine();
}

string Part1(Dictionary<char, int>[] counters)
{
    return string.Join("", counters.Select(c => c.OrderByDescending(info => info.Value).First().Key));
}

string Part2(Dictionary<char, int>[] counters)
{
    return string.Join("", counters.Select(c => c.OrderBy(info => info.Value).First().Key));
}

static Dictionary<char, int>[] CreateStatistics(string[] input)
{
    var counters = Enumerable.Range(0, input.First().Length).Select(i => new Dictionary<char, int>()).ToArray();

    foreach (var line in input)
    {
        for (int index = 0; index < line.Length; index++)
        {
            char ch = line[index];

            counters[index].TryGetValue(ch, out int counter);
            counters[index][ch] = counter + 1;
        }
    }

    return counters;
}