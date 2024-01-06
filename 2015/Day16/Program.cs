using System.Text.RegularExpressions;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day16\Full.txt";

    Dictionary<string, int> descriptions = new()
    {
        { "children", 3 },
        { "cats", 7 },
        { "samoyeds", 2 },
        { "pomeranians", 3 },
        { "akitas", 0 },
        { "vizslas", 0 },
        { "goldfish", 5 },
        { "trees", 3 },
        { "cars", 2 },
        { "perfumes", 1 }
    };

    string[] input = File.ReadAllLines(fileName);
    Sue sueDescription = new Sue(-1, descriptions);

    IEnumerable<Sue> sues = input.Select(line => new Sue(line));

    Console.WriteLine("Part 1: " + Part1(sues, sueDescription));
    Console.WriteLine("Part 2: " + Part2(sues, sueDescription));
    Console.ReadLine();
}


long Part1(IEnumerable<Sue> sues, Sue sueDescription)
{
    return sues
        .First(sue => sue.FitsDescription(sueDescription, [], []))
        .Number;
}

long Part2(IEnumerable<Sue> sues, Sue sueDescription)
{
    HashSet<string> underratedProperties = ["cats", "trees"];
    HashSet<string> overratedProperties = ["pomeranians", "goldfish"];

    return sues
        .First(sue => sue.FitsDescription(sueDescription, underratedProperties, overratedProperties))
        .Number;
}

class Sue 
{
    public int Number { get; set; }
    public Dictionary<string, int> Properties { get; set; }

    public Sue(int number, Dictionary<string, int> properties)
    {
        Number = number;
        Properties = properties;
    }

    public Sue(string input)
    {
        Number = int.Parse(Regex.Match(input, "Sue (\\d+):").Groups[1].Value);

        Properties =
            Regex.Matches(input, "([a-z]+): (\\d+)")
            .Select(m => (m.Groups[1].Value, int.Parse(m.Groups[2].Value)))
            .ToDictionary();
    }

    public bool FitsDescription(Sue description, 
        HashSet<string> underratedProperties, 
        HashSet<string> overratedProperties)
    {
        foreach (var attribute in description.Properties)
        {
            if(Properties.TryGetValue(attribute.Key, out int value))
            {
                if (underratedProperties.Contains(attribute.Key))
                {
                    if (attribute.Value >= value) return false;
                }
                else if (overratedProperties.Contains(attribute.Key))
                {
                    if (attribute.Value <= value) return false;
                }
                else
                {
                    if (attribute.Value != value) return false;
                }
            }
        }

        return true;
    }
}