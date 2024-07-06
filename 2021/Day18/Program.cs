// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day18\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day18\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    Node[] nodes = lines.Select(Node.Parse).ToArray();
    Node sum = nodes.First();

    foreach (Node node in nodes.Skip(1))
    {
        sum = Node.AddNodes(sum, node);
    }

    return sum.GetMagnitude();
}

long Part2(string[] lines)
{
    long maxMagnitude = 0;

    foreach(var i in lines)
    {
        foreach (var j in lines)
        {
            if (i == j) continue;

            Node node1 = Node.Parse(i);
            Node node2 = Node.Parse(j);

            Node combined = Node.AddNodes(node1, node2);
            long magnitude = combined.GetMagnitude();   

            maxMagnitude = Math.Max(maxMagnitude, magnitude);
        }
    }

    return maxMagnitude;
}

public class Node
{
    public int Number;
    public List<Node> List = [];
    public bool IsNumber = false;

    public Node Left => List[0];
    public Node Right => List[1];

    public Node? Parent = null;

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
                    {
                        Node numberNode = new Node { IsNumber = true, Number = token.ValueAsInt };
                        Node parent = stack.Peek();
                        parent.List.Add(numberNode);
                        numberNode.Parent = parent;
                        break;
                    }

                case TokenType.OpenList:
                    Node listNode = new Node { IsNumber = false };

                    if (stack.Any())
                    {
                        Node parent = stack.Peek();
                        parent.List.Add(listNode);
                        listNode.Parent = parent;
                    }
                    stack.Push(listNode);
                    break;

                case TokenType.CloseList:
                    lastRemoved = stack.Pop();
                    break;
            }
        }

        return lastRemoved!;
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

    public static Node AddNodes(Node node1, Node node2)
    {
        var result = AddNodesInternal(node1, node2);
        return ReduceNode(result);
    }

    private static Node AddNodesInternal(Node node1, Node node2)
    {
        Node parent = new Node { IsNumber = false, List = [node1, node2] };
        node1.Parent = parent;
        node2.Parent = parent;
        return parent;
    }

    private static Node ReduceNode(Node node)
    {
        bool actionPerformed;

        do
        {
            actionPerformed = false;

            Node? pair = FindPairToExplode(node);
            if(pair != null)
            {
                node = ExplodePair(node, pair);
                actionPerformed = true;
            }
            else
            {
                pair = FindPairToSplit(node);
                if (pair != null)
                {
                    node = SplitPair(node, pair);
                    actionPerformed = true;
                }
            }
        } while (actionPerformed);

        return node;
    }

    private static Node? FindPairToExplode(Node node, int depth = 0)
    {
        if (node.IsNumber) return null;

        if (depth == 4) return node;
        
        Node? result = FindPairToExplode(node.Left, depth + 1);
        if (result != null) return result;

        return FindPairToExplode(node.Right, depth + 1);
    }

    private static Node ExplodePair(Node root, Node pair)
    {
        Node zero = new Node { IsNumber = true, Number = 0 };
        if(root == pair) return zero;

        int leftValue = pair.Left.Number;
        int rightValue = pair.Right.Number;

        Node? leftNumber = FindLeftNumber(pair, goingUp: true);
        if (leftNumber != null) leftNumber.Number += leftValue;

        Node? rightNumber = FindRightNumber(pair, goingUp: true);
        if (rightNumber != null) rightNumber.Number += rightValue;

        if (pair.Parent!.List[0] == pair) pair.Parent.List[0] = zero;
        if (pair.Parent!.List[1] == pair) pair.Parent.List[1] = zero;
        zero.Parent = pair.Parent;

        return root;
    }

    private static Node? FindLeftNumber(Node pair, bool goingUp)
    {
        if(pair.IsNumber) return pair;

        if (goingUp)
        {
            if(pair == null) return null;
            if(pair.Parent == null) return null;

            Node? result = null;
            if (pair.Parent.Right == pair)
            {
                result = FindLeftNumber(pair.Parent.Left, goingUp: false);
                if (result != null) return result;
            }

            return FindLeftNumber(pair.Parent, goingUp: true);
        }
        else
        {
            Node? result = FindLeftNumber(pair.Right, goingUp: false);
            if (result != null) return result;

            return FindLeftNumber(pair.Left, goingUp: false);
        }
    }

    private static Node? FindRightNumber(Node pair, bool goingUp)
    {
        if (pair.IsNumber) return pair;

        if (goingUp)
        {
            if (pair == null) return null;
            if (pair.Parent == null) return null;

            Node? result = null;
            if (pair.Parent.Left == pair)
            {
                result = FindRightNumber(pair.Parent.Right, goingUp: false);
                if (result != null) return result;
            }

            return FindRightNumber(pair.Parent, goingUp: true);
        }
        else
        {
            Node? result = FindRightNumber(pair.Left, goingUp: false);
            if (result != null) return result;

            return FindRightNumber(pair.Right, goingUp: false);
        }
    }


    private static Node? FindPairToSplit(Node node)
    {
        if (node.IsNumber)
        {
            if (node.Number >= 10) return node;
            return null;
        }
        else
        {
            Node? result = FindPairToSplit(node.Left);
            if (result != null) return result;

            return FindPairToSplit(node.Right);
        }
    }

    private static Node SplitPair(Node root, Node pair)
    {
        int number = pair.Number;
        int newLeftNumber = (int)Math.Floor(number / 2.0);
        int newRightNumber = (int)Math.Ceiling(number / 2.0);
        Node newLeftNode = new Node { IsNumber = true, Number = newLeftNumber};
        Node newRightNode = new Node { IsNumber = true, Number = newRightNumber};

        Node newPair = new Node { IsNumber = false, List = [newLeftNode, newRightNode] };
        newLeftNode.Parent = newPair;
        newRightNode.Parent = newPair;

        if (root == pair) return newPair;

        if (pair.Parent!.Left == pair) pair.Parent.List[0] = newPair;
        if (pair.Parent!.Right == pair) pair.Parent.List[1] = newPair;
        newPair.Parent = pair.Parent;

        return root;
    }

    public long GetMagnitude()
    {
        if(IsNumber) return Number;
        return 3 * Left.GetMagnitude() + 2 * Right.GetMagnitude();
    }

    public override string ToString()
    {
        if (IsNumber) return $"{Number}";
        return $"[{Left},{Right}]";
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