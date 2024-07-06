// #define Sample

using System.Diagnostics;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day13\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day13\Full.txt";
#endif

    PacketPair[] inputs = 
        File.ReadAllText(fileName)
        .ReplaceLineEndings("\n")
        .Split("\n\n")
        .Select(s => new PacketPair(s))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(inputs));
    Console.WriteLine("Part 2: " + Part2(inputs));
    Console.ReadLine();
}

long Part1(PacketPair[] inputs)
{
    return inputs
        .Select((pair, index) => (Pair: pair, Index: index + 1))
        .Where(item => item.Pair.IsInRightOrder())
        .Sum(item => item.Index);
}


long Part2(PacketPair[] inputs)
{
    Node separator1 = Node.Parse("[[2]]");
    Node separator2 = Node.Parse("[[6]]");
    
    List<Node> allNodes = inputs
        .SelectMany(pair => new Node[] { pair.FirstNode, pair.SecondNode })
        .ToList();

    allNodes.AddRange([separator1, separator2]);
    allNodes.Sort(new NodeComparer());

    int index1 = allNodes.IndexOf(separator1) + 1;
    int index2 = allNodes.IndexOf(separator2) + 1;

    return index1 * index2;
}


public class PacketPair
{
    public readonly Node FirstNode;
    public readonly Node SecondNode;

    public PacketPair(string input)
    {
        string[] splitted = input.Split("\n");
        FirstNode = Node.Parse(splitted[0]);
        SecondNode = Node.Parse(splitted[1]);
    }

    public bool IsInRightOrder()
    {
        return Node.Compare(FirstNode, SecondNode) < 0;
    }
}



public class Node
{
    public int Number;
    public List<Node> List = [];
    public bool IsNumber = false;

    public static int Compare(Node first, Node second)
    {
        if (first.IsNumber && second.IsNumber)
        {
            return CompareNumber(first, second);
        }

        if (!first.IsNumber && !second.IsNumber)
        {
            return CompareList(first.List, second.List);
        }

        return CompareMixed(first, second);
    }

    private static int CompareList(List<Node> first, List<Node> second)
    {
        int index = 0;

        while (true)
        {
            if (index >= first.Count && index >= second.Count) return 0;
            if (index >= first.Count) return -1;
            if (index >= second.Count) return +1;

            int result = Compare(first[index], second[index]);
            if (result != 0) return result;

            index++;
        }
    }

    private static int CompareNumber(Node first, Node second)
    {
        Trace.Assert(first.IsNumber);
        Trace.Assert(second.IsNumber);

        return first.Number.CompareTo(second.Number);
    }

    private static int CompareMixed(Node first, Node second)
    {
        if (first.IsNumber)
        {
            Trace.Assert(!second.IsNumber);
            return CompareList([first], second.List);
        }
        else
        {
            Trace.Assert(second.IsNumber);
            return CompareList(first.List, [second]);
        }
    }

    public static Node Parse(string input)
    {
        Token[] tokens = ScanTokens(input);
        return BuildNodeFromTokens(tokens);
    }


    private static Node BuildNodeFromTokens(Token[] tokens)
    {
        Stack<Node> stack = [];
        Node? lastRemoved = null;

        foreach (var token in tokens)
        {
            switch (token.TokenType)
            {
                case TokenType.Number:
                    Node numberNode = new Node { IsNumber = true, Number = token.ValueAsInt };
                    stack.Peek().List.Add(numberNode);
                    break;

                case TokenType.OpenList:
                    Node listNode = new Node { IsNumber = false };

                    if (stack.Any())
                    {
                        stack.Peek().List.Add(listNode);
                    }
                    stack.Push(listNode);
                    break;

                case TokenType.CloseList:
                    lastRemoved = stack.Pop();
                    break;
            }
        }

        Trace.Assert(lastRemoved != null);
        return lastRemoved;
    }

    private static Token[] ScanTokens(string formulaString)
    {
        List<Token> tokens = new();

        bool inNumber = false;
        string current = "";

        foreach (var ch in formulaString + " ")
        {
            if (ch is >= '0' and <= '9')
            {
                if (inNumber)
                {
                    current += ch;
                }
                else
                {
                    inNumber = true;
                    current = "" + ch;
                }
            }
            else
            {
                if (inNumber)
                {
                    inNumber = false;
                    tokens.Add(new Token { TokenType = TokenType.Number, Value = current });
                    current = "";
                }
            }

            if (ch is ',') tokens.Add(new Token { TokenType = TokenType.Comma });
            if (ch is '[') tokens.Add(new Token { TokenType = TokenType.OpenList });
            if (ch is ']') tokens.Add(new Token { TokenType = TokenType.CloseList });
        }

        return tokens.ToArray();
    }

}

public class NodeComparer : IComparer<Node>
{
    public int Compare(Node? x, Node? y)
    {
        return Node.Compare(x!, y!);  
    }
}

public enum TokenType
{
    Number,
    Comma,
    OpenList,
    CloseList,
}

public record struct Token
{
    public TokenType TokenType;
    public string Value;
    public int ValueAsInt => int.Parse(Value);
}