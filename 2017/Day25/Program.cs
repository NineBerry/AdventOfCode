// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day25\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    TuringMachine machine = new TuringMachine(lines);
    machine.Run();  
    return machine.GetCheckSum();
}

public class TuringMachine
{
    Dictionary<int, bool> strip = [];
    Dictionary<char, TuringMachineState> states = [];

    private char StartState = '0';
    private char CurrentState = '0';
    private int StepsToPerform = 0;
    private int Cursor = 0;

    public TuringMachine(string[] lines)
    {
        StartState = lines[0][^2];
        StepsToPerform = Regex.Matches(lines[1], "\\d+").Select(m => int.Parse(m.Value)).Single();

        int stateCounter = 0;
        while (true)
        {
            string[] sublines = lines.Skip(3 + stateCounter * 10).Take(10).ToArray();
            if(sublines.Length == 0) { break; }
            
            TuringMachineState state = new TuringMachineState(sublines);
            states.Add(state.StateName, state);

            stateCounter++;
        }
    }

    public void Run()
    {
        CurrentState = StartState;

        for(int i=1; i <= StepsToPerform; i++)
        {
            CurrentState = PerformStep(CurrentState);
        }
    }

    private char PerformStep(char currentState)
    {
        TuringMachineState state = states[currentState];
        return PerformAction(Read() ? state.TrueAction : state.FalseAction);
    }

    private char PerformAction(TuringMachineAction turingMachineAction)
    {
        Write(turingMachineAction.WriteValue);
        Cursor += turingMachineAction.Move;
        return turingMachineAction.NextState;
    }

    private bool Read()
    {
        strip.TryGetValue(Cursor, out bool value);
        return value;
    }

    private void Write(bool value)
    {
        strip[Cursor] = value;
    }

    public int GetCheckSum()
    {
        return strip.Count(p => p.Value);
    }
}

public class TuringMachineState
{
    public TuringMachineState(string[] lines)
    {
        StateName = lines[0][^2];
        FalseAction = new TuringMachineAction(lines.Skip(2).Take(3).ToArray());
        TrueAction = new TuringMachineAction(lines.Skip(6).Take(3).ToArray());
    }

    public char StateName;
    public TuringMachineAction FalseAction;
    public TuringMachineAction TrueAction;
}

public class TuringMachineAction
{
    public TuringMachineAction(string[] lines)
    {
        WriteValue = lines[0][^2] == '1';
        Move = lines[1].Contains("right") ? 1 : -1;
        NextState = lines[2][^2];
    }

    public bool WriteValue;
    public int Move;
    public char NextState;
}