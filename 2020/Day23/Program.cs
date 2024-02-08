// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day23\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day23\Full.txt";
#endif

    int[] values =
        Regex.Matches(File.ReadAllText(fileName), "\\d").
        Select(m => int.Parse(m.Value))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(values));
    Console.WriteLine("Part 2: " + Part2(values));
    Console.ReadLine();
}

string Part1(int[] labels)
{
    CrabCups game = new(labels);

    foreach(var _ in Enumerable.Range(1, 100))
    {
        game.PlayMove();
    }

    return string.Join("", game.GetLabelsBetween1(labels.Length - 1));
}

long Part2(int[] labels)
{
    int startAdd = labels.Max() + 1;
    int countAdd = 1_000_000 - labels.Length;
    int percentRange = 10_000_000 / 100;
    int[] extendedLabels = [.. labels, ..Enumerable.Range(startAdd, countAdd)];

    (int x, int y) cached = (0, 0);


    CrabCups game = new(extendedLabels);

    foreach (var moveNumber in Enumerable.Range(1, 10_000_000))
    {
        if (moveNumber % percentRange == 0)
        {
            Console.Write("\r" + moveNumber / percentRange + "%");
        }

        if (moveNumber % 10000 == 0)
        {
            var resultValuesTemp = game.GetLabelsBetween1(2);
            Console.WriteLine($"Move {moveNumber} => {resultValuesTemp[0]}-{resultValuesTemp[1]}  - {game.Current.Value}");
        }

        // var resultValuesTemp = game.GetLabelsBetween1(2);
        // var currentResult = (resultValuesTemp[0], resultValuesTemp[1]);

        /* if (currentResult != cached)
        {
            Console.WriteLine($"Move {moveNumber} => {cached} => {currentResult}");
            cached = currentResult;
        }
        */

        game.PlayMove();
    }

    Console.Write("\r");

    var resultValues = game.GetLabelsBetween1(2);

    return (long)resultValues[0] * (long)resultValues[1];
}


public class CrabCups
{
    int MaxValue;
    LinkedList<int> Cups;
    public LinkedListNode<int> Current;

    public CrabCups(int[] values)
    {
        Cups = new(values);
        MaxValue = values.Max();
        Current = Cups.First!;
    }

    public void PlayMove()
    {
        var picked = ExtractAfter(Current, 3);
        var toInsert = PickNextCurrent(picked);
        InsertAfter(toInsert, picked);
        Current = GetNextClockWise(Current);
    }

    LinkedListNode<int> PickNextCurrent(List<int> excluded)
    {
        int seed = Current.Value;
        for (int i = seed - 1; i > 0; i--)
        {
            if (excluded.Contains(i)) continue;

            var templeft = Current;
            var tempright = Current;
            
            do
            {
                templeft = GetNextCounterClockWise(templeft);
                tempright = GetNextClockWise(tempright);
                if (tempright.Value == i) return tempright;
                if (templeft.Value == i) return templeft;
            } while (templeft.Value != seed);
        }

        for (int i = MaxValue; i > seed; i--)
        {
            if (excluded.Contains(i)) continue;

            var templeft = Current;
            var tempright = Current;

            do
            {
                templeft = GetNextCounterClockWise(templeft);
                tempright = GetNextClockWise(tempright);
                if (tempright.Value == i) return tempright;
                if (templeft.Value == i) return templeft;
            } while (templeft.Value != seed);
        }

        throw new ApplicationException("No next current found");
    }

    public List<int> GetLabelsBetween1(int count)
    {
        var temp = Cups.First!;
        List<int> result = [];

        while (temp.Value != 1) temp = GetNextClockWise(temp);

        temp = GetNextClockWise(temp);

        foreach(var _ in Enumerable.Range(1, count))
        {
            if (temp.Value == 1) break;

            result.Add(temp.Value);
            temp = GetNextClockWise(temp);
        }

        return result;
    }

    List<int> ExtractAfter(LinkedListNode<int> item, int count)
    {
        List<int> result = [];

        item = GetNextClockWise(item);
        foreach (var _ in Enumerable.Range(1, count))
        {
            result.Add(item.Value);
            var toDelete = item;
            item = GetNextClockWise(item);
            Cups.Remove(toDelete);
        }

        return result;
    }
    void InsertAfter(LinkedListNode<int> item, List<int> toInsert)
    {
        foreach (var i in toInsert)
        {
            item = Cups.AddAfter(item, i);
        }

    }

    LinkedListNode<int> GetNextClockWise(LinkedListNode<int> item)
    {
        return item.Next ?? Cups.First!;
    }

    LinkedListNode<int> GetNextCounterClockWise(LinkedListNode<int> item)
    {
        return item.Previous ?? Cups.Last!;
    }
}