// #define Sample
using System.Text.RegularExpressions;
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day25\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Network network = Network.BuildNetwork(input);

    Console.WriteLine("Part 1: " + Part1(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    Random random = new Random();
    EdgeStatistic edgeStatistic = new();
    var nodes = network.Keys.ToArray();
    int groupCount = nodes.Length;

    while (groupCount == nodes.Length)
    {
        string start = nodes[random.Next(nodes.Length)];
        string end = nodes[random.Next(nodes.Length)];

        var foundPath = network.RandomWalk(start, end, random);

        if (foundPath is not [])
        {
            edgeStatistic.AddPath(foundPath);
            var topEdges = edgeStatistic.GetTop(3);

            var cutNetwork = network.Clone();
            cutNetwork.RemoveEdges(topEdges);
            groupCount = cutNetwork.CountConnectedNodes(cutNetwork.First().Key);
        }
    }

    return groupCount * (nodes.Length - groupCount);
}

/// <summary>
/// Record Edge
/// </summary>
public record Edge
{
    public Edge(string first, string second)
    {
        if(first.CompareTo(second) > 0)
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

/// <summary>
/// Class EdgeStatistic
/// </summary>

public class EdgeStatistic : Dictionary<Edge, int>
{
    public void AddEdge(Edge edge)
    {
        if (!TryGetValue(edge, out int count))
        {
            count = 0;
        }

        this[edge] = count + 1;
    }

    public void AddPath(IEnumerable<string> foundPath)
    {
        foreach (var pair in foundPath.Skip(1).Zip(foundPath))
        {
            AddEdge(new Edge(pair.First, pair.Second));
        }
    }

    public IEnumerable<Edge> GetTop(int count)
    {
        return this.OrderByDescending(p => p.Value)
        .Take(count)
        .Select(p => p.Key);
    }
}

/// <summary>
/// Class Network
/// </summary>

public class Network: Dictionary<string, HashSet<string>>
{
    public static Network BuildNetwork(string[] input)
    {
        Network network = new Network();

        foreach (string line in input)
        {
            var components = Regex.Matches(line, "[a-z]{3}").Select(m => m.Value).ToArray();

            string first = components.First();
            foreach (var second in components.Skip(1))
            {
                network.AddEdge(new Edge(first, second));
            }
        }

        return network;
    }

    public Network Clone()
    {
        var clone = new Network();

        foreach (var pair in this)
        {
            clone.Add(pair.Key, new HashSet<string>(pair.Value));
        }

        return clone;
    }

    public void RemoveEdges(IEnumerable<Edge> edges)
    {
        foreach(var edge in edges)
        {
            RemoveEdge(edge);
        }
    }

    public void RemoveEdge(Edge edge)
    {
        if (TryGetValue(edge.First, out var connections))
        {
            connections.Remove(edge.Second);
        }

        if (TryGetValue(edge.Second, out connections))
        {
            connections.Remove(edge.First);
        }
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

    /// <summary>
    /// Not used. Doing RandomWalk is faster
    /// </summary>
    public List<string> RunDykstra(string start, string end)
    {
        var queue = new PriorityQueue<(string NextNode, HashSet<string> Visited, List<string> CurrentPath), int>();

        queue.Enqueue((start, [], [start]), 1);

        while (queue.TryDequeue(out var path, out _))
        {
            if (path.NextNode == end)
            {
                return path.CurrentPath;
            }

            if (path.Visited.Contains(path.NextNode)) continue;

            foreach (var node in this[path.NextNode])
            {
                queue.Enqueue((node, [.. path.Visited, path.NextNode], [.. path.CurrentPath, node]), path.CurrentPath.Count + 1);
            }
        }

        return [];
    }

    public List<string> RandomWalk(string start, string end, Random rnd)
    {
        string nextNode = start;
        HashSet<string> visited = [];
        List<string> currentPath = [start];

        while (true)
        {
            if (nextNode == end) return currentPath;

            var possibleNodes = this[nextNode].Except(visited).Except([nextNode]).ToArray();

            if (possibleNodes.Length == 0) break;

            string node = possibleNodes[rnd.Next(possibleNodes.Length)];

            currentPath.Add(nextNode);
            nextNode = node;
            visited.Add(node);
        }

        return [];
    }

    public int CountConnectedNodes(string node)
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

        return visited.Count;
    }
}