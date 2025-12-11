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

    long fromFFTToDAC = network.CountUniquePaths("fft", "dac");
    long fromDACToFFT = network.CountUniquePaths("dac", "fft");

    if (fromDACToFFT != 0 && fromFFTToDAC != 0) throw new Exception("Unexpected loop between fft and dac");

    string firstTarget = fromDACToFFT == 0 ? "fft" : "dac";
    string secondTarget = fromDACToFFT == 0 ? "dac" : "fft";


    long fromStartToFirstTarget = network.CountUniquePaths("svr", firstTarget);
    long fromFirstToSecondTarget = firstTarget == "fft" ? fromFFTToDAC : fromDACToFFT;
    long fromSecondTargetToEnd = network.CountUniquePaths(secondTarget, "out");

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

    // TODO: Since the graphs does not contain loops,
    // we can actually use memoization on Traverse()
    // and return the number of paths found

    public long CountUniquePaths(string start, string end)
    {
        long counter = 0;
        HashSet<string> deadEnds = [];

        Traverse(start, [start]);

        return counter;

        // returns true when at least 1 valid path found
        bool Traverse(string current, List<string> soFar)
        {
            if(current == end)
            {
                counter++;
                return true;
            }

            if (!Edges.ContainsKey(current)) return false;
            if (!Edges[current].Any()) return false;

            bool anyFound = false;
            foreach (var neighbor in Edges[current])
            {
                if (!soFar.Contains(neighbor) && !deadEnds.Contains(neighbor))
                {
                    bool found = Traverse(neighbor, [.. soFar, neighbor]);
                    anyFound = anyFound || found;
                }
            }

            if (!anyFound)
            {
                // There is no way from this node to the end
                // So don't try again
                deadEnds.Add(current);  
            }

            return anyFound;
        }
    }
}