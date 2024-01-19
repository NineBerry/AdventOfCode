// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day04\Full.txt";
#endif

    IEnumerable<IEnumerable<string>> input = File.ReadAllLines(fileName).Select(GetWords);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(IEnumerable<IEnumerable<string>> lines)
{
    return lines.Count(IsValidPart1);
}
long Part2(IEnumerable<IEnumerable<string>> lines)
{
    return lines.Count(IsValidPart2);
}

IEnumerable<string> GetWords(string input)
{
    return Regex.Matches(input, "[a-z]+").Select(m => m.Value);
}

bool IsValidPart1(IEnumerable<string> passphrase)
{
    return 
        ! 
        passphrase
        .GroupBy(s => s)
        .Where(g => g.Count() > 1)
        .Any();
}

bool IsValidPart2(IEnumerable<string> passphrase)
{
    return
        !
        passphrase
        .GroupBy(SortString)
        .Where(g => g.Count() > 1)
        .Any();
}

string SortString(string s)
{
    return  new String(s.OrderBy(ch => ch).ToArray());
}