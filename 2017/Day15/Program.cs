// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day15\Full.txt";
#endif

    int[] values = Regex.Matches(File.ReadAllText(fileName), "\\d+").Select(m => int.Parse(m.Value)).ToArray();

    Console.WriteLine("Part 1: " + Part1(values[0], values[1]));
    Console.WriteLine("Part 2: " + Part2(values[0], values[1]));
    Console.ReadLine();
}

int Part1(int startA, int startB)
{
    var generatorA = Generator(startA, 16807, 1);
    var generatorB = Generator(startB, 48271, 1);

    var generated = generatorA.Zip(generatorB).Take(40_000_000);

    return generated.Count(pair => Judge(pair.First, pair.Second));
}

int Part2(int startA, int startB)
{
    var generatorA = Generator(startA, 16807, 4);
    var generatorB = Generator(startB, 48271, 8);

    var generated = generatorA.Zip(generatorB).Take(5_000_000);

    return generated.Count(pair => Judge(pair.First, pair.Second));
}

bool Judge(long first, long second)
{
    ushort lowerFirst = (ushort)first;
    ushort lowerSecond = (ushort)second;

    return lowerFirst == lowerSecond;
}

IEnumerable<long> Generator(long start, long factor, int mustBeMultipleOf)
{
    long previous = start;

    while (true)
    {
        previous *= factor;
        previous %= 2147483647L;

        if(previous % mustBeMultipleOf == 0) yield return previous;
    }
}
