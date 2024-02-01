// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day15\Full.txt";
#endif

    Disc[] discs = File.ReadAllLines(fileName).Select(l => new Disc(l)).ToArray();

    Console.WriteLine("Part 1: " + Part1(discs));
    Console.WriteLine("Part 2: " + Part2(discs));
    Console.ReadLine();
}

long Part1(Disc[] discs)
{
    return Solve(discs);
}

long Part2(Disc[] discs)
{
    var additional = new Disc($"Disc #{discs.Length + 1} has 11 positions; at time=0, it is at position 0.");
    Disc[] enhancedDiscs = [.. discs, additional];
    return Solve(enhancedDiscs);
}

long Solve(Disc[] discs)
{
    int waitTime = 0;

    while (!discs.All(d => d.IsAtZero(waitTime)))
    {
        waitTime++;
    }

    return waitTime;
}

public record Disc
{
    public Disc(string line)
    {
        int[] values = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();

        ID = values[0];
        Positions = values[1];
        StartPosition = values[3];
    }

    public int ID;
    public int Positions;
    public int StartPosition;

    public bool IsAtZero(int startTime)
    {
        int position = (startTime + ID + StartPosition) % Positions;
        return position == 0;

    }
}