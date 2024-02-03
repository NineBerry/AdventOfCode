// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day06\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    Network network = new Network(lines);

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 1: " + Part2(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    return network.GetOrbitCounts();
}
long Part2(Network network)
{
    var YouPath = network.GetParentPath("YOU");
    var SantaPath = network.GetParentPath("SAN");

    while(YouPath.Last() == SantaPath.Last())
    {
        YouPath.Remove(YouPath.Last());
        SantaPath.Remove(SantaPath.Last());
    }

    return YouPath.Count+ SantaPath.Count;
}

public class Network
{
    public Network(string[] lines) 
    { 
        foreach (var line in lines)
        {
            string[] parts = line.Split(')');

            Satellite parent = GetSatellite(parts[0]);
            Satellite child = GetSatellite(parts[1]);

            parent.Children.Add(child);
            child.Parent = parent;
        }
    }

    private Dictionary<string, Satellite> Satellites = [];

    private Satellite GetSatellite(string name)
    {
        if (!Satellites.TryGetValue(name, out var satellite)) 
        {
            satellite = new Satellite(name);
            Satellites.Add(name, satellite);
        }
        return satellite;
    }

    public int GetOrbitCounts()
    {
        Satellite root = GetSatellite("COM");

        return GetOrbitCountsRecursive(root, 0);

        int GetOrbitCountsRecursive(Satellite satellite, int depth)
        {
            return depth + satellite.Children.Select(s => GetOrbitCountsRecursive(s, depth + 1)).Sum();
        }
    }

    public List<string> GetParentPath(string satelliteName)
    {
        List<string> result = [];
        Satellite satellte = GetSatellite(satelliteName);

        while (satellte.Parent != null) 
        {
            result.Add(satellte.Parent.Name);
            satellte = satellte.Parent;
        }

        return result;
    }

}

public class Satellite
{
    public Satellite(string name)
    {
        Name = name;
    }

    public string Name;
    public Satellite? Parent;
    public List<Satellite> Children = [];
}