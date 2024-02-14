// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day14\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day14\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    string startWord = input[0];
    var expansionRules = input.Skip(2).ToDictionary(s => s.Substring(0, 2), s => s[6]);

    Console.WriteLine("Part 1: " + Solve(startWord, expansionRules, 10));
    Console.WriteLine("Part 2: " + Solve(startWord, expansionRules, 40));
    Console.ReadLine();
}

long Solve(string startWord, Dictionary<string, char> expansionRules, int steps)
{
    var statistics = MakeStatisticsForStartWord(startWord);

    foreach(var _ in Enumerable.Range(1, steps))
    {
        statistics = PerformStep(statistics, expansionRules);
    }

    var occurences = CountOccurence(statistics);

    var ordered = occurences.OrderBy(p => p.Value);
    var leastCommon = ordered.First();
    var mostCommon = ordered.Last();

    return mostCommon.Value - leastCommon.Value;
}

Dictionary<string, long> MakeStatisticsForStartWord(string startWord)
{
    Dictionary<string, long> result = [];

    for(int i=0; i < startWord.Length - 1; i++)
    {
        string pair = startWord.Substring(i, 2);
        result.Increase(pair, 1);
    }

    result.Increase(startWord.Last() + "_", 1);

    return result;
}


Dictionary<string, long> PerformStep(Dictionary<string, long> before, Dictionary<string, char> expansionRules)
{
    Dictionary<string, long> result = [];

    foreach(var pair in before)
    {
        if(expansionRules.TryGetValue(pair.Key, out var toInsert))
        {
            result.Increase("" + pair.Key[0] + toInsert, pair.Value);
            result.Increase("" + toInsert + pair.Key[1], pair.Value);

        }
        else
        {
            result.Increase(pair.Key, pair.Value);
        }
    }

    return result; 
}

Dictionary<char, long> CountOccurence(Dictionary<string, long> statistics)
{
    Dictionary<char, long> result = [];

    var distinctChars = statistics.Keys.Select(s => s[0]).Distinct();
    foreach(var ch in distinctChars) 
    {
        result[ch] = statistics.Where(s => s.Key[0] == ch).Sum(s => s.Value);
    }

    return result; 
}

public static class Extensions
{
    public static void Increase(this Dictionary<string, long> dictionary, string key, long value)
    {
        dictionary.TryGetValue(key, out var currentValue);
        dictionary[key] = currentValue + value;
    }
}