﻿// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day16\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day16\Full.txt";
#endif

    var network = new Network(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    return network.GetMaxFlow();
}

// 1671 too low
// 1719 too low
// 1998 too low

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
    public Dictionary<(string, string), int> Distances = [];

    public Network(string[] lines)
    {
        Valves = lines.Select(s => new Valve(s)).ToDictionary(v => v.Name, v => v);
        RelevantValves = Valves.Values.Where(v => v.FlowValue > 0 || v.Name == "AA").Select(v => v.Name).ToArray();

        foreach(var v1 in RelevantValves)
        {
            foreach(var v2 in RelevantValves)
            {
                if (v1 == v2) continue;
                Distances[(v1, v2)] = GetDistance(v1, v2);
            }
        }
    }

    public int GetMaxFlow()
    {
        return GetCombinations();



        // TODO: Idea actual output and combination finding together.
        // Move the check whether the path is potentially higher than the furtest so far
        // from output calculcation to combination finding


        /* List<Valve>[] combinations = GetCombinations();

        int maxOutput = 0;

        int combCount = combinations.Length;
        int counter = 0;
        int percentValue = combCount / 100;
        foreach(var comb in combinations)
        {
            counter++;
            if(counter  % percentValue == 0)
            {
                Console.Write("\r" + (counter / percentValue) + "%");
            }


            maxOutput = Math.Max(maxOutput, GetOutput(comb));
        }

        // List<Valve> comb = [Valves["AA"], Valves["DD"], Valves["BB"], Valves["JJ"], Valves["HH"], Valves["EE"], Valves["CC"]];
        // maxOutput = Math.Max(maxOutput, GetOutput(comb));

        return maxOutput;*/
    }

    private int max = 0;

    private int GetOutput(List<Valve> combination)
    {
        int output = 0;
        int minute = 1;
        int currentFlow = 0;
        
        Queue<Valve> remaining = new(combination.Skip(1));
        Valve current = combination.First();
        Valve next = remaining.Dequeue();
        int minutesToNext = Distances[(current.Name, next.Name)];

        while (minute <= 30)
        {
            // Console.WriteLine($"Minute {minute} Releasing {currentFlow}");            
            output += currentFlow;

            int minutesRemaining = 30 - minute;
            int maxPotential =  output + minutesRemaining * (currentFlow + remaining.Sum(r => r.FlowValue));
            if (maxPotential < max)
            {
                return 0;
            }

            if (minutesToNext > 0)
            {
                minutesToNext--;
            }
            else
            {
                // Console.WriteLine($"Opening valve {next.Name} at minute {minute}");

                currentFlow += next.FlowValue;
                current = next;
                
                if(remaining.Count == 0)
                {
                    next = null;
                    minutesToNext = int.MaxValue;
                }
                else
                {
                    next = remaining.Dequeue();
                    minutesToNext = Distances[(current.Name, next.Name)];
                }
            }

            minute++;    
        }

        if(output > max)
        {
            max = output;
            Console.WriteLine(max);
        }

        return output;
    }

    private int GetCombinations()
    {
        Valve start = Valves["AA"];
        Valve[] remaining = RelevantValves.Where(s => s != "Aa").Select(v => Valves[v]).ToArray();
        return GetCombinationsRecursive([start], remaining.ToHashSet());
    }

    private int GetCombinationsRecursive(List<Valve> soFar, HashSet<Valve> remaining)
    {
        if(!remaining.Any()) return GetOutput(soFar);

        int result = 0;

        foreach(var next in remaining.OrderByDescending(s => s.FlowValue).Take(5))
        {
            result = Math.Max(result, GetCombinationsRecursive([..soFar, next], remaining.Except([next]).ToHashSet()));
        }

        return result;
    }

    private int GetDistance(string source, string target)
    {
        HashSet<string> visited = new HashSet<string>();
        PriorityQueue<string, int> todos = new PriorityQueue<string, int>();

        todos.Enqueue(source, 0);

        while(todos.TryDequeue(out var current, out var currentDistance))
        {
            if (visited.Contains(current)) continue;
            visited.Add(current);

            if(current == target) return currentDistance;

            Valve valve = Valves[current];
            foreach(var connected in valve.ConnectedValves)
            {
                todos.Enqueue(connected,  currentDistance + 1);
            }
        }
        
        return 0;
    }
}