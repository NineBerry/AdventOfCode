// #define Sample


{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day25\Full.txt";
#endif

    var items = File.ReadAllText(fileName).ReplaceLineEndings(Environment.NewLine).Split(Environment.NewLine + Environment.NewLine);
    (var locks, var keys) = IdentifyItems(items);

    Console.WriteLine("Part 1: " + Part1(locks, keys));
    Console.ReadLine();
}


long Part1(int[][] locks, int[][] keys)
{
    int counter = 0;
    
    foreach(var aLock in locks)
    {
        foreach(var key in keys)
        {
            if (Enumerable.Range(0, key.Length).All(column => aLock[column] + key[column] <= 7))
            {
                counter++;
            }
        }
    }

    return counter;
}

(int[][] Locks, int[][] Keys) IdentifyItems(string[] items)
{
    List<int[]> locks = [];
    List<int[]> keys = [];

    foreach (var item in items)
    {
        string[] lines = item.Split(Environment.NewLine);
        
        int[] lengths = Enumerable.Range(0, lines[0].Length).Select(column => lines.Count(line => line[column] == '#')).ToArray();

        if (lines.First().All(character => character == '#'))
        {
            locks.Add(lengths);
        }
        else
        {
            keys.Add(lengths);
        }
    }

    return ([..locks], [..keys]);
}
