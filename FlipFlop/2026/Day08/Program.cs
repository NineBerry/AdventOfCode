// #define Sample

using System.Numerics;
using System.Text;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day08\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.WriteLine("Part 3: " + Part3(lines));

    Console.ReadLine();
}
long Part1(string[] lines)
{
    SingleRule[] rules = lines.Select(SingleRule.Parse).ToArray();

    Dictionary<char, long> stoats = new Dictionary<char, long>();
    stoats['A'] = 1;
    stoats['B'] = 1;

    foreach (var _ in Enumerable.Range(1, 7))
    {
        var old = stoats;
        stoats = [];

        var uniqueLetters = old.Keys.Distinct().ToArray();

        foreach (var letter in uniqueLetters)
        {
            var rule = rules.First(r => r.From == letter);
            foreach (var to in rule.To)
            {
                if (!stoats.ContainsKey(to))
                {
                    stoats[to] = 0;
                }
                stoats[to] += old[letter];
            }
        }
    }

    return stoats.Values.Sum();
}

long Part2(string[] lines)
{
    PairRule[] rules = lines.Select(PairRule.Parse).ToArray();

    // Naive solution actually creating string.
    // I already expected part 3 to use more generations and could have implemented 
    // optimized solution immediately but thought it would be fun to use naive approach first.
    StringBuilder stoats = new("AB");

    foreach (var _ in Enumerable.Range(1, 7))
    {
        var old = stoats;
        stoats = new();

        foreach (var i in Enumerable.Range(0, old.Length - 1))
        {
            var children = PairRule.Pair(rules, old[i], old[i + 1]);
            
            if(i == 0) stoats.Append(old[i]); 

            stoats.Append(children);
            stoats.Append(old[i+1]);
        }
    }

    return stoats.Length;
}

BigInteger Part3(string[] lines)
{
    PairRule[] rules = lines.Select(PairRule.Parse).ToArray();
    Dictionary<(char, char, int), BigInteger> cache = [];
    
    return RecursiveCount('A', 'B', 21);

    BigInteger RecursiveCount(char parent1, char parent2, int generationsToGo)
    {
        if (generationsToGo == 0) return 2;

        if(cache.TryGetValue((parent1, parent2, generationsToGo), out BigInteger cached)) return cached;

        char[] nextGeneration = [parent1, ..PairRule.Pair(rules, parent1, parent2), parent2];

        BigInteger result = 0; 
        
        foreach (var i in Enumerable.Range(0, nextGeneration.Length - 1))
        {
            // Initial pair does not overlap so count left parent
            // All other pairs overlap so we subtract 1 to avoid double counting
            if (i != 0) result += -1;
            result +=  RecursiveCount(nextGeneration[i], nextGeneration[i + 1], generationsToGo - 1);
        }

        cache[(parent1, parent2, generationsToGo)] = result;
        return result;
    }
}

class SingleRule()
{
    public static SingleRule Parse(string line)
    {
        var letters = line.Where(c => c != ' ');
        return new SingleRule
        {
            From = letters.First(),
            To = [.. letters.Skip(1)]
        };
    }   

    public char From { get; init; }
    public char[] To { get; init; } = [];
}

class PairRule()
{
    public static PairRule Parse(string line)
    {
        var letters = line.Where(c => c != ' ').ToArray();
        return new PairRule
        {
            Parent1 = letters[0],
            Parent2 = letters[1],
            To = [.. letters.Skip(2)]
        };
    }

    public static char[] Pair(PairRule[] rules, char parent1, char parent2)
    {
        var rule = rules.FirstOrDefault(r => r.Parent1 == parent1 && r.Parent2 == parent2 || r.Parent1 == parent2 && r.Parent2 == parent1);
        return rule?.To ?? [];
    }

    public char Parent1 { get; init; }
    public char Parent2 { get; init; }
    public char[] To { get; init; } = [];
}
