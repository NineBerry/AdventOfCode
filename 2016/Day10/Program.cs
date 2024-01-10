// #define Sample

using System.Text.RegularExpressions;
using ObjectAddress = (BotNetObjectType Type, int ID);

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day10\Sample.txt";
    int distributionLow = 2;
    int distributionHigh = 3;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day10\Full.txt";
    int distributionLow = 17;
    int distributionHigh = 61;
#endif

    string[] input = File.ReadAllLines(fileName);

    BotNet botNet = new BotNet();
    Instruction[] instructions = input.Select(BotNet.ParseInstruction).ToArray();

    Console.WriteLine("Part 1: " + Part1(botNet, instructions, distributionLow, distributionHigh));
    Console.WriteLine("Part 2: " + Part2(botNet));
    Console.ReadLine();
}

long Part1(BotNet botNet, Instruction[] instructions, int distributionLow, int distributionHigh)
{
    int interestingBot = 0;

    Bot.DistributionEvent += (Bot bot, int lowNumber, int HighNumber) =>
    {
        if(lowNumber == distributionLow && HighNumber == distributionHigh)
        {
            interestingBot = bot.ID;
        }
    };

    Instruction[] setupInstructions = instructions.Where(x => x is SetupInstruction).ToArray();
    Instruction[] valueInstructions = instructions.Where(x => x is ValueInstruction).ToArray();

    botNet.ExecuteInstructions(setupInstructions);
    botNet.ExecuteInstructions(valueInstructions);

    return interestingBot;
}

long Part2(BotNet botNet)
{
    var output0Value = botNet.GetObject((BotNetObjectType.Output, 0)).GetValues().First();
    var output1Value = botNet.GetObject((BotNetObjectType.Output, 1)).GetValues().First();
    var output2Value = botNet.GetObject((BotNetObjectType.Output, 2)).GetValues().First();

    return output0Value * output1Value * output2Value;
}

public class BotNet
{
    private Dictionary<int, Bot> bots = [];
    private Dictionary<int, Output> outputs = [];

    public BotNetObject GetObject(ObjectAddress address)
    {
        if (address.Type == BotNetObjectType.Bot)
        {
            if (!bots.TryGetValue(address.ID, out var bot))
            {
                bot = new Bot(address.ID, this);
                bots.Add(address.ID, bot);
            }

            return bot;
        }
        else if (address.Type == BotNetObjectType.Output)
        {
            if (!outputs.TryGetValue(address.ID, out var output))
            {
                output = new Output(address.ID, this);
                outputs.Add(address.ID, output);
            }

            return output;

        }
        else
        {
            throw new ApplicationException("Unknown object type");
        }
    }

    public void ExecuteInstructions(Instruction[] instructions)
    {
        foreach(var instruction in instructions)
        {
            instruction.Execute(this);
        }
    }

    public static Instruction ParseInstruction(string s)
    {
        if (s.StartsWith("value"))
        {
            return new ValueInstruction(s);
        }
        else if (s.StartsWith("bot"))
        {
            return new SetupInstruction(s);
        }
        else throw new ArgumentException("Invalid instruction");
    }
}

public enum BotNetObjectType
{
    Bot,
    Output
}

public abstract class BotNetObject
{
    public BotNetObject(int id, BotNet net)
    {
        ID = id;
        BotNet = net;
    }

    public readonly int ID;
    public readonly BotNet BotNet;

    protected List<int> values = [];

    public virtual void ReceiveValue(int value)
    {
        values.Add(value);
    }

    public int[] GetValues() => values.ToArray();
}

public class Bot : BotNetObject
{
    public delegate void DistributionEventHandler(Bot bot, int lowNumber, int HighNumber);

    public static event DistributionEventHandler DistributionEvent;

    private ObjectAddress LowTarget;
    private ObjectAddress HighTarget;

    public Bot(int id, BotNet net) : base(id, net)
    {
    }

    public void Setup(ObjectAddress lowTarget, ObjectAddress highTarget)
    {
        LowTarget = lowTarget;
        HighTarget = highTarget;
    }

    public override void ReceiveValue(int value)
    {
        base.ReceiveValue(value);

        if(values.Count == 2)
        {
            int low = values.Min();
            int high = values.Max();
            values.Clear();

            DistributionEvent?.Invoke(this, low, high);

            if (LowTarget.ID == 0 && HighTarget.ID == 0) throw new ApplicationException("Not initialized");

            BotNet.GetObject(LowTarget).ReceiveValue(low);
            BotNet.GetObject(HighTarget).ReceiveValue(high);
        }
    }
}

public class Output: BotNetObject
{
    public Output(int id, BotNet net) : base(id, net)
    {
    }
}

public abstract class Instruction
{
    public abstract void Execute(BotNet botNet);
}

public class ValueInstruction: Instruction
{
    public ValueInstruction(string input)
    {
        int[] values = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        Value = values[0];  
        BotID = values[1];
    }

    public int BotID;
    public int Value;

    public override void Execute(BotNet botNet)
    {
        botNet.GetObject((BotNetObjectType.Bot, BotID)).ReceiveValue(Value);
    }
}

public class SetupInstruction: Instruction
{
    public SetupInstruction(string input)
    {
        var match = Regex.Match(input, "bot (\\d+) gives low to ((?>output|bot) \\d+) and high to ((?>output|bot) \\d+)");
        
        BotID = int.Parse(match.Groups[1].Value);
        LowAddress = ParseAddress(match.Groups[2].Value);
        HighAddress = ParseAddress(match.Groups[3].Value);
    }

    public int BotID;
    public ObjectAddress LowAddress;
    public ObjectAddress HighAddress;

    public override void Execute(BotNet botNet)
    {
        if (botNet.GetObject((BotNetObjectType.Bot, BotID)) is Bot bot)
        {
            bot.Setup(LowAddress, HighAddress);
        }
    }
    private ObjectAddress ParseAddress(string value)
    {
        int objectID = int.Parse(Regex.Match(value, "\\d+").Value);

        if (value.StartsWith("bot")) return (BotNetObjectType.Bot, objectID);
        if (value.StartsWith("output")) return (BotNetObjectType.Output, objectID);

        throw new ArgumentException("Invalid Object Address");
    }
}