// #define Sample

using System.Collections.Frozen;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day19\Full.txt";
#endif

    int elvesCount = int.Parse(File.ReadAllText(fileName));

    Console.WriteLine("Part 1: " + Part1(elvesCount));
    Console.WriteLine("Part 2: " + Part2(elvesCount));
    Console.ReadLine();
}

int Part1(int elvesCount)
{
    LinkedList<int> elves = BuildElvesCircle(elvesCount);
    
    var current = elves.First!;

    while (elves.Count > 1)
    {
        var leftElf = GetNext(elves, current);
        elves.Remove(leftElf);
        current = GetNext(elves, current);
    }

    return current.Value; 
}

int Part2(int elvesCount)
{
    LinkedList<int> elves = BuildElvesCircle(elvesCount);

    var current = elves.First!;
    var currentAcross = GetAccross(elves, current);

    while (elves.Count > 1)
    {
        var acrossElf = currentAcross;

        currentAcross = GetNext(elves, currentAcross);
        if (elves.Count % 2 == 1) currentAcross = GetNext(elves, currentAcross);

        elves.Remove(acrossElf);

        current = GetNext(elves, current);
    }

    return current.Value;
}

static LinkedList<int> BuildElvesCircle(int elvesCount)
{
    LinkedList<int> elves = [];

    foreach (int i in Enumerable.Range(1, elvesCount))
    {
        elves.AddLast(i);
    }

    return elves;
}

LinkedListNode<int> GetNext(LinkedList<int> elves, LinkedListNode<int> current)
{
    return current.Next ?? elves.First!;
}

LinkedListNode<int> GetAccross(LinkedList<int> elves, LinkedListNode<int> current)
{
    int moves = elves.Count / 2;

    var next = current;

    foreach(var _ in Enumerable.Range(1, moves))
    {
        next = GetNext(elves, next);
    }

    return next;
}
