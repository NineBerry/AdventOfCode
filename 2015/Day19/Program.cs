#define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day19\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Replacement[] replacements = input.TakeWhile(s => s != "").Select(s => new Replacement(s)).ToArray();
    string molecule = input.Last();

    Console.WriteLine("Part 1: " + Part1(molecule, replacements));
    Console.WriteLine("Part 2: " + Part2(molecule, replacements));
    Console.ReadLine();
}

long Part1(string originalMolecule, Replacement[] replacements)
{
    HashSet<string> possibleMolecules = FabricateMolecules([originalMolecule] , replacements);
   
    return possibleMolecules.Count;
}

long Part2(string targetMolecule, Replacement[] replacements)
{
    HashSet<string> seen = new HashSet<string>();
    replacements = replacements.Select(re => new Replacement(re.To, re.From)).ToArray();

    DateTime start = DateTime.Now;
    int steps = ReduceTo(targetMolecule, "e", 0, replacements, seen);
    int duration = (int)(DateTime.Now - start).TotalSeconds;

    Console.WriteLine($"Taken {duration} seconds");

    return steps;
}

int ReduceTo (string molecule, string reduceTo, int steps, Replacement[] replacements, HashSet<string> seen)
{
    if(seen.Contains(molecule)) return 0;
    seen.Add(molecule);

    if (molecule == reduceTo) return steps;

    for (int i = 0; i < molecule.Length; i++)
    {
        foreach (var replacement in replacements)
        {
            if (molecule.Substring(i).StartsWith(replacement.From))
            {
                string newMolecule =
                      molecule.Substring(0, i)
                    + replacement.To
                    + molecule.Substring(i + replacement.From.Length);

                int result = ReduceTo(newMolecule, reduceTo, steps + 1, replacements, seen);
                if(result > 0) return result;
            }
        }
    }

    return 0;
}

HashSet<string> FabricateMolecules(HashSet<string> sourceMolecules, Replacement[] replacements)
{
    HashSet<string> possibleMolecules = [];

    foreach (var originalMolecule in sourceMolecules)
    {
        for (int i = 0; i < originalMolecule.Length; i++)
        {
            foreach (var replacement in replacements)
            {
                if (originalMolecule.Substring(i).StartsWith(replacement.From))
                {
                    string newMolecule =
                          originalMolecule.Substring(0, i)
                        + replacement.To
                        + originalMolecule.Substring(i + replacement.From.Length);
                    possibleMolecules.Add(newMolecule);
                }
            }
        }
    }

    return possibleMolecules;
}

public record Replacement
{
    public Replacement(string from, string to)
    {
        From = from;
        To = to;
    }

    public Replacement(string input)
    {
        var parts = input.Split("=>", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        From = parts[0];    
        To = parts[1];  
    }
    public string From;
    public string To;
}