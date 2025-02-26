// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day20\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day20\Full.txt";
#endif

    long[] numbers = File.ReadAllLines(fileName).Select(long.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(numbers));
    Console.WriteLine("Part 2: " + Part2(numbers));
    Console.ReadLine();
}

long Part1(long[] numbers)
{
    return Mix(numbers, 1, 1);
}

long Part2(long[] numbers)
{
    return Mix(numbers, 811589153, 10);
}

long Mix(long[] numbers, int decryptionKey, int loops)
{
    numbers = numbers.Select(n => n * decryptionKey).ToArray();

    var list = CreateLinkedList(numbers);
    List<LinkedListNode<long>> todos = ExtractNodes(list);

    foreach (var _ in Enumerable.Range(1, loops))
    {
        foreach (var node in todos)
        {
            if (node.Value > 0)
            {
                var right = GetNextClockWise(list, node);
                list.Remove(node);
                var next = GetNextClockWiseCount(list, right, node.Value - 1);
                list.AddAfter(next, node);
            }
            else if (node.Value < 0)
            {
                var left = GetNextCounterClockWise(list, node);
                list.Remove(node);
                var next = GetNextCounterClockWiseCount(list, left, Math.Abs(node.Value) - 1);
                list.AddBefore(next, node);
            }
        }
    }

    var zeroNode = list.Find(0)!;

    var _1000Node = GetNextClockWiseCount(list, zeroNode, 1000);
    var _2000Node = GetNextClockWiseCount(list, _1000Node, 1000);
    var _3000Node = GetNextClockWiseCount(list, _2000Node, 1000);
    
    return _1000Node.Value + _2000Node.Value + _3000Node.Value;
}


LinkedList<long> CreateLinkedList(long[] numbers)
{
    LinkedList<long> list = new LinkedList<long>(numbers);
    return list;
}

List<LinkedListNode<long>> ExtractNodes(LinkedList<long> list)
{
    List<LinkedListNode<long>> nodes = new List<LinkedListNode<long>>();
    for(var node=list.First!; node != null; node = node.Next!)
    {
        nodes.Add(node);
    }
    return nodes;
}

LinkedListNode<long> GetNextClockWiseCount(LinkedList<long> numbers, LinkedListNode<long> current, long count)
{
    int rest = (int)(count % numbers.Count);

    foreach (var _ in Enumerable.Range(1, rest))
    {
        current = GetNextClockWise(numbers, current);
    }

    return current;
}


LinkedListNode<long> GetNextClockWise(LinkedList<long> numbers, LinkedListNode<long> current)
{
    return current.Next ?? numbers.First!;
}

LinkedListNode<long> GetNextCounterClockWiseCount(LinkedList<long> numbers, LinkedListNode<long> current, long count)
{
    int rest = (int)(count % numbers.Count);
    foreach (var _ in Enumerable.Range(1, rest))
    {
        current = GetNextCounterClockWise(numbers, current);
    }

    return current;
}

LinkedListNode<long> GetNextCounterClockWise(LinkedList<long> numbers, LinkedListNode<long> current)
{
    return current.Previous ?? numbers.Last!;
}