// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day19\Full.txt";
#endif 

    var lines = File.ReadAllLines(fileName);

    var availableDesigns = lines[0].Split(',', StringSplitOptions.TrimEntries).ToHashSet();
    var requestedTowels = lines.Skip(2).ToArray();
    Onsen onsen = new(availableDesigns);

    Console.WriteLine("Part 1: " + Part1(onsen, requestedTowels));
    Console.WriteLine("Part 2: " + Part2(onsen, requestedTowels));
    Console.ReadLine();
}


int Part1(Onsen onsen, string[] requestedTowels)
{
    return requestedTowels.Count(s => onsen.GetCombinationCount(s) > 0);
}

long Part2(Onsen onsen, string[] requestedTowels)
{
    return requestedTowels.Sum(s => onsen.GetCombinationCount(s));
}

public class Onsen
{
    private HashSet<string> AvailableDesigns;
    private Dictionary<string, long> CombinationCounts = [];

    public Onsen(HashSet<string> availableDesigns)
    {
        AvailableDesigns = availableDesigns;
    }

    public long GetCombinationCount(string towel)
    {
        if(towel == "") return 1;

        if(CombinationCounts.TryGetValue(towel, out long cached))
        {
            return cached;
        }

        long result = 0;

        foreach (var design in AvailableDesigns)
        {
            if (towel.StartsWith(design))
            {
                result += GetCombinationCount(towel.Substring(design.Length));
            }
        }

        CombinationCounts.Add(towel, result);

        return result;
    }
}