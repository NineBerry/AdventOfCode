// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day06\Full.txt";
#endif

    MemoryBank memoryBank = new MemoryBank(File.ReadAllText(fileName));
    Console.WriteLine("Part 1: " + Part1(memoryBank));
    Console.WriteLine("Part 2: " + Part2(memoryBank));
    Console.ReadLine();
}

long Part1(MemoryBank memoryBank)
{
    return CyclesUntilSeen(memoryBank);
}

long Part2(MemoryBank memoryBank)
{
    return CyclesUntilSeen(memoryBank);
}

long CyclesUntilSeen(MemoryBank memoryBank)
{
    int cycles = 0;
    HashSet<string> seen = new HashSet<string>();

    while (!seen.Contains(memoryBank.ToString()))
    {
        seen.Add(memoryBank.ToString());
        memoryBank.Distribute();
        cycles++;
    }

    return cycles;
}

public class MemoryBank
{
    public int[] banks = [];

    public MemoryBank(string input)
    {
        banks = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    }

    public override string ToString()
    {
        return string.Join(",", banks);
    }

    internal void Distribute()
    {
        int maxValue = banks.Max();
        int index = Array.IndexOf(banks, maxValue);

        int valueToDistribute = banks[index];
        banks[index] = 0;

        while(valueToDistribute > 0)
        {
            index++;
            if (index >= banks.Length) index = 0;

            banks[index]++;
            valueToDistribute--;
        }
    }
}
