// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day03\Full.txt";
#endif
    var code = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(code));
    Console.WriteLine("Part 2: " + Part2(code));
    Console.ReadLine();
}

const string mulRegex = @"mul\((\d{1,3}),(\d{1,3})\)";
const string dontInstruction = "don't()";
const string doInstruction = "do()";

long Part1(string code)
{
    return ProcessCode(code, mulRegex);
}

// Alternative approach for Part 2 would be this regex
// do\(\).*?(mul\((\d{1,3}),(\d{1,3})\).*?)*don't\(\)
// With Option SingleLine enabled

long Part2(string code)
{
    string combinedRegex = mulRegex
        + "|" + Regex.Escape(dontInstruction)
        + "|" + Regex.Escape(doInstruction);

    return ProcessCode(code, combinedRegex);
}

long ProcessCode(string code, string regex)
{
    bool mulEnabled = true;
    return Regex.Matches(code, regex)
        .Sum(m => ProcessInstruction(m, ref mulEnabled));
}

long ProcessInstruction(Match match, ref bool mulEnabled)
{
    long result = 0;
    
    switch (match.Value)
    {
        case doInstruction:
            mulEnabled = true;
            break;
        case dontInstruction:
            mulEnabled = false;
            break;
        default:
            if (mulEnabled) result = EvaluateMul(match);
            break;
    }
    
    return result;
}

long EvaluateMul(Match match)
{
    int left = int.Parse(match.Groups[1].Value);
    int right = int.Parse(match.Groups[2].Value);
    return left * right;
}
