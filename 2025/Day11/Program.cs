// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2025\Day11\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2025\Day11\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2025\Day11\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2025\Day11\Full.txt";
#endif

    Network network = new(File.ReadAllLines(fileNamePart1));
    Console.WriteLine("Part 1: " + Part1(network));

    Network network2 = new(File.ReadAllLines(fileNamePart2));
    Console.WriteLine("Part 2: " + Part2(network2));

    Console.ReadLine();
}

long Part1(Network network)
{
    return network.CountUniquePaths("you", "out");
}


long Part2(Network network)
{
    // We noticed that one intermediate target always appears before the other
    // -> Only need to look at that order and partial paths from start -> first -> second -> out

    const string NODE_DAC = "dac";
    const string NODE_FFT = "fft";
    const string NODE_SVR = "svr";
    const string NODE_OUT = "out";

    long fromFFTToDAC = network.CountUniquePaths(NODE_FFT, NODE_DAC);
    long fromDACToFFT = network.CountUniquePaths(NODE_DAC, NODE_FFT);

    if (fromDACToFFT != 0 && fromFFTToDAC != 0) throw new Exception("Unexpected loop between fft and dac");

    string firstTarget = fromDACToFFT == 0 ? NODE_FFT : NODE_DAC;
    string secondTarget = fromDACToFFT == 0 ? NODE_DAC : NODE_FFT;

    long fromStartToFirstTarget = network.CountUniquePaths(NODE_SVR, firstTarget);
    long fromFirstToSecondTarget = fromFFTToDAC + fromDACToFFT;
    long fromSecondTargetToEnd = network.CountUniquePaths(secondTarget, NODE_OUT);

    return fromStartToFirstTarget * fromFirstToSecondTarget * fromSecondTargetToEnd;
}

class Network
{
    private Dictionary<string, HashSet<string>> Edges = [];

    public Network(string[] lines)
    {
        foreach(var line in lines)
        {
            AddEdges(line);
        }
    }

    private void AddEdges(string line)
    {
        string[] nodes = Regex.Matches(line, @"[a-z]{3}").Select(s => s.Value).ToArray();
        Edges.Add(nodes[0], nodes.Skip(1).ToHashSet());
    }

    public long CountUniquePaths(string start, string end)
    {
        Dictionary<string, long> cache = [];
        return Traverse(start);

        long Traverse(string current)
        {
            if (cache.TryGetValue(current, out var result)) return result;

            if (current == end) return 1;

            if (Edges.TryGetValue(current, out var neighbors))
            {
                result = neighbors.Sum(Traverse);
            }

            cache.Add(current, result);
            return result;
        }
    }
}