// #define Sample
using System.Diagnostics;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day07\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    return lines.Count(SupportsTLS);
}

long Part2(string[] lines)
{
    return lines.Count(SupportsSSL);
}

bool SupportsTLS(string input)
{
    bool inside = false;
    bool abbaInside = false;
    bool abbaOutside = false;

    for (int i = 0; i <= input.Length - 4; i++)
    {
        if (input[i] == '[')
        {
            inside = true;
        }
        else if (input[i] == ']')
        {
            inside = false;
        }
        else
        {
            string abbaCandidate = input.Substring(i, 4);

            if (IsAbba(abbaCandidate))
            {
                if(inside)
                {
                    abbaInside = true;
                }
                else
                {
                    abbaOutside = true;
                }
            }
        }
    }

    return abbaOutside && !abbaInside;
}

bool SupportsSSL(string input)
{
    bool inside = false;

    HashSet<string> insideAbas = new();
    HashSet<string> outsideAbas = new();

    for (int i = 0; i <= input.Length - 3; i++)
    {
        if (input[i] == '[')
        {
            inside = true;
        }
        else if (input[i] == ']')
        {
            inside = false;
        }
        else
        {
            string abaCandidate = input.Substring(i, 3);

            if (IsAba(abaCandidate))
            {
                if (inside)
                {
                    insideAbas.Add(abaCandidate);
                }
                else
                {
                    outsideAbas.Add(abaCandidate);
                }
            }
        }
    }

    return outsideAbas.Any(aba => insideAbas.Contains(InverseAba(aba)));
}

bool IsAbba(string abbaCandidate)
{
    Debug.Assert(abbaCandidate.Length == 4);

    return (abbaCandidate[0] != abbaCandidate[1]) && (abbaCandidate[0] == abbaCandidate[3]) && (abbaCandidate[1] == abbaCandidate[2]);
}

bool IsAba(string abaCandidate)
{
    Debug.Assert(abaCandidate.Length == 3);

    return (abaCandidate[0] != abaCandidate[1]) && (abaCandidate[0] == abaCandidate[2]);
}


string InverseAba(string aba)
{
    return new string([aba[1], aba[0], aba[1]]);
}


