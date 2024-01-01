{
    // string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day20\Sample.txt";
    // string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day20\Sample2.txt";
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day20\Full.txt";

    string[] input = File.ReadAllLines(fileName);

    long part1 = Part1(Module.BuildModules(input));
    Console.WriteLine($"Part 1: " + part1);

    // Rebuild modules so they have initial state
    long part2 = Part2(Module.BuildModules(input));
    Console.WriteLine($"Part 2: " + part2);

    Console.ReadLine();
}

long Part1(Dictionary<string, Module> modules)
{
    long sumLowPulses = 0;
    long sumHighPulses = 0;

    for(int i = 0; i < 1000; i++)
    {
        (long lowPulses, long highPulses) = PressButton(modules);

        sumLowPulses += lowPulses;
        sumHighPulses += highPulses;
    }

    return sumLowPulses * sumHighPulses;
}


long Part2(Dictionary<string, Module> modules)
{
    // By looking at the graph we found that the rx module
    // is dependent on one Conjunction module and that module
    // is dependent on a small number of other Conjunction modules.
    // (Relevant Conjunction Modules RCM)
    // 
    // For the RX module to be triggered, those RCM modules need to send
    // a high pulse.
    // By running a few thousand button presses and logging at which button
    // presses they send high and low pulses, we observed that these
    // modules send high pulses in a fixed cyclic pattern. 
    //
    // To calculate the button presses needed for rx to be activated, we need
    // to read out the cycles for these RCM modules and calculate the
    // Least common multiple.

    if (!modules.ContainsKey("rx")) 
        throw new ApplicationException("Unexpected network setup, no rx module");

    Module rx = modules["rx"];
    if (rx.ConnectedFrom.Count != 1) 
        throw new ApplicationException("Unexpected network setup, rx has not exactly one input");
        
    Module beforeRx = modules[rx.ConnectedFrom.First()];
    if(beforeRx is not ConjunctionModule) 
        throw new ApplicationException("Unexpected network setup, rx's input is not a conjunction");

    List<ConjunctionModule> relevantConjectionModules = new();

    foreach(var module in beforeRx.ConnectedFrom.Select(name => modules[name]))
    { 
        if (module is ConjunctionModule conjunctionModule)
        {
            relevantConjectionModules.Add(conjunctionModule);
        }
        else throw new ApplicationException("Unexpected network setup, one relevant module is not a conjunction");
    }

    Module.StepCounter = 0;

    while (relevantConjectionModules.Any(m => m.HighCycleFound() == 0))
    {
        Module.StepCounter++;
        PressButton(modules);
    }

    return relevantConjectionModules.Select(m => m.HighCycleFound()).Aggregate(LCM);

    long LCM(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }

    long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}


(long lowPulses, long HighPulses) PressButton(Dictionary<string, Module> modules)
{
    long lowPulses = 0;
    long highPulses = 0;

    Queue<Pulse> q = new Queue<Pulse>();
    q.Enqueue(new Pulse { Sender = "button", Receiver = BroadcasterModule.BroadCasterModuleName, HighPulse = false });

    while (q.TryDequeue(out var pulse))
    {
        if (pulse.HighPulse) highPulses++; else lowPulses++;

        var module = modules[pulse.Receiver];

        var newPulses = module.ReceivePulse(pulse);

        foreach (var newPulse in newPulses)
        {
            q.Enqueue(newPulse);
        }
    }

    return (lowPulses, highPulses);
}

public record struct Pulse
{
    public string Sender;
    public string Receiver;
    public bool HighPulse;
}

public abstract class Module
{
    public Module(string name, string[] connectedTo)
    {
        Name = name;
        ConnectedTo = connectedTo;
    }
    protected Pulse[] SendPulse(bool HighPulse)
    {
        return ConnectedTo
            .Select(m => new Pulse { Sender = Name, Receiver = m, HighPulse = HighPulse })
            .ToArray();
    }

    public virtual void RegisterPossibleSender(string sender)
    {
        connectedFrom.Add(sender);
    }

    public string Name { get; private set; }

    public string[] ConnectedTo { get; private set; }

    private HashSet<string> connectedFrom = new();

    public IReadOnlySet<string> ConnectedFrom => connectedFrom;

    public abstract Pulse[] ReceivePulse(Pulse pulse);

    public static long StepCounter = 0;

    public static Dictionary<string, Module> BuildModules(string[] input)
    {
        Dictionary<string, Module> modules = new();

        foreach (string line in input)
        {
            var parts = line.Split("->", StringSplitOptions.TrimEntries);
            var connectedTo = parts[1].Split(',', StringSplitOptions.TrimEntries);

            Module module = ModuleFactory(parts[0], connectedTo);
            modules.Add(module.Name, module);
        }

        var allModules = modules.Values.ToArray();

        foreach (var module in allModules)
        {
            foreach (string connectedTo in module.ConnectedTo)
            {
                if (!modules.TryGetValue(connectedTo, out var receiveModule))
                {
                    receiveModule = ModuleFactory(connectedTo, []);
                    modules.Add(connectedTo, receiveModule);
                }
                receiveModule.RegisterPossibleSender(module.Name);
            }
        }

        return modules;
    }

    private static Module ModuleFactory(string typeAndName, string[] connectedTo)
    {
        return typeAndName switch
        {
            BroadcasterModule.BroadCasterModuleName => new BroadcasterModule(typeAndName, connectedTo),
            ['%', .. var rest] => new FlipFlopModule(rest, connectedTo),
            ['&', .. var rest] => new ConjunctionModule(rest, connectedTo),
            _ => new TerminalModule(typeAndName, connectedTo)
        };
    }
}

public class BroadcasterModule : Module
{
    public BroadcasterModule(string name, string[] connectedTo) : base(name, connectedTo)
    {
    }

    public override Pulse[] ReceivePulse(Pulse pulse) => SendPulse(pulse.HighPulse);

    public const string BroadCasterModuleName = "broadcaster";
}

public class FlipFlopModule : Module
{
    public FlipFlopModule(string name, string[] connectedTo) : base(name, connectedTo)
    {
    }

    private bool IsOn = false;
    public override Pulse[] ReceivePulse(Pulse pulse)
    {
        if (!pulse.HighPulse)
        {
            IsOn = !IsOn;
            return SendPulse(IsOn);
        }
        return [];
    }
}

public class ConjunctionModule : Module
{
    public ConjunctionModule(string name, string[] connectedTo) : base(name, connectedTo)
    {
    }

    private Dictionary<string, bool> inputPulses = new();

    public override void RegisterPossibleSender(string sender)
    {
        base.RegisterPossibleSender(sender);

        if (!inputPulses.ContainsKey(sender))
        {
            inputPulses[sender] = false;
        }
    }

    private List<long> highHits = [];

    public override Pulse[] ReceivePulse(Pulse pulse)
    {
        inputPulses[pulse.Sender] = pulse.HighPulse;

        bool allSet = inputPulses.Values.All(b => b);

        if (!allSet && (highHits.Count <= 3) && highHits.LastOrDefault() != Module.StepCounter)
        {
            highHits.Add(Module.StepCounter);
        }

        return SendPulse(!allSet);
    }

    public long HighCycleFound()
    {
        if(highHits.Count == 3)
        {
            long diff = highHits[2] - highHits[1];
            if (highHits[1] - highHits[0] == diff) return diff;
        }

        return 0;
    }
}

public class TerminalModule : Module
{
    public TerminalModule(string name, string[] connectedTo) : base(name, connectedTo)
    {
    }

    public override Pulse[] ReceivePulse(Pulse pulse) => [];
}
