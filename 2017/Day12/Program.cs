// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day12\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    Network network = Network.BuildNetwork(lines);

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 2: " + Part2(network));
    Console.ReadLine();
}

int Part1(Network network)
{
    return network.GetConnectedNodes("0").Count;
}

int Part2(Network network)
{
    int count = 0;
    HashSet<string> remaining = network.GetAllNodes();

    while (remaining.Any())
    {
        count++;
        remaining.ExceptWith(network.GetConnectedNodes(remaining.First()));
    }

    return count;   
}

public record Edge
{
    public Edge(string first, string second)
    {
        if (first.CompareTo(second) > 0)
        {
            First = first;
            Second = second;
        }
        else
        {
            First = second;
            Second = first;
        }
    }

    public readonly string First;
    public readonly string Second;
}

public class Network : Dictionary<string, HashSet<string>>
{
    public static Network BuildNetwork(string[] input)
    {
        Network network = new Network();

        foreach (string line in input)
        {
            var components = Regex.Matches(line, "[0-9]+").Select(m => m.Value).ToArray();

            string first = components.First();
            foreach (var second in components.Skip(1))
            {
                network.AddEdge(new Edge(first, second));
            }
        }

        return network;
    }

    public void AddEdge(Edge edge)
    {
        if (!TryGetValue(edge.First, out var connections))
        {
            connections = new();
            this[edge.First] = connections;
        }

        connections.Add(edge.Second);

        if (!TryGetValue(edge.Second, out connections))
        {
            connections = new();
            this[edge.Second] = connections;
        }

        connections.Add(edge.First);
    }

    public HashSet<string> GetConnectedNodes(string node)
    {
        Stack<string> stack = new();
        HashSet<string> visited = new();

        stack.Push(node);

        while (stack.TryPop(out var nextNode))
        {
            if (visited.Contains(nextNode)) continue;
            visited.Add(nextNode);

            foreach (var connectedNode in this[nextNode])
            {
                if (!visited.Contains(connectedNode))
                {
                    stack.Push(connectedNode);
                }
            }
        }

        return visited;
    }

    public HashSet<string> GetAllNodes()
    {
        return this.Keys.ToHashSet();
    }
}