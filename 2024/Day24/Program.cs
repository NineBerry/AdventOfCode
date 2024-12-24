// #define Sample

using System.Numerics;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day24\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);
    var part1Input = lines.TakeWhile(s => s != "");
    var deviceDecscription = lines.SkipWhile(s => s != "").Skip(1);
    Device device = new(deviceDecscription);


    Console.WriteLine("Part 1: " + Part1(part1Input, device));
    Console.WriteLine("Part 2: " + Part2(device));
    Console.ReadLine();
}

long Part1(IEnumerable<string> input, Device device)
{
    foreach (var line in input)
    {
        var parts = line.Split(": ");
        var wire = parts[0];
        bool value = parts[1] == "1";

        device.SetWireValue(wire, value);
    }
    
    return device.GetIntegerVariable('z');
}

// False: ctt,vhw,z08,z15,z22,z23,z31,z32
// False: ctt,vhw,z13,z29,z31,z32,z36,z38

string Part2(Device device)
{
    return device.FindFaultyGates();
}

// Ideas:

// For Z* find all connected X* and Y* Each Z* must be connected to all X# and Y# where # <= *
// And must not be connected to an X# or Y# with # > *
// If we can find these issues, then we find the candidates to swap on the path to wrong input


// If this is unsuccessful, use out earlier approach with the behaviourcounter, take the 20 nastiests
// nodes according to their behaviour and actually simulate pairwise swapping within that smaller group of 
// candidates.


class Device
{
    private Dictionary<string, List<Gate>> GatesAtInputWires = [];
    private Dictionary<string, Gate> GatesAtOutputWires = [];
    private Dictionary<string, string> Redirections = [];

    private Dictionary<string, bool> Wires = [];

    public Device(IEnumerable<string> description)
    {
        foreach (var gateDescription in description)
        {
            Gate gate = Gate.Factory(this, gateDescription);

            AddGateAtInputWire(gate.Input1Name, gate);
            AddGateAtInputWire(gate.Input2Name, gate);
            GatesAtOutputWires[gate.OutputName] = gate;
        }
    }

    private void AddGateAtInputWire(string wire, Gate gate)
    {
        if(!GatesAtInputWires.TryGetValue(wire, out var list))
        {
            list = [];
            GatesAtInputWires[wire] = list;
        }

        list.Add(gate);
    }

    public bool? GetWireValue(string wire)
    {
        if(Wires.TryGetValue(wire, out var value))
            return value;

        return null;
    }
    public void SetWireValue(string wire, bool value)
    {
        Wires[wire] = value;
        Trigger(wire);
    }

    private void Trigger(string wire)
    {
        if(GatesAtInputWires.TryGetValue(wire, out var list))
        {
            foreach (var gate in list)
            {
                gate.Trigger();
            }
        }
    }

    public long GetIntegerVariable(char prefix)
    {
        long result = 0;
        int counter = 0;

        while (true)
        {
            string name = prefix + counter.ToString("00");

            var value = GetWireValue(name);

            if(!value.HasValue) break;

            if (value.Value)
            {
                result += (1L << counter);
            }

            counter++;
        }

        return result;
    }

    public void SetIntegerVariable(char prefix, long value)
    {

        foreach(var counter in Enumerable.Range(0, 45))
        {
            string name = prefix + counter.ToString("00");

            var bit = (value & (1L << counter)) != 0; 
            SetWireValue(name, bit);
        }
    }


    private void Reset()
    {
        Wires.Clear();
    }

    Dictionary<string, HashSet<string>> PrecidingCache = [];

    public HashSet<string> GetPrecidingOutputs(string wire)
    {
        if(PrecidingCache.TryGetValue(wire, out var cached))
        {
            return cached;
        }
        
        HashSet<string> result = [];

        if (GatesAtOutputWires.TryGetValue(wire, out var gate))
        {
            result.Add(wire);
            result.UnionWith(GetPrecidingOutputs(gate.Input1Name));
            result.UnionWith(GetPrecidingOutputs(gate.Input2Name));
        }


        PrecidingCache.Add(wire, result);
        return result;
    }


    private Dictionary<string, int> BehaviourCounter = [];
    private Dictionary<(string, string), int> PairBehaviourCounter = [];

    public string FindFaultyGates()
    {
        Debug(false);

        var marks = BehaviourCounter.OrderBy(x => x.Value).ToList();


        foreach (var mark in marks)
        {
            Console.WriteLine($"{mark.Key}: {mark.Value}");
        }

        return string.Join(',', marks.Take(16).Select(x => x.Key).Order()); 
    }


    private bool Debug(bool earlyExit)
    {
        bool allWasCorrect = true; 

        foreach (var counter in Enumerable.Range(0, 45))
        {
            long op1 = 1 << counter;
            long op2 = 0;
            long expected = op1;

            allWasCorrect &= DebugAddition(
                op1,
                op2,
                expected);
            allWasCorrect &= DebugAddition(
                op2,
                op1,
                expected);

            var expected2 = op1 * 2;
            allWasCorrect &= DebugAddition(
                op1,
                op1,
                expected2);

            if(earlyExit && !allWasCorrect) return false;
        }

        foreach (var counter in Enumerable.Range(0, 1000))
        {
            long op1 = Random.Shared.NextInt64(35184372088832);
            long op2 = Random.Shared.NextInt64(35184372088832); ;
            long expected = op1 + op2;

            allWasCorrect &= DebugAddition(
                op1,
                op2,
                expected);
            allWasCorrect &= DebugAddition(
                op2,
                op1,
                expected);

            var expected2 = op1 * 2;
            allWasCorrect &= DebugAddition(
                op1,
                op1,
                expected2);

            var expected3 = op2 * 2;
            allWasCorrect &= DebugAddition(
                op2,
                op2,
                expected3);

            if (earlyExit && !allWasCorrect) return false;
        }


        return allWasCorrect;
    }

    public bool DebugAddition(long x, long y, long expected)
    {
        Reset();
        SetIntegerVariable('x', x);
        SetIntegerVariable('y', y);

        long sum = GetIntegerVariable('z');

        bool wasCorrect = true;
        HashSet<string> result = [];

        foreach (var counter in Enumerable.Range(0, 45))
        {

            var actualbit = (sum & (1L << counter)) != 0;
            var expectedbit = (expected & (1L << counter)) != 0;


            var mark = (actualbit == expectedbit) ? 1 : -1;

            if(actualbit != expectedbit) wasCorrect = false;

            string name = 'z' + counter.ToString("00");
            var outputs = GetPrecidingOutputs(name);
            foreach(var output in outputs)
            {
                BehaviourCounter.AddSum(output, mark);
            }

            var list = outputs.Order().ToArray();


        }

        return wasCorrect;
    }

    private void ClearRedirections()
    {
        Redirections.Clear();
    }

    private void AddRedirection(string swap1, string swap2)
    {
        Redirections[swap1] = swap2;
        Redirections[swap2] = swap1;
    }
}

abstract class Gate
{
    protected Device Device;

    public readonly string Input1Name;
    public readonly string Input2Name;
    public readonly string OutputName;

    protected bool? Input1 => Device.GetWireValue(Input1Name);
    protected bool? Input2 => Device.GetWireValue(Input2Name);
    protected bool? Output => Device.GetWireValue(OutputName);

    public Gate(Device device, string input1, string input2, string output)
    {
        Device = device;
        Input1Name = input1;
        Input2Name = input2;
        OutputName = output;
    }

    public void Trigger()
    {
        if(Input1.HasValue && Input2.HasValue && !Output.HasValue)
        {
            var result = PerformCalculation(Input1.Value, Input2.Value);
            Device.SetWireValue(OutputName, result);
        }
    }

    protected abstract bool PerformCalculation(bool in1, bool in2);

    public static Gate Factory(Device device, string input)
    {
        var parts = input.Split(' ');

        var input1 = parts[0];
        var input2 = parts[2];
        var output = parts[4];

        return parts[1] switch
        {
            "AND" => new AndGate(device, input1, input2, output),
            "OR" => new OrGate(device, input1, input2, output),
            "XOR" => new XorGate(device, input1, input2, output),
            _ => throw new NotImplementedException()
        };
    }
}

class AndGate: Gate
{
    public AndGate(Device device, string input1, string input2, string output) : base(device, input1, input2, output){}
    protected override bool PerformCalculation(bool in1, bool in2) => in1 && in2;
}

class OrGate : Gate
{
    public OrGate(Device device, string input1, string input2, string output) : base(device, input1, input2, output) { }
    protected override bool PerformCalculation(bool in1, bool in2) => in1 || in2;
}

class XorGate : Gate
{
    public XorGate(Device device, string input1, string input2, string output) : base(device, input1, input2, output) { }
    protected override bool PerformCalculation(bool in1, bool in2) => in1 ^ in2;
}

public static class Tools
{
    public static void AddSum<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        where TValue : IBinaryInteger<TValue>
    {
        if (dict.TryGetValue(key, out var current))
        {
            dict[key] = current + value;
        }
        else
        {
            dict[key] = value;
        }
    }
}
