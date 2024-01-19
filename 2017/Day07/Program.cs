// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day07\Full.txt";
#endif

    Network network = new Network(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 2: " + Part2(network));
    Console.ReadLine();
}

string Part1(Network network)
{
    return network.GetRoot().Name;
}

int Part2(Network network)
{
    Node? unbalancedParentNode = null;

    while (unbalancedParentNode == null)
    {
        unbalancedParentNode = network.ReduceOneNode();
    }

    // Get childnot with differing weight.
    var errorNode =
        unbalancedParentNode.Children
        .GroupBy(n => n.CombinedWeigth)
        .Single(g => g.Count() == 1)
        .Single();
    
    // Get regular node
    var regularNode = 
        unbalancedParentNode.Children
        .First(n => n != errorNode);

    // Calculate difference and needed weight for the error node
    int difference = errorNode.CombinedWeigth - regularNode.CombinedWeigth;
    int expectedWeight = errorNode.OwnWeight - difference; 

    return expectedWeight; 

}

public class Network
{
    public Network(string[] lines)
    {
        foreach(var line in lines)
        {
            int weight = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).Single();
            string[] names = Regex.Matches(line, "[a-z]+").Select(m => m.Value).ToArray();

            Node current = GetNode(names.First(), weight);

            foreach(var childName in names.Skip(1)) 
            {
                Node childNode = GetNode(childName, 0);
                current.Children.Add(childNode);
                childNode.Parent = current;
            }
        }
    }

    public Dictionary<string, Node> Nodes = [];

    private Node GetNode(string name, int weight)
    {
        if(!Nodes.TryGetValue(name, out Node? node))
        {
            node = new Node(name);
            Nodes.Add(name, node);  
        }

        if (weight != 0)
        {
            node.OwnWeight = weight;
        }

        return node;
    }

    public Node GetRoot()
    {
        return Nodes.Values.Where(n => n.Parent == null).Single();
    }

    internal Node? ReduceOneNode()
    {
        Node toBalance = Nodes.Values.Where(n => n.Children.Any() && n.Children.All(cn => cn.Children.Count == 0)).First();

        int expectedChildWeight = toBalance.Children.First().CombinedWeigth;

        if(toBalance.Children.All(cn => cn.CombinedWeigth == expectedChildWeight))
        {
            toBalance.ChildWeights = toBalance.Children.Count * expectedChildWeight;

            foreach(var child in toBalance.Children)
            {
                Nodes.Remove(child.Name);
            }

            toBalance.Children.Clear();

            return null;
        }
        else
        {
            return toBalance;
        }
    }
}

public class Node
{
    public Node(string name)
    {
        Name = name;
    }

    public List<Node> Children = [];
    public Node? Parent = null;

    public string Name;
    public int OwnWeight;
    public int ChildWeights;

    public int CombinedWeigth => OwnWeight + ChildWeights;
}