// #define Sample
using System.Text;
using RuleGrid = System.Collections.Generic.List<System.Collections.Generic.List<Rule>>;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day21\Sample.txt";
    int iterationsPart1 = 2;
    int iterationsPart2 = 2;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day21\Full.txt";
    int iterationsPart1 = 5;
    int iterationsPart2 = 18;
#endif

    Rule[] rules = File.ReadAllLines(fileName).Select(l => new Rule(l)).ToArray();

    Console.WriteLine("Part 1: " + Solve(rules, iterationsPart1));
    Console.WriteLine("Part 2: " + Solve(rules, iterationsPart2));
    Console.ReadLine();
}


long Solve(Rule[] rules, int iterations)
{
    const string startPattern = ".#./..#/###";

    ArtProcess process = new ArtProcess(rules);
    Rule start = process.GetRuleForPattern(startPattern);
    RuleGrid grid = [[start]];
    
    foreach (var _ in Enumerable.Range(1, iterations - 1))
    {
        grid = process.Step(grid);
    }

    return grid.Sum(row => row.Sum(rule => rule.OutputCount));
}

/// <summary>
/// I exptected a much higher value for Part2 that would require optimzing the algorithm.
/// So I chose an approach that could later on be optimized by remembering what set of rules 
/// one rule would turn into after three steps. This way we would only deal with references to rules 
/// and wouldn't have to actually build the grid text for later iterations.
/// But since part2 only requires 18 iterations, that was not necessary on my gaming pc ;)
/// </summary>


public class ArtProcess
{
    public ArtProcess(Rule[] rules) 
    { 
        Rules = rules;
    }

    public readonly Rule[] Rules;
    Dictionary<string, Rule> PatternsToRules = [];

    public Rule GetRuleForPattern(string pattern)
    {
        if (PatternsToRules.TryGetValue(pattern, out var rule))
        {
            return rule;
        }

        if ((rule = GetRuleForPatternFromArray(pattern)) != null)
        {
            PatternsToRules[pattern] = rule;
            return rule;
        }

        string rotatedPattern = pattern;
        for(int i=0; i <= 3; i++)
        {
            if (PatternsToRules.TryGetValue(rotatedPattern, out rule))
            {
                PatternsToRules[pattern] = rule;
                return rule;
            }

            if ((rule = GetRuleForPatternFromArray(rotatedPattern)) != null)
            {
                PatternsToRules[pattern] = rule;
                return rule;
            }

            string flippedPattern = Rule.FlipVertical(rotatedPattern);
            if (PatternsToRules.TryGetValue(flippedPattern, out rule))
            {
                PatternsToRules[pattern] = rule;
                return rule;
            }

            if ((rule = GetRuleForPatternFromArray(flippedPattern)) != null)
            {
                PatternsToRules[pattern] = rule;
                return rule;
            }

            rotatedPattern = Rule.RotateClockwise(rotatedPattern);
        }

        throw new ApplicationException("More flipping or rotation required");
    }

    private Rule? GetRuleForPatternFromArray(string pattern)
    {
        return Rules.Where(r => r.Input == pattern).FirstOrDefault();
    }

    internal RuleGrid Step(RuleGrid fromGrid)
    {
        List<StringBuilder> stringRepresentation = MakeExpansion(fromGrid);

        return SplitIntoRulesToApply(stringRepresentation.Select(sb => sb.ToString()).ToArray());
    }

    private RuleGrid SplitIntoRulesToApply(string[] stringRepresentation)
    {
        int sourceWidth = stringRepresentation.Length;
        int targetStepWidth = sourceWidth % 2 == 0 ? 2 : 3;
        int targetSteps = sourceWidth / targetStepWidth;
        
        RuleGrid grid = Enumerable.Range(0, targetSteps).Select(_ => new List<Rule>(targetSteps)).ToList();

        for(int x=0; x <targetSteps; x++)
        {
            for (int y = 0; y < targetSteps; y++)
            {
                string pattern = "";
                for(int i=0; i < targetStepWidth; i++)
                {
                    if(pattern != "") pattern += "/";
                    pattern += stringRepresentation[y * targetStepWidth + i].Substring(x * targetStepWidth, targetStepWidth);
                }

                grid[y].Add(GetRuleForPattern(pattern));
            }
        }

        return grid;
    }

    private static List<StringBuilder> MakeExpansion(RuleGrid fromGrid)
    {
        List<StringBuilder> stringRepresentation = new List<StringBuilder>();

        int offsetY = 0;
        foreach (var ruleLine in fromGrid)
        {
            int newBlockWidth = ruleLine.First().OutputWidth;

            foreach (var i in Enumerable.Range(0, newBlockWidth))
            {
                stringRepresentation.Add(new StringBuilder());
            }

            foreach (var rule in ruleLine)
            {
                foreach (var i in Enumerable.Range(0, newBlockWidth))
                {
                    stringRepresentation[offsetY + i].Append(rule.OutputExpanded[i]);
                }
            }

            offsetY += newBlockWidth;
        }

        return stringRepresentation;
    }
}

public record Rule
{
    public Rule(string line)
    {
        string[] parts = line.Split("=>", StringSplitOptions.TrimEntries);

        Input = parts[0];   
        Output = parts[1];

        InputCount = Input.Count(ch => ch == '#');
        OutputCount = Output.Count(ch => ch == '#');

        InputExpanded = Expand(Input);
        OutputExpanded = Expand(Output);

        InputWidth = InputExpanded[0].Length;
        OutputWidth = OutputExpanded[0].Length;
    }

    public string Input;
    public string Output;

    public string[] InputExpanded;
    public string[] OutputExpanded;

    public int InputCount;
    public int OutputCount;

    public int InputWidth;
    public int OutputWidth;

    public static string Compress(string[] input)
    {
        return string.Join('/', input);
    }
    public static string[] Expand(string input)
    {
        return input.Split('/');
    }

    internal static string RotateClockwise(string pattern)
    {
        string[] patternExpanded = Expand(pattern);

        string firstRow = patternExpanded.First();

        var rotatedMap = firstRow.Select(
                (column, columnIndex) => string.Join("", patternExpanded.Reverse().Select(row => row[columnIndex]))
            ).ToArray();

        return Compress(rotatedMap);
    }

    internal static string FlipVertical(string pattern)
    {
        string[] patternExpanded = Expand(pattern);

        return Compress(patternExpanded.Select(s => new string(s.Reverse().ToArray())).ToArray());
    }
}