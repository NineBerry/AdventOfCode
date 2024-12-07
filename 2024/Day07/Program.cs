// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day07\Full.txt";
#endif

    List<long[]> equations = 
        File
        .ReadAllLines(fileName)
        .Select(Tools.ParseLongs)
        .ToList();

    Console.WriteLine("Part 1: " + Solve(equations, allowConcat: false));
    Console.WriteLine("Part 2: " + Solve(equations, allowConcat: true));
    Console.ReadLine();
}

long Solve(List<long[]> rows, bool allowConcat)
{
    return rows
        .Where(e => HasValidCombinations(e, allowConcat))
        .Sum(e => e[0]);
}

bool HasValidCombinations(long[] values, bool allowConcat)
{
    long expectedResult = values[0];
    int maxIndex = values.Length - 1;

    // int = index, long = valueSoFar
    PriorityQueue<int, long> todo = new(Comparer<long>.Create(Tools.InverseComparer));
    todo.Enqueue(1, values[1]);

    while(todo.TryDequeue(out int index, out long valueSoFar))
    {
        if (valueSoFar > expectedResult) continue;

        if (index == maxIndex)
        {
            if (valueSoFar == expectedResult)
            {
                return true;
            }
            else continue;
        }

        todo.Enqueue(index + 1, valueSoFar + values[index + 1]);
        todo.Enqueue(index + 1, valueSoFar * values[index + 1]);

        if (allowConcat)
        {
            todo.Enqueue(index + 1, Tools.Concat(valueSoFar, values[index + 1]));
        }
    }

    return false;
}

public static class Tools
{
    public static long[] ParseLongs(string input)
    {
        return
            Regex.Matches(input, @"\d+")
            .Select(m => long.Parse(m.Value))
            .ToArray();
    }

    public static int InverseComparer(long l1, long l2) => l2.CompareTo(l1);

    public static long Concat(long left, long right)
    {
        return long.Parse($"{left}{right}");
    }
}