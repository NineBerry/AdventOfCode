// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day08\Full.txt";
#endif

    string input = File.ReadAllText(fileName);
    var values = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value));
    Queue<int> queue = new Queue<int>(values);

    Node tree = Node.ReadFrom(queue);

    Console.WriteLine("Part 1: " + Part1(tree));
    Console.WriteLine("Part 2: " + Part2(tree));
    Console.ReadLine();
}

long Part1(Node tree)
{
    return tree.GetRecursiveMetaDataSum();
}
long Part2(Node tree)
{
    return tree.GetValue();
}

public class Node
{
    public static Node ReadFrom(Queue<int> queue)
    {
        Node node = new Node();

        int countChildren = queue.Dequeue();
        int countMetaData = queue.Dequeue();

        for(int i=1; i <= countChildren; i++)
        {
            node.Children.Add(Node.ReadFrom(queue));
        }

        for (int i = 1; i <= countMetaData; i++)
        {
            node.MetaData.Add(queue.Dequeue());
        }

        return node;
    }

    public List<Node> Children = [];
    public List<int> MetaData = [];

    public long GetRecursiveMetaDataSum()
    {
        long sum = 0;   

        foreach (Node node in Children)
        {
            sum += node.GetRecursiveMetaDataSum();
        }

        sum += MetaData.Sum();

        return sum; 
    }
    public long GetValue()
    {
        long sum = 0;

        if(Children.Count == 0)
        {
            sum += MetaData.Sum();
        }
        else
        {
            foreach (int metaData in MetaData)
            {
                sum += GetChildByNumber(metaData)?.GetValue() ?? 0;
            }
        }

        return sum;
    }

    public Node GetChildByNumber(int number)
    {
        if (number <= 0) return null;
        if (number > Children.Count) return null;
        return Children[number - 1];
    }
}