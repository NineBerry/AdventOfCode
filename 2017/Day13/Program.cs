// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day13\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day13\Full.txt";
#endif

    var scannerLayers = File.ReadAllLines(fileName).Select(s => new ScannerLayer(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(scannerLayers));
    Console.WriteLine("Part 2: " + Part2(scannerLayers));
    Console.ReadLine();
}


long Part1(ScannerLayer[] layers)
{
    return layers.Where(l => l.Caught(0)).Sum(l => l.Severity);
}

long Part2(ScannerLayer[] layers)
{
    int waitTime = 0;
    
    while(layers.Where(l => l.Caught(waitTime)).Any())
    {
        waitTime++;
    }
    return waitTime;
}

record ScannerLayer
{
    public ScannerLayer(string input)
    {
        int[] values = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        Depth = values[0];
        Range = values[1];
        CycleLength = (Range * 2) - 2;
    }

    public int Depth;
    public int Range;

    public int CycleLength;
    public int Severity => Depth * Range;

    public bool Caught(int StartTime)
    {
        return ((StartTime + Depth) % CycleLength) == 0;
    }
}