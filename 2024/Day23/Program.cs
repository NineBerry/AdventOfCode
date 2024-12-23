// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day23\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day23\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Network network = Network.BuildNetwork(input);

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 2: " + Part2(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    var cliques = network.FindCliques(3);
    return cliques.Count(c => c.Any(s => s[0] == 't'));
}

string Part2(Network network)
{
    return network.MakeNetworkString(network.FindBiggestClique());
}

public class Network : Dictionary<string, HashSet<string>>
{
    public static Network BuildNetwork(string[] input)
    {
        Network network = new Network();

        foreach (string line in input)
        {
            var components = Regex.Matches(line, "[a-z]{2}").Select(m => m.Value).ToArray();
            network.AddEdge(components[0], components[1]);
        }

        return network;
    }

    public void AddEdge(string first, string second)
    {
        Add(first, second);
        Add(second, first);

        void Add(string s1, string s2)
        {
            if (!TryGetValue(s1, out var connections))
            {
                connections = new();
                this[s1] = connections;
            }

            connections.Add(s2);
        }
    }

    internal string[] FindBiggestClique()
    {
        HashSet<string> alreadyHandled = [];
        string[] maxClique = [];

        var size3Cliques = FindCliques(3);

        foreach (var clique in size3Cliques)
        {
            TryGrow(clique);
        }

        void TryGrow(string[] clique)
        {
            var candidateNodes = clique.SelectMany(c => this[c]).Except(clique);

            foreach(var candidateNode in candidateNodes)
            {
                string[] toCheck = [.. clique, candidateNode];
                string asString = MakeNetworkString(toCheck);
                if (!alreadyHandled.Contains(asString))
                {
                    alreadyHandled.Add(asString);

                    bool isClique = CheckClique(toCheck);

                    if (isClique)
                    {
                        if(toCheck.Length > maxClique.Length)
                        {
                            maxClique = toCheck;
                        }

                        TryGrow(toCheck);
                    }
                }
            }
        }

        return maxClique;
    }

    public IEnumerable<string[]> FindCliques(int size)
    {
        List<string[]> result = [];
        HashSet<string> allNodes = this.Keys.ToHashSet();
        HashSet<string> alreadyFound = [];

        foreach(var node in allNodes)
        {
            var cliques = FindCliquesForNode(node, size);

            foreach(var clique in cliques)
            {
                string asString = MakeNetworkString(clique);
                if (!alreadyFound.Contains(asString))
                {
                    alreadyFound.Add(asString);
                    result.Add(clique);
                }
            }
        }

        return result;
    }


    private List<string[]> FindCliquesForNode(string node, int size)
    {
        List <string[]> result = [];
        HashSet<string> found = [];

        foreach(var connected in this[node])
        {
            FindRecursive([node, connected]);
        }

        void FindRecursive(string[] alreadyIncludedNodes)
        {
            if(alreadyIncludedNodes.Length == size)
            {
                if (CheckClique(alreadyIncludedNodes)) 
                {
                    string asString = MakeNetworkString(alreadyIncludedNodes);

                    if (!found.Contains(asString))
                    {
                        found.Add(asString);
                        result.Add([.. alreadyIncludedNodes]);
                    }

                }
            }
            else
            {
                string last = alreadyIncludedNodes.Last();
                foreach (var connected in this[last])
                {
                    if (!alreadyIncludedNodes.Contains(connected))
                    {
                        FindRecursive([..alreadyIncludedNodes, connected]);
                    }
                }
            }
        }
        return result;
    }


    bool CheckClique(string[] candidate)
    {
        for (int i = 0; i < candidate.Length; i++)
        {
            for (int j = i + 1; j < candidate.Length; j++)
            {
                string first = candidate[i];
                string second = candidate[j];
                if (!this[first].Contains(second)) return false;
            }
        }

        return true;
    }

    public string MakeNetworkString(IEnumerable<string> nodes)
    {
        return string.Join(',', nodes.Order());
    }
}