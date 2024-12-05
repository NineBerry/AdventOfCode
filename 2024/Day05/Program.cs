// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day05\Full.txt";
#endif
    var lines = File.ReadAllLines(fileName);

    OrderingRules orderingRules = new(
        lines
        .TakeWhile(x => x != "")
        .Select(l => new OrderingRule(l))
    );

    SafetyManualUpdate[] safetyManualUpdates =
        lines
        .SkipWhile(x => x != "")
        .Skip(1)
        .Select(l => new SafetyManualUpdate(l))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(orderingRules, safetyManualUpdates));
    Console.WriteLine("Part 2: " + Part2(orderingRules, safetyManualUpdates));
    Console.ReadLine();
}

int Part1(OrderingRules orderingRules, SafetyManualUpdate[] safetyManualUpdates)
{
    return
        safetyManualUpdates
        .Where(m => m.FollowsRules(orderingRules))
        .Sum(m => m.GetMiddlePageNumber());
}

int Part2(OrderingRules orderingRules, SafetyManualUpdate[] safetyManualUpdates)
{
    return
        safetyManualUpdates
        .Where(m => !m.FollowsRules(orderingRules))
        .Select(m => m.CreateWithCorrectOrder(orderingRules))
        .Sum(m => m.GetMiddlePageNumber());
}

public class OrderingRule
{
    public readonly int Lower;
    public readonly int Upper;

    public OrderingRule(string input)
    {
        int[] values = Tools.ParseIntegers(input);
        Lower = values[0];
        Upper = values[1];
    }
}

public class OrderingRules: IComparer<int>
{
    public readonly OrderingRule[] Rules;
    private HashSet<(int, int)> rulesHash;

    public OrderingRules(IEnumerable<OrderingRule> rules)
    {
        Rules = [..rules];
        rulesHash = [..Rules.Select(r => (r.Lower, r.Upper))];
    }

    public int Compare(int x, int y)
    {
        if(x == y) return 0;

        if (rulesHash.Contains((x, y))) return 1;
        if (rulesHash.Contains((y, x))) return -1;

        throw new Exception($"Don't know how to order {x} and {y}");
    }
}

public class SafetyManualUpdate
{
    private int[] pages;
    private Dictionary<int, int> indexOfPages;

    public SafetyManualUpdate(string input) : this(Tools.ParseIntegers(input))
    {
    }

    public SafetyManualUpdate(int[] input)
    {
        pages = [..input];    
        indexOfPages = pages
            .Select((page, index) => (page, index))
            .ToDictionary(pair => pair.page, pair => pair.index);
    }

    public bool FollowsRules(OrderingRules rules)
    {
        return rules.Rules.All(FollowsRule);
    }
    
    private bool FollowsRule(OrderingRule rule)
    {
        if (indexOfPages.TryGetValue(rule.Lower, out int lowerIndex)
            && indexOfPages.TryGetValue(rule.Upper, out int upperIndex))
        {
            return lowerIndex < upperIndex;
        }

        return true;
    }

    public SafetyManualUpdate CreateWithCorrectOrder(OrderingRules orderingRules)
    {
        int[] orderedPages = [.. pages.Order(orderingRules)];
        return new SafetyManualUpdate(orderedPages);
    }
    public int GetMiddlePageNumber()
    {
        return pages[pages.Length / 2];
    }
}

public static class Tools
{
    public static int[] ParseIntegers(string input)
    {
        return
            Regex.Matches(input, @"\d+")
            .Select(m => int.Parse(m.Value))
            .ToArray();
    }
}