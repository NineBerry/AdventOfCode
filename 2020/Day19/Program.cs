// #define Sample

using System.Diagnostics;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day19\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Parser parser = new Parser(input.TakeWhile(s => s != "").ToArray());
    string[] messages = input.SkipWhile(s => s != "").Skip(1).ToArray();

    Console.WriteLine("Part 1: " + Part1(parser, messages));
    Console.WriteLine("Part 2: " + Part2(parser, messages));
    Console.ReadLine();
}

long Part1(Parser parser, string[] messages)
{
    return CountValidMessages(parser, messages);
};

long Part2(Parser parser, string[] messages)
{
    parser.Rules[8] = new Rule(parser, "8: 42 | 42 8");
    parser.Rules[11] = new Rule(parser, "11: 42 31 | 42 11 31");

    return CountValidMessages(parser, messages);
}

long CountValidMessages(Parser parser, string[] messages)
{
    return messages.Count(parser.MatchesWholeMessage);
}

public class Parser
{
    public Parser(string[] ruleDefs)
    {
        Rules = ruleDefs
            .Select(def => new Rule(this, def))
            .ToDictionary(r => r.ID, r => r);
    }

    public bool MatchesWholeMessage(string message)
    {
        int[] indexes = Rules[0].Eat(message, [0]);
        return indexes.Any(i => i == message.Length);
    }

    public Dictionary<int, Rule> Rules;
}

public class Rule
{
    public Rule(Parser parser, string ruleDef)
    {
        Parser = parser;

        string[] parts = ruleDef.Split(':', StringSplitOptions.TrimEntries);
        ID = int.Parse(parts[0]);

        if (parts[1].StartsWith('"'))
        {
            ActualCharacter = parts[1][1];
        }
        else
        {
            Subrules = parts[1].Split('|', StringSplitOptions.TrimEntries)
                .Select(a => a.Split(' ', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray())
                .ToArray();
        }
    }

    public int[] Eat(string message, int[] indexes)
    {
        if (indexes is []) return [];

        if (ActualCharacter.HasValue)
        {
            return Eat(message, indexes, ActualCharacter.Value);
        }
        else if (Subrules != null)
        {
            return Eat(message, indexes, Subrules);
        }
        else throw new UnreachableException();
    }
    private int[] Eat(string message, int[] indexes, char actualCharacter)
    {
        if (indexes is []) return [];

        return indexes
            .Where(i => i < message.Length && message[i] == actualCharacter)
            .Select(i => i + 1)
            .ToArray();
    }

    private int[] Eat(string message, int[] originalIndexes, int[][] alternativeRules)
    {
        if (originalIndexes is []) return [];

        HashSet<int> indexes = [];

        foreach (var sequence in alternativeRules)
        {
            int[] alternativeIndexes = Eat(message, originalIndexes, sequence);
            indexes.UnionWith(alternativeIndexes);

        }
        return indexes.ToArray();
    }

    private int[] Eat(string message, int[] originalIndexes, int[] sequentialRules)
    {
        if (originalIndexes is []) return [];

        int[] indexes = originalIndexes;

        foreach (var ruleID in sequentialRules)
        {
            Rule rule = Parser.Rules[ruleID];
            indexes = rule.Eat(message, indexes);
            if (indexes is [])
            {
                break;
            }
        }
        return indexes;
    }

    public int ID;
    public char? ActualCharacter = null;
    public Parser Parser;
    public int[][]? Subrules;
}