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
    int[] extendedLabels = [.. labels, ..Enumerable.Range(startAdd, countAdd)];

    CrabCups game = new(extendedLabels);

    foreach (var _ in Enumerable.Range(1, 10_000_000))
    {
        game.PlayMove();
    }

    var resultValues = game.GetLabelsBetween1(2);
    return (long)resultValues[0] * (long)resultValues[1];
}


public class CrabCups
{
    int MaxValue;
    int[] ValueAfter;
    public int Current;

    public CrabCups(int[] values)
    {
        MaxValue = values.Max();
        Current = values.First();

        ValueAfter = new int[MaxValue + 1];
        int previous = 0;
        foreach(var value in values) 
        {
            ValueAfter[previous] = value;
            previous = value;
        }
        
        ValueAfter[values.Last()] = values.First();
        ValueAfter[0] = 0;
    }

    public void PlayMove()
    {
        var picked = ExtractAfter(Current, 3);
        var toInsert = PickNextCurrent(picked);
        InsertAfter(toInsert, picked);
        Current = GetNextClockWise(Current);
    }

    int PickNextCurrent(List<int> excluded)
    {
        int seed = Current;
        for (int i = seed - 1; i > 0; i--)
        {
            if (excluded.Contains(i)) continue;
            return i;
        }

        for (int i = MaxValue; i > seed; i--)
        {
            if (excluded.Contains(i)) continue;
            return i;
        }

        throw new ApplicationException("No next current found");
    }

    public List<int> GetLabelsBetween1(int count)
    {
        var item = GetNextClockWise(1);
        List<int> result = [];

        foreach(var _ in Enumerable.Range(1, count))
        {
            result.Add(item);
            item = GetNextClockWise(item);
        }

        return result;
    }

    List<int> ExtractAfter(int item, int count)
    {
        var originalItem = item;
        List<int> result = [];

        item = GetNextClockWise(item);
        foreach (var _ in Enumerable.Range(1, count))
        {
            result.Add(item);
            int previous = item;
            item = GetNextClockWise(item);
            ValueAfter[previous] = -1;
        }

        ValueAfter[originalItem] = item;

        return result;
    }
    void InsertAfter(int item, List<int> toInsert)
    {
        int originalNext = ValueAfter[item];
        int previous = item;
        
        foreach (var i in toInsert)
        {
            ValueAfter[previous] = i;
            previous = i;
        }

        ValueAfter[previous] = originalNext;
    }

    int GetNextClockWise(int item)
    {
        return ValueAfter[item];
    }
}