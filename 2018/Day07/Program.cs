// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day07\Sample.txt";
    int durationOffset = 0;
    int workersPart2 = 2;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day07\Full.txt";
    int durationOffset = 60;
    int workersPart2 = 5;
#endif

    string[] input = File.ReadAllLines(fileName);

    Network network = new Network(input);
    Console.WriteLine("Part 1: " + Part1(network));

    Network network2 = new Network(input);
    Console.WriteLine("Part 2: " + Part2(network2, durationOffset, workersPart2));

    Console.ReadLine();
}

string Part1(Network network)
{
    string result = "";

    Node? node = network.GetNextRoot();
    while(node != null)
    {
        result += node.Name;
        
        network.RemoveFromNetwork(node);
        node = network.GetNextRoot();
    }

    return result;
}

int Part2(Network network, int durationOffset, int workersPart2)
{
    Worker[] workers = 
        Enumerable.Range(1, workersPart2)
        .Select(i => new Worker(network, durationOffset))
        .ToArray();
    
    int seconds = 0;

    while (network.Any())
    {
        foreach (Worker worker in workers)
        {
            worker.GetNewTask();
        }
        
        foreach (Worker worker in workers)
        {
            worker.DoWork();
        }

        seconds++;
    }

    return seconds;
}

public class Worker
{
    public Worker(Network network, int durationOffset)
    {
        Network = network;  
        DurationOffset = durationOffset;
    }

    public void DoWork()
    {
        if(CurrentTask != null)
        {
            RemainingWork--;

            if(RemainingWork <= 0)
            {
                Network.RemoveFromNetwork(CurrentTask);
                CurrentTask = null;
            }
        }
    }

    public void GetNewTask()
    {
        if (CurrentTask == null)
        {
            Node? node = Network.GetNextRoot();
            if (node != null)
            {
                CurrentTask = node;
                RemainingWork = DurationOffset + node.GetDuration();
            }
        }
    }

    public Node? CurrentTask = null;
    public int RemainingWork = 0;
    public Network Network;
    public int DurationOffset;
}

public class Network: Dictionary<string, Node>
{
    public Network(string[] input)
    {
        foreach(var line in input)
        {
            string[] matches = Regex.Matches(line, " ([A-Z]) ").Select(m => m.Groups[1].Value).ToArray();

            var node = GetNode(matches[1]);
            var depended = GetNode(matches[0]);

            node.DependentOn.Add(depended);
        }
    }

    public Node? GetNextRoot()
    {
        Node? next = 
            Values
            .Where(v => !v.Busy)
            .Where(v => v.DependentOn.Count == 0)
            .OrderBy(v => v.Name)
            .FirstOrDefault();

        if (next != null)
        {
            next.Busy = true;
        }
        return next;
    }

    public void RemoveFromNetwork(Node removeNode)
    {
        foreach(var node in this.Values)
        {
            node.DependentOn.Remove(removeNode);
        }

        this.Remove(removeNode.Name);
    }

    public Node GetNode(string name)
    {
        if(!TryGetValue(name, out Node? node))
        {
            node = new Node(name);
            Add(name, node);
        }

        return node;
    }
}

public class Node 
{
    public Node(string name)
    {
        Name = name;
    }

    public string Name;
    public HashSet<Node> DependentOn = [];
    
    public bool Busy = false;
    public int GetDuration() => Name[0] - 'A' + 1;
}