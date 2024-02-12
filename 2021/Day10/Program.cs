// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day10\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    ParserResult[] results = input.Select(ParserResult.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(results));
    Console.WriteLine("Part 2: " + Part2(results));
    Console.ReadLine();
}

long Part1(ParserResult[] results)
{
    return results
        .Where(r => r.Type == ParserResultType.Corrupted)
        .Select(r => r.GetScore())
        .Sum();
}

long Part2(ParserResult[] results)
{
    var sortedScores = results
        .Where(r => r.Type == ParserResultType.Incomplete)
        .Select(r => r.GetScore())
        .Order()
        .ToArray();

    return sortedScores[sortedScores.Length / 2];
}


public class ParserResult
{
    public ParserResult(ParserResultType type, char corruptedCharacter = '\0', string missingCharacters = "")
    {
        Type = type;
        CorruptedCharacter = corruptedCharacter;
        MissingCharacters = missingCharacters;
    }

    public ParserResultType Type;
    public char CorruptedCharacter;
    public string MissingCharacters;


    public static ParserResult Parse(string input)
    {
        string opening = "<({[";
        string closing = ">)}]";

        Stack<char> stack = [];

        foreach(var ch in input)
        {
            if (opening.Contains(ch))
            {
                stack.Push(ch);
            }
            else if (closing.Contains(ch))
            {
                var read = stack.Pop();
                if(ch != GetClosing(read))
                {
                    return new ParserResult(ParserResultType.Corrupted, ch);
                }
            }
            else throw new ApplicationException("Unexpected Character");
        }

        if (stack.Count > 0)
        {
            string missing = "";

            while(stack.TryPop(out char missingChar))
            {
                missing += GetClosing(missingChar);
            }
            return new ParserResult(ParserResultType.Incomplete, missingCharacters: missing);
        }
        
        return new ParserResult(ParserResultType.Match, '\0');
    }

    public long GetScore()
    {
        if(Type == ParserResultType.Corrupted) return GetPointsForCorrupted(CorruptedCharacter);
        if (Type == ParserResultType.Incomplete) return GetPointsForIncomplete(MissingCharacters);
        return 0;
    }

    private long GetPointsForIncomplete(string missingCharacters)
    {
        long result = 0;

        foreach(char ch in missingCharacters)
        {
            result *= 5;
            result += ch switch
            {
                ')' => 1,
                ']' => 2,
                '}' => 3,
                '>' => 4,
                _ => throw new ArgumentException("Unknown Character")
            };
        }

        return result;
    }

    private static long GetPointsForCorrupted(char ch)
    {
        return ch switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new ArgumentException("Unknown Character")
        };
    }
    private static char GetClosing(char ch)
    {
        return ch switch
        {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            '<' => '>',
            _ => throw new ArgumentException("Unknown Character")
        };
    }
}

public enum ParserResultType
{
    Match,
    Corrupted,
    Incomplete
}