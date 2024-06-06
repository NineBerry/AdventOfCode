// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day11\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    State initialState = State.CreateInitialState(input);
    Console.WriteLine("Part 1: " + Solve(initialState));

    input[0] += " An elerium generator. An elerium-compatible microchip. A dilithium generator. A dilithium-compatible microchip.";
    State initialStatePart2 = State.CreateInitialState(input);

    Console.WriteLine("Part 2: " + Solve(initialStatePart2));
    Console.ReadLine();
}

long Solve(State initialState)
{
    Facility facility = new(initialState);
    return facility.GetSteps();
}

public class Facility
{
    public Facility(State initialState)
    {
        this.initialState = initialState;
    }

    private State initialState;
    private HashSet<string> seenStates = [];

    public long GetSteps()
    {
        PriorityQueue<State, int> todos = new();

        todos.Enqueue(initialState, 0);

        while (todos.TryDequeue(out State? current, out int steps))
        {
            var hashString = current.ToAnonymizedString();
            if (seenStates.Contains(hashString)) continue;
            seenStates.Add(hashString);

            if (current.IsEndState()) return steps;
            if (!current.IsValid()) continue;

            State[] nextStates = current.GetNextStates();
            foreach(var nextState in nextStates)
            {
                if (seenStates.Contains(nextState.ToAnonymizedString())) continue;
                if (!nextState.IsValid()) continue;
                todos.Enqueue(nextState, steps + 1);
            }
        }

        return 0;
    }
}

public record State
{
    public int ElevatorPosition;
    public ComponentPair[] ComponentPairs = [];

    public bool IsEndState()
    {
        if(ElevatorPosition != 4) return false;
        foreach (var pair in ComponentPairs)
        {
            if (pair.MicrochipPosition != 4) return false;
            if (pair.GeneratorPosition != 4) return false;
        }

        return true;    
    }

    public bool IsValid()
    {
        foreach(var checkChip in ComponentPairs)
        {
            // Chip is protected when generator is at the same position
            if (checkChip.GeneratorPosition == checkChip.MicrochipPosition) continue;

            // Look for a generator at the same position as the chip
            foreach (var checkGenerator in ComponentPairs)
            {
                if(checkGenerator.ElementName == checkChip.ElementName) continue;

                if(checkGenerator.GeneratorPosition == checkChip.MicrochipPosition) return false;
            }
        }
        
        return true ;
    }

    public static State CreateInitialState(string[] lines)
    {
        Dictionary<string, ComponentPair> buildArea = new();

        int floor = 0;
        foreach(string line in lines)
        {
            floor++;
            ProcessLine(line, floor);
        }

        State state = new State
        {
            ElevatorPosition = 1,
            ComponentPairs = buildArea.Values.ToArray(),
        }; 
        return state;


        ComponentPair GetPair(string elementName)
        {
            if(!buildArea.TryGetValue(elementName, out var pair))
            {
                pair = new ComponentPair(elementName);
                buildArea[elementName] = pair;
            }

            return pair;
        }

        void ProcessLine(string line, int floor)
        {
            var generators = Regex.Matches(line, "an? ([^ ]*) generator", RegexOptions.IgnoreCase).Select(m => m.Groups[1].Value);
            foreach(var g in generators)
            {
                GetPair(g).GeneratorPosition = floor;
            }

            var chips = Regex.Matches(line, "an? ([^ ]*)-compatible microchip", RegexOptions.IgnoreCase).Select(m => m.Groups[1].Value);
            foreach (var c in chips)
            {
                GetPair(c).MicrochipPosition = floor;
            }
        }
    }

    public State[] GetNextStates()
    {
        List<State> states = new List<State>(); 

        if(ElevatorPosition > 1)
        {
            states.AddRange(GetNextStatesMoving(ElevatorPosition, ElevatorPosition - 1));    
        }

        if (ElevatorPosition < 4)
        {
            states.AddRange(GetNextStatesMoving(ElevatorPosition, ElevatorPosition + 1));
        }

        return states.ToArray();
    }

    private IEnumerable<State> GetNextStatesMoving(int fromPosition, int toPosition)
    {
        return GetNextStatesMovingRecursive(fromPosition, toPosition, [], this.ComponentPairs, 0);
    }

    private IEnumerable<State> GetNextStatesMovingRecursive(int fromPosition, int toPosition, IEnumerable<ComponentPair> soFar, IEnumerable<ComponentPair> remaining, int count)
    {
        if (count > 2) return [];
        if(!remaining.Any() && count == 0) return [];

        if (!remaining.Any())
        {
            return [new State 
            { 
                ElevatorPosition = toPosition,
                ComponentPairs = soFar.ToArray(),
            }];
        }

        List<State> states = new List<State>(); 
        var nextPair = remaining.First();

        if (nextPair.GeneratorPosition == fromPosition && nextPair.MicrochipPosition == fromPosition)
        {
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair with { MicrochipPosition = toPosition, GeneratorPosition = toPosition }], remaining.Skip(1), count + 2));
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair with { MicrochipPosition = toPosition }], remaining.Skip(1), count + 1));
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair with { GeneratorPosition = toPosition }], remaining.Skip(1), count + 1));
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair], remaining.Skip(1), count));
        }
        else if (nextPair.GeneratorPosition == fromPosition)
        {
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair with { GeneratorPosition = toPosition }], remaining.Skip(1), count + 1));
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair], remaining.Skip(1), count));
        }
        else if (nextPair.MicrochipPosition == fromPosition)
        {
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair with { MicrochipPosition = toPosition }], remaining.Skip(1), count + 1));
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [.. soFar, nextPair], remaining.Skip(1), count));
        }
        else
        {
            states.AddRange(GetNextStatesMovingRecursive(fromPosition, toPosition, [..soFar, nextPair], remaining.Skip(1), count));
        }

        return states;
    }

    public string ToAnonymizedString()
    {
        // Idea is that for checking whether we have seen the same state already, we don't need
        // to look at the concrete elements, but just the configuration of positions.
        // Sort by positions to get a state string for the configuration ignoring the concrete elements.
        var sortedPairs = ComponentPairs.OrderBy(p => p.MicrochipPosition).ThenBy(p => p.GeneratorPosition).Select(p => p.ToAnonymizedString());
        return ElevatorPosition.ToString() + string.Join("", sortedPairs);
    }
}

public record class ComponentPair
{
    public ComponentPair(string elementName)
    {
        ElementName = elementName;
    }

    public string ElementName;
    public int MicrochipPosition;
    public int GeneratorPosition;

    public string ToAnonymizedString()
    {
        return $"({MicrochipPosition},{GeneratorPosition})";
    }
}
