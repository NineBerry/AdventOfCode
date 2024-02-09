// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day14\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day14\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    ResourceRuleset ruleset = new ResourceRuleset(lines);

    Console.WriteLine("Part 1: " + Part1(ruleset));
    Console.WriteLine("Part 2: " + Part2(ruleset));
    Console.ReadLine();
}

long Part1(ResourceRuleset ruleset)
{
    return ruleset.CalculateRequiredOre(ResourceRuleset.FUELName, 1);
}


long Part2(ResourceRuleset ruleset)
{
    const long maxAvailableORE = 1_000_000_000_000;

    long minFuel = 1;
    long maxFuel = maxAvailableORE;

    // Binary search
    while (maxFuel > minFuel + 1)
    {
        long midFuel = minFuel + (maxFuel - minFuel) / 2;
        long midORECount = ruleset.CalculateRequiredOre(ResourceRuleset.FUELName, midFuel);

        if(midORECount >= maxAvailableORE)
        {
            maxFuel = midFuel;
        }
        else
        {
            minFuel = midFuel;
        }
    }

    return minFuel;
}

public class ResourceRuleset
{
    Dictionary<string, ResourceCreationRule> resourceCreationRules = [];

    public ResourceRuleset(string[] lines)
    {
        foreach (string line in lines)
        {
            AddResourceInformation(line);
        }
    }

    private void AddResourceInformation(string line)
    {
        var matches = Regex.Matches(line, "([0-9]+) ([A-Z]+)");

        string parentName = matches.Last().Groups[2].Value;
        long createdCount = long.Parse(matches.Last().Groups[1].Value);

        ResourceCreationRule parentRule = GetResourceCreationRule(parentName);
        parentRule.CreatedCount = createdCount;

        var childs = matches.SkipLast(1).Select(m => (Name: m.Groups[2].Value, RequiredCount: long.Parse(m.Groups[1].Value)));

        foreach (var child in childs)
        {
            var childRule = GetResourceCreationRule(child.Name);
            parentRule.Required.Add((childRule, child.RequiredCount));
        }
    }

    public ResourceCreationRule GetResourceCreationRule(string name)
    {
        if (!resourceCreationRules.TryGetValue(name, out var rule))
        {
            rule = new ResourceCreationRule(name);
            resourceCreationRules[name] = rule;
        }

        return rule;
    }

    public long CalculateRequiredOre(string wantedResource, long wantedQuantity)
    {
        Dictionary<string, long> spareParts = [];
        Dictionary<string, long> requiredParts = [];
        requiredParts.Add(wantedResource, wantedQuantity);

        while (requiredParts.Count > 1 || requiredParts.Keys.Single() != OREName)
        {
            ReduceResources(requiredParts, spareParts);
        }

        return requiredParts[OREName];
    }

    private void ReduceResources(Dictionary<string, long> requiredParts, Dictionary<string, long> spareParts)
    {
        var toReduce = requiredParts.Where(pair => pair.Key != OREName).Take(1).Select(pair => (Name: pair.Key, Count: pair.Value)).Single();
        requiredParts.Remove(toReduce.Name);

        // Take parts from spare parts if available
        if (spareParts.TryGetValue(toReduce.Name, out var available) && available > 0)
        {
            long takeAvailable = Math.Min(toReduce.Count, available);
            toReduce.Count -= takeAvailable;
            spareParts[toReduce.Name] = spareParts[toReduce.Name] - takeAvailable;
        }

        if(toReduce.Count > 0)
        {
            AddResourcesRequiredForResource(toReduce, requiredParts, spareParts);
        }
    }

    private void AddResourcesRequiredForResource((string Name, long Count) toReduce, Dictionary<string, long> requiredParts, Dictionary<string, long> spareParts)
    {
        var parentRule = GetResourceCreationRule(toReduce.Name);

        (long actualWanted, long spare) = GetActualRequired(toReduce.Count, parentRule.CreatedCount);

        if(spare > 0)
        {
            spareParts.TryGetValue(toReduce.Name, out var available);
            spareParts[toReduce.Name] = available + spare;
        }

        foreach (var childRule in parentRule.Required)
        {
            AddResourcesRequiredForResource(actualWanted, childRule, requiredParts);
        }
    }

    private void AddResourcesRequiredForResource(long wantedCount, 
        (ResourceCreationRule Rule, long RequiredCount) childRule, 
        Dictionary<string, long> requiredParts)
    {
        requiredParts.TryGetValue(childRule.Rule.Name, out var currentRequired);
        requiredParts[childRule.Rule.Name] = currentRequired  + wantedCount * childRule.RequiredCount;
    }

    private (long Required, long Spare) GetActualRequired(long requiredCount, long canCreateCount)
    {
        if (requiredCount % canCreateCount == 0) return (requiredCount / canCreateCount, 0);

        long actualRequiredCount = ((requiredCount / canCreateCount) + 1);
        long spare = actualRequiredCount * canCreateCount - requiredCount;
        return (actualRequiredCount, spare);
    }

    public const string OREName = "ORE";
    public const string FUELName = "FUEL";
}

public record ResourceCreationRule
{
    public ResourceCreationRule(string name)
    {
        Name = name;
    }

    public string Name;
    public long CreatedCount;
    public List<(ResourceCreationRule Rule, long RequiredCount)> Required= [];

    public override string ToString() => Name;
}