// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day12\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Network network = new Network(input);

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 2: " + Part2(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    return network.GetAllPaths().Length;
}

long Part2(Network network)
{
    return network.GetAllPathsWithMoreTime().Length;
}

public class Network
{
    Dictionary<string, Node> Nodes = [];

    public Network(string[] input) 
    { 
        foreach (var line in input)
        {
            string[] parts = line.Split('-');

            var first = GetNode(parts[0]);
            var second = GetNode(parts[1]);

            first.Others.Add(second);
            second.Others.Add(first);
        }
    }

    public Node GetNode(string name)
    {
        if(!Nodes.TryGetValue(name, out var node))
        {
            node = new Node(name);
            Nodes.Add(name, node);
        }

        return node;
    }

    public Node[][] GetAllPaths()
    {
        return GetPathsRecursive([GetNode("start")]);
    }

    private Node[][] GetPathsRecursive(Node[] currentPath)
    {
        Node last = currentPath.Last();
        if (last.Name == "end") return [currentPath];

        List<Node[]> list = [];

        foreach(var other in last.Others)
        {
            if(other.IsBig || !currentPath.Contains(other))
            {
                list.AddRange(GetPathsRecursive([..currentPath, other]));
            }
        }

        return list.ToArray();
    }

    public Node[][] GetAllPathsWithMoreTime()
    {
        return GetPathsWithMoreTimeRecursive([GetNode("start")], null);
    }

    private Node[][] GetPathsWithMoreTimeRecursive(Node[] currentPath, Node? doubleVisitedCave)
    {
        Node last = currentPath.Last();
        if (last.Name == "end") return [currentPath];

        List<Node[]> list = [];

        foreach (var other in last.Others)
        {
            if (other.Name == "start") continue;

            if (other.IsBig)
            {
                list.AddRange(GetPathsWithMoreTimeRecursive([.. currentPath, other], doubleVisitedCave));
            }
            else
            {
                if (currentPath.Contains(other))
                {
                    if(doubleVisitedCave == null && currentPath.Count(n => n == other) == 1)
                    {
                        list.AddRange(GetPathsWithMoreTimeRecursive([.. currentPath, other], other));
                    }
                }
                else
                {
                    list.AddRange(GetPathsWithMoreTimeRecursive([.. currentPath, other], doubleVisitedCave));
                }
            }
        }

        return list.ToArray();
    }

}

public record Node
{
    public Node(string name)
    {
        Name = name;
        IsBig = char.IsUpper(name[0]);
    }

    public string Name;
    public List<Node> Others = [];
    public bool IsBig;
}