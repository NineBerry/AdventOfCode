#define Sample

using System.Diagnostics;
using System.Diagnostics.Contracts;

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
    int count = 0;

    foreach(var message in messages)
    {
        if(parser.MatchesWholeMessage(message))
        {
            Console.WriteLine("Match: " + message);
            count++;
        }
    }

    return count;
}

long Part2(Parser parser, string[] messages)
{
    parser.Rules[8] = new Rule(parser, "8: 42 | 42 8");
    parser.Rules[11] = new Rule(parser, "11: 42 31 | 42 11 31");

    int count = 0;

    foreach (var message in messages)
    {
        if (parser.MatchesWholeMessage(message))
        {
            Console.WriteLine("Match: " + message);
            count++;
        }
    }

    return count;
}



public class Parser
{
    public Parser(string[] ruleDefs)
    {
        foreach(var ruleDef in ruleDefs)
        {
            Rule rule = new Rule(this, ruleDef);
            Rules.Add(rule.ID, rule);
        }
    }
    
    public bool MatchesWholeMessage(string message)
    {
        (bool result, int index) = Rules[0].Eat(message, 0);
        return result && index == message.Length;
    }

    public Dictionary<int, Rule> Rules = [];
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

    public (bool Match, int Index) Eat(string message, int index)
    {
        if(ActualCharacter.HasValue)
        {
            return Eat(message, index, ActualCharacter.Value);
        }
        else if(Subrules != null)
        {
            return Eat(message, index, Subrules);
        }
        else throw new UnreachableException();
    }
    private (bool Match, int Index) Eat(string message, int index, char actualCharacter)
    {
        if(index >= message.Length) return (false, index); 

        if (message[index] == actualCharacter)
        {
            return (true, index + 1);
        }
        return (false, index);
    }

    private (bool Match, int Index) Eat(string message, int originalIndex, int[][] alternativeRules)
    {
        foreach (var sequence in alternativeRules)
        {
            (var result, var index) = Eat(message, originalIndex, sequence);
            if (result)
            {
                return (result, index);
            }
        }
        return (false, originalIndex);
    }

    private (bool Match, int Index) Eat(string message, int originalIndex, int[] sequentialRules)
    {
        bool result = false;
        int index = originalIndex;
        foreach (var ruleID in sequentialRules)
        {
            Rule rule = Parser.Rules[ruleID];
            (result, index) = rule.Eat(message, index);
            if (!result)
            {
                break;
            }
        }
        return (result, index);
    }

    public int ID;
    public char? ActualCharacter = null;
    public Parser Parser;
    public int[][]? Subrules;
}