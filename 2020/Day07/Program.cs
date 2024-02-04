// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day07\Full.txt";
#endif

    string[]lines = File.ReadAllLines(fileName);
    BagRuleset ruleset = new BagRuleset(lines);

    Console.WriteLine("Part 1: " + Part1(ruleset));
    Console.WriteLine("Part 2: " + Part2(ruleset));
    Console.ReadLine();
}

long Part1(BagRuleset ruleset)
{
    HashSet<Bag> bags = [];
    Bag shinyGold = ruleset.GetBag("shiny gold");
    Traverse(shinyGold);
    return bags.Except([shinyGold]).Count();

    void Traverse(Bag bag)
    {
        if (!bags.Contains(bag))
        {
            bags.Add(bag);
            foreach ((Bag child, _) in bag.CanBeContainedIn) 
            { 
                Traverse(child);
            }
        }
    }
}

long Part2(BagRuleset ruleset)
{
    int bagCount = 0;
    
    Bag shinyGold = ruleset.GetBag("shiny gold");
    Traverse(shinyGold, 1);
    
    return bagCount - 1;

    void Traverse(Bag bag, int multiplier)
    {
        bagCount += multiplier;

        foreach ((Bag child, int count) in bag.Contains)
        {
            Traverse(child, multiplier * count);
        }
    }
}

public class BagRuleset
{
    Dictionary<string, Bag> bags = [];

    public BagRuleset(string[] lines)
    {
        foreach (string line in lines)
        {
            AddBagInformation(line);
        }
    }

    private void AddBagInformation(string line)
    {
        string parent = 
            Regex.Matches(line, "^([a-z]+ [a-z]+) b")
            .Select(m => m.Groups[1].Value)
            .First();

        Bag parentBag = GetBag(parent);

        var childs = Regex.Matches(line, "(\\d+) ([a-z]+ [a-z]+) b").Select(m => (Name: m.Groups[2].Value, Count: int.Parse(m.Groups[1].Value)));

        foreach(var child in childs)
        {
            var childBag = GetBag(child.Name);
            parentBag.Contains.Add((childBag, child.Count));
            childBag.CanBeContainedIn.Add((parentBag, child.Count)); ;
        }
    }

    public Bag GetBag(string name)
    {
        if(!bags.TryGetValue(name, out var bag))
        {
            bag = new Bag(name);
            bags[name] = bag;
        }

        return bag;
    }
}

public record Bag
{
    public Bag(string name)
    {
        Name = name;    
    }

    public string Name;
    public List<(Bag Bag, int Count)> CanBeContainedIn = [];
    public List<(Bag Bag, int Count)> Contains = [];

    public override string ToString() => Name;
}