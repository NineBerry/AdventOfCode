// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day02\Full.txt";
#endif

    IEnumerable<int[]> input = File.ReadAllLines(fileName).Select(ParseLine);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(IEnumerable<int[]> input)
{
    return input.Sum(values => values.Max() - values.Min());
}

long Part2(IEnumerable<int[]> input)
{
    
    return input.Sum(GetEvenDivision);
}

int GetEvenDivision(int[] values)
{
    for (int i = 0; i < values.Length; i++)
    {
        for (int j = 0; j < values.Length; j++)
        {
            if (i == j) continue;

            if (values[i] % values[j] == 0) return values[i] / values[j];
        }
    }

    return 0;
}

int[] ParseLine(string line)
{
    return Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();   
}