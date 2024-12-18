// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day16\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day16\Full.txt";
#endif

    var network = new Network(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 2: " + Part2(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    return network.GetMaxFlow(30, network.RelevantValves.ToHashSet());
}
long Part2(Network network)
{
    return network.GetMaxFlowPair();
}

public class Valve
{
    public string Name;
    public int FlowValue;
    public string[] ConnectedValves = [];

    public Valve(string input)
    {
        string[] names = Regex.Matches(input, "[A-Z]{2}").Select(m => m.Value).ToArray();
        Name = names[0];
        ConnectedValves = names.Skip(1).ToArray();
        FlowValue = int.Parse(Regex.Matches(input, "\\d+").Single().Value);
    }
}

public class Network
{
    public Dictionary<string, Valve> Valves = [];
    public string[] RelevantValves = [];
    public Dictionary<(string, string), string[]> Distances = [];

    public Network(string[] lines)
    {
        Valves = lines.Select(s => new Valve(s)).ToDictionary(v => v.Name, v => v);
        RelevantValves = Valves.Values.Where(v => v.FlowValue > 0).Select(v => v.Name).ToArray();

        foreach(var v1 in RelevantValves.Union(["AA"]))
        {
            foreach (var v2 in RelevantValves.Union(["AA"]))
            {
                if (v1 == v2) continue;
                Distances[(v1, v2)] = GetDistance(v1, v2);
            }
        }
    }

    public int GetMaxFlow(int totalMinutes, HashSet<string> availableValves)
    {
        int max = 0;

        Recurse(0, 0, 0, "AA", availableValves);

        void Recurse(int minutesPassed, int flowPerMinute, int sumFlow, string currentValve, HashSet<string> remainingValves)
        {
            if (minutesPassed > totalMinutes) return;

            ReportFlow(minutesPassed, sumFlow, flowPerMinute);

            foreach (var nextValve in remainingValves)
            {
                int minutesToWalk = Distances[(currentValve, nextValve)].Length - 1;
                int minutesToOpen = 1;
                int flowWhileWalking = minutesToWalk * flowPerMinute;
                int flowWhileOpening = flowPerMinute;

                int newFlowPerMinute = flowPerMinute + Valves[nextValve].FlowValue;

                Recurse(
                    minutesPassed + minutesToWalk + minutesToOpen,
                    newFlowPerMinute,
                    sumFlow + flowWhileWalking + flowWhileOpening,
                    nextValve,
                    remainingValves.Except([nextValve]).ToHashSet());
            }
        }

        void ReportFlow(int minutesPassed, int sumFlow, int flowPerMinute)
        {
            int minutesLeft = totalMinutes - minutesPassed;
            sumFlow += minutesLeft * flowPerMinute;
            max = Math.Max(max, sumFlow);
        }

        return max;
    }

    // Ideas for possible improvements:
    // * Cull tree by estimating best expected result and comparing with current max
    // * Chose valves in order as found in Part1
    public int GetMaxFlowPair()
    {
        int totalMinutes = 26;
        int max = 0;

        Recurse(0, 0, 0, "AA", 0, "AA", 0, RelevantValves.ToHashSet());

        void Recurse(int minutesPassed, int flowPerMinute, int sumFlow, 
            string p1Target, int p1StepsLeft,
            string p2Target, int p2StepsLeft,
            HashSet<string> remainingValves)
        {
            ReportFlow(minutesPassed, sumFlow, flowPerMinute);

            if (minutesPassed >= totalMinutes) return;

            if(p1StepsLeft < 0 && p2StepsLeft < 0) return;

            int nextFlowPerMinute = flowPerMinute;

            bool newPlayer1 = false;
            bool newPlayer2 = false;

            if (p1StepsLeft == 0)
            {
                newPlayer1 = true;
                nextFlowPerMinute = nextFlowPerMinute + Valves[p1Target].FlowValue;
            }

            if (p2StepsLeft == 0)
            {
                newPlayer2 = true;
                nextFlowPerMinute = nextFlowPerMinute + Valves[p2Target].FlowValue;
            }

            int nextSumFlow = sumFlow + nextFlowPerMinute;

            if (!remainingValves.Any())
            {
                Recurse(
                    minutesPassed + 1,
                    nextFlowPerMinute,
                    nextSumFlow,
                    p1Target,
                    p1StepsLeft - 1,
                    p2Target,
                    p2StepsLeft - 1,
                    remainingValves);
            }

            if (newPlayer1 && newPlayer2)
            {
                if (p1Target == p2Target)
                {
                    var remainingList = remainingValves.ToList();
                    
                    for (int i = 0; i < remainingList.Count; i++)
                    {
                        for (int j = i + 1; j < remainingList.Count; j++)
                        {
                            string nextValveP1 = remainingList[i];
                            string nextValveP2 = remainingList[j];
                            int timeToTargetP1 = Distances[(p1Target, nextValveP1)].Length - 1;
                            int timeToTargetP2 = Distances[(p2Target, nextValveP2)].Length - 1;

                            Recurse(
                                minutesPassed + 1,
                                nextFlowPerMinute,
                                nextSumFlow,
                                nextValveP1,
                                timeToTargetP1,
                                nextValveP2,
                                timeToTargetP2,
                                remainingValves.Except([nextValveP1, nextValveP2]).ToHashSet());
                        }
                    }
                }
                else
                {
                    foreach (var nextValveP1 in remainingValves)
                    {
                        foreach (var nextValveP2 in remainingValves)
                        {
                            if (nextValveP1 == nextValveP2) continue;

                            int timeToTargetP1 = Distances[(p1Target, nextValveP1)].Length - 1;
                            int timeToTargetP2 = Distances[(p2Target, nextValveP2)].Length - 1;

                            Recurse(
                                minutesPassed + 1,
                                nextFlowPerMinute,
                                nextSumFlow,
                                nextValveP1,
                                timeToTargetP1,
                                nextValveP2,
                                timeToTargetP2,
                                remainingValves.Except([nextValveP1, nextValveP2]).ToHashSet());
                        }
                    }

                }

                foreach (var nextValveP1 in remainingValves)
                {
                    foreach (var nextValveP2 in remainingValves)
                    {
                        if (nextValveP1 == nextValveP2) continue;

                        int timeToTargetP1 = Distances[(p1Target, nextValveP1)].Length - 1;
                        int timeToTargetP2 = Distances[(p2Target, nextValveP2)].Length - 1;

                        Recurse(
                            minutesPassed + 1,
                            nextFlowPerMinute,
                            nextSumFlow,
                            nextValveP1,
                            timeToTargetP1,
                            nextValveP2,
                            timeToTargetP2,
                            remainingValves.Except([nextValveP1, nextValveP2]).ToHashSet());
                    }
                }
            }
            else if (newPlayer1)
            {
                foreach (var nextValve in remainingValves)
                {
                    int timeToTargetP1 = Distances[(p1Target, nextValve)].Length - 1;
                    Recurse(
                        minutesPassed + 1,
                        nextFlowPerMinute,
                        nextSumFlow,
                        nextValve,
                        timeToTargetP1,
                        p2Target,
                        p2StepsLeft - 1,
                        remainingValves.Except([nextValve]).ToHashSet());
                }
            }
            else if (newPlayer2)
            {
                foreach (var nextValve in remainingValves)
                {
                    int timeToTargetP2 = Distances[(p2Target, nextValve)].Length - 1;
                    Recurse(
                        minutesPassed + 1,
                        nextFlowPerMinute,
                        nextSumFlow,
                        p1Target,
                        p1StepsLeft - 1,
                        nextValve,
                        timeToTargetP2,
                        remainingValves.Except([nextValve]).ToHashSet());
                }
            }
            else
            {
                Recurse(
                    minutesPassed + 1,
                    nextFlowPerMinute,
                    nextSumFlow,
                    p1Target,
                    p1StepsLeft - 1,
                    p2Target,
                    p2StepsLeft - 1,
                    remainingValves);
            }
        }

        void ReportFlow(int minutesPassed, int sumFlow, int flowPerMinute)
        {
            int minutesLeft = totalMinutes - minutesPassed;
            sumFlow += minutesLeft * flowPerMinute;
            if (sumFlow > max)
            {
                max = Math.Max(max, sumFlow);
                Console.WriteLine(max);
            }
        }

        return max;
    }

    private string[] GetDistance(string source, string target)
    {
        HashSet<string> visited = new HashSet<string>();
        PriorityQueue<string[], int> todos = new PriorityQueue<string[], int>();

        todos.Enqueue([source], 0);

        while(todos.TryDequeue(out var path, out var currentDistance))
        {
            var current = path.Last();
            if (visited.Contains(current)) continue;
            visited.Add(current);

            if(current == target) return path;

            Valve valve = Valves[current];
            foreach(var connected in valve.ConnectedValves)
            {
                todos.Enqueue([..path, connected],  currentDistance + 1);
            }
        }
        
        return [];
    }
}