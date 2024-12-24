// #define Sample

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
    // Console.WriteLine("Part 2: " + Part2());
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

class Device
{
    private Dictionary<string, List<Gate>> GatesAtWires = [];

    private Dictionary<string, bool> Wires = [];

    public Device(IEnumerable<string> description)
    {
        foreach (var gateDescription in description)
        {
            Gate gate = Gate.Factory(this, gateDescription);

            AddGateAtWire(gate.Input1Name, gate);
            AddGateAtWire(gate.Input2Name, gate);
            AddGateAtWire(gate.OutputName, gate);
        }
    }

    private void AddGateAtWire(string wire, Gate gate)
    {
        if(!GatesAtWires.TryGetValue(wire, out var list))
        {
            list = [];
            GatesAtWires[wire] = list;
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
        foreach(var gate in GatesAtWires[wire])
        {
            gate.Trigger();
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
