// #define Sample

using System.Text;
using System.Text.RegularExpressions;
using Google.OrTools.LinearSolver;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day10\Full.txt";
#endif

    Machine[] machines = 
        File.ReadAllLines(fileName)
        .Select(s => new Machine(s))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(machines));
    Console.WriteLine("Part 2: " + Part2(machines));
    Console.ReadLine();
}

long Part1(Machine[] machines)
{
    return machines.Sum(m => m.CountMinButtonPressesToInitLights()); ;
}

long Part2(Machine[] machines)
{
    return machines.Sum(m => m.CalculateMinButtonPressesToReachVoltage()); ;
}

class Machine
{
    private string TargetState;
    private int[] TargetVoltage;
    private int[][] Buttons;

    public Machine(string input)
    {
        TargetState = Regex.Match(input, @"\[(.*)\]").Groups[1].Value;

        var buttonStrings = Regex.Matches(input, @"\([\d,]+\)").Select(m => m.Value);
        Buttons = buttonStrings.Select(Tools.ParseInts).ToArray();

        var voltageString = Regex.Match(input, @"\{(.*)\}").Groups[1].Value;
        TargetVoltage = Tools.ParseInts(voltageString);
    }

    public long CountMinButtonPressesToInitLights()
    {
        HashSet<string> seen = [];

        PriorityQueue<string, int> todo = new();
        todo.Enqueue(CreateInitialState(), 0);

        while (todo.TryDequeue(out var currentState, out int currentPresses))
        {
            if (seen.Contains(currentState)) continue;
            seen.Add(currentState);

            if (currentState == TargetState) return currentPresses;

            foreach (var button in Buttons)
            {
                string nextState = PressButton(currentState, button);
                todo.Enqueue(nextState, currentPresses + 1);
            }
        }

        throw new Exception("Not possible");
    }

    private string PressButton(string previousState, int[] button)
    {
        StringBuilder result = new StringBuilder(previousState);

        foreach (var position in button)
        {
            result[position] = ToggleLight(result[position]);
        }

        return result.ToString();
    }

    private char ToggleLight(char from)
    {
        return from == '.' ? '#' : '.';
    }

    private string CreateInitialState()
    {
        return TargetState.Replace('#', '.');
    }

    public long CalculateMinButtonPressesToReachVoltage()
    {
        Solver solver = Solver.CreateSolver("SCIP");

        // Variables
        int variableCount = Buttons.Length;
        Variable[] variables = new Variable[variableCount];
        foreach (var i in Enumerable.Range(0, variableCount))
        {
            variables[i] = solver.MakeIntVar(0, int.MaxValue, "B" + i);
        }

        // Equations
        int equationsCount = TargetVoltage.Length;
        foreach (var position in Enumerable.Range(0, equationsCount))
        {
            var constraint = solver.MakeConstraint(TargetVoltage[position], TargetVoltage[position]);

            foreach (var variable in Enumerable.Range(0, variableCount))
            {
                if (Buttons[variable].Contains(position))
                {
                    constraint.SetCoefficient(variables[variable], 1);
                }
            }
        }

        // Add objective to minimize sum
        Objective objective = solver.Objective();
        objective.SetMinimization();

        foreach (var i in Enumerable.Range(0, variableCount))
        {
            objective.SetCoefficient(variables[i], 1);
        }

        solver.Solve();
        var total = variables.Sum(v => (long)v.SolutionValue());

        return total;
    }
}

public static class Tools
{
    public static int[] ParseInts(string input)
    {
        return
            Regex.Matches(input, @"\d+")
            .Select(m => int.Parse(m.Value))
            .ToArray();
    }
}