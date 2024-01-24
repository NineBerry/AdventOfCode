// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day24\Full.txt";
#endif

    HashSet<Component> components = File.ReadAllLines(fileName).Select(s => new Component(s)).ToHashSet();

    IEnumerable<List<Component>> bridges = BuildBridges([], 0, components);

    Console.WriteLine("Part 1: " + Part1(bridges));
    Console.WriteLine("Part 2: " + Part2(bridges));
    Console.ReadLine();
}

long Part1(IEnumerable<List<Component>> bridges)
{
    return bridges.Max(b => b.GetStrength());
}

long Part2(IEnumerable<List<Component>> bridges)
{
    int maxLength = bridges.Max(b => b.Count);
    return bridges.Where(b => b.Count == maxLength).Max(b => b.GetStrength());
}


IEnumerable<List<Component>> BuildBridges(List<Component> soFar, int connectionPort, HashSet<Component> remainingComponents)
{
    List<List<Component>> bridges = [];

    foreach (var component in remainingComponents)
    {
        if(component.Fits(connectionPort, out int otherPort))
        {
            bridges.AddRange(BuildBridges([..soFar, component], otherPort, remainingComponents.Except([component]).ToHashSet()));
        }
    }

    if (!bridges.Any())
    {
        if (soFar.Any()) bridges.Add(soFar);
    }

    return bridges;
}

public class Component
{
    public Component(string input)
    {
        int[] values = input.Split('/').Select(s => int.Parse(s)).ToArray();
        Port1 = values[0];
        Port2 = values[1];
    }

    public bool Fits(int connectionPort, out int otherPort)
    {
        if (Port1 == connectionPort)
        {
            otherPort = Port2;
            return true;
        }
        else if (Port2 == connectionPort)
        {
            otherPort = Port1;
            return true;
        }
        else
        {
            otherPort = 0;
            return false;
        }
    }

    public int Port1;
    public int Port2;

    public int Strength => Port1 + Port2;
}

public static class Extensions
{
    public static int GetStrength(this List<Component> bridge)
    {
        return bridge.Sum(c => c.Strength);
    }
}