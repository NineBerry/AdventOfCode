// #define Sample

using System.Collections.Concurrent;

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2017\Day18\SamplePart1.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2017\Day18\SamplePart2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2017\Day18\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2017\Day18\Full.txt";
#endif

    Instruction[] programPart1 = 
        File.ReadAllLines(fileNamePart1)
        .Select(s => Instruction.Factory(s, false))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(programPart1));

    Instruction[] programPart2 =
        File.ReadAllLines(fileNamePart2)
        .Select(s => Instruction.Factory(s, true))
        .ToArray();
    Console.WriteLine("Part 2: " + await Part2(programPart2));
    Console.ReadLine();
}

long Part1(Instruction[] program)
{
    Computer computer = new Computer();
    computer.ExecuteProgram(program);
    return computer.PlayingSoundFrequency;
}

async Task<long> Part2(Instruction[] program2)
{
    BlockingCollection<long> from0To1 = new();
    BlockingCollection<long> from1To0 = new();
    CancellationTokenSource cts = new CancellationTokenSource();
    int sendByComputer1Count = 0;
    bool computer0Receiving = false;
    bool computer1Receiving = false;
    object sync = new object();

    Computer computer0 = new Computer();
    computer0.SetRegisterValue("p", 0);
    computer0.Name = "0";
    computer0.FromQueue = from1To0;
    computer0.ToQueue = from0To1;
    computer0.CancellationToken = cts.Token;
    computer0.BeforeInstruction += Computer_BeforeInstruction;

    Computer computer1 = new Computer();
    computer1.SetRegisterValue("p", 1);
    computer1.Name = "1";
    computer1.FromQueue = from0To1;
    computer1.ToQueue = from1To0;
    computer1.CancellationToken = cts.Token;
    computer1.BeforeInstruction += Computer_BeforeInstruction;

    Task computer1Task = Task.Run(() => computer0.ExecuteProgram(program2));
    Task computer2Task = Task.Run(() => computer1.ExecuteProgram(program2));

    await Task.WhenAll(computer1Task, computer2Task);

    return sendByComputer1Count;

    void Computer_BeforeInstruction(Computer computer, Instruction instruction)
    {
        lock (sync)
        {
            if (computer.Name == "1" && instruction is SendInstruction)
            {
                sendByComputer1Count++;
            }

            if (computer.Name == "0")
            {
                computer0Receiving = instruction is ReceiveInstruction && computer.FromQueue.Count == 0;
            }

            if (computer.Name == "1")
            {
                computer1Receiving = instruction is ReceiveInstruction && computer.FromQueue.Count == 0;
            }

            if(computer0Receiving && computer1Receiving && from0To1.Count == 0 && from1To0.Count == 0)
            {
                // Deadlock
                cts.Cancel();
            }
        }
    }
}


public record Computer
{
    public delegate void BeforeInstructionHandler(Computer computer, Instruction instruction);

    public event BeforeInstructionHandler? BeforeInstruction;

    private Dictionary<string, long> registers = new();

    public long GetRegisterValue(string register)
    {
        registers.TryGetValue(register, out long value);
        return value;
    }

    public long GetConstOrRegisterValue(string input)
    {
        if(!long.TryParse(input, out long result))
        {
            result = GetRegisterValue(input);
        }

        return result;
    }

    public void SetRegisterValue(string register, long value)
    {
        registers[register] = value;
    }

    public long InstructionPointer = 0;
    public long PlayingSoundFrequency = 0;
    public string Name = "";

    public BlockingCollection<long> ToQueue;
    public BlockingCollection<long> FromQueue;
    public CancellationToken CancellationToken;

    public Instruction[] Program = [];

    public void ExecuteProgram(Instruction[] program)
    {
        InstructionPointer = 0;
        Program = program;

        while (InstructionPointer >= 0 && InstructionPointer < Program.Length)
        {
            if (CancellationToken.IsCancellationRequested) break;

            Instruction currentInstruction = Program[InstructionPointer];
            BeforeInstruction?.Invoke(this, currentInstruction);
            InstructionPointer += currentInstruction.Execute(this);
        }
    }

}

public abstract record Instruction
{
    public abstract long Execute(Computer computer);

    public static Instruction Factory(string input, bool part2)
    {
        string[] parameters = input.Split(' ');
        
        return parameters[0] switch
        {
            "snd" when part2 => new SendInstruction{ Parameter1 = parameters[1] },
            "snd" => new SoundInstruction { Parameter1 = parameters[1] },
            "rcv" when part2 => new ReceiveInstruction { Parameter1 = parameters[1] },
            "rcv" => new ReceiveSoundInstruction { Parameter1 = parameters[1] },

            "set" => new SetInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "add" => new AddInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "mul" => new MulInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "mod" => new ModInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "jgz" => new JumpGreaterZeroInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            _ => throw new ApplicationException("Unknown Instruction")
        };
    }
}

public record SoundInstruction: Instruction
{
    public string Parameter1 = "";

    public override long Execute(Computer computer)
    {
        computer.PlayingSoundFrequency = computer.GetConstOrRegisterValue(Parameter1);
        return 1;
    }
}

public record SetInstruction: Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        computer.SetRegisterValue(Parameter1, computer.GetConstOrRegisterValue(Parameter2));
        return 1;
    }
}

public record AddInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        computer.SetRegisterValue(Parameter1, computer.GetConstOrRegisterValue(Parameter1) + computer.GetConstOrRegisterValue(Parameter2));
        return 1;
    }
}

public record MulInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        computer.SetRegisterValue(Parameter1, computer.GetConstOrRegisterValue(Parameter1) * computer.GetConstOrRegisterValue(Parameter2));
        return 1;
    }
}

public record ModInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        computer.SetRegisterValue(Parameter1, computer.GetConstOrRegisterValue(Parameter1) % computer.GetConstOrRegisterValue(Parameter2));
        return 1;
    }
}

public record ReceiveSoundInstruction : Instruction
{
    public string Parameter1 = "";

    public override long Execute(Computer computer)
    {
        if(computer.GetConstOrRegisterValue(Parameter1) != 0)
        {
            return computer.Program.Length + 1;
        }
        return 1;
    }
}

public record JumpGreaterZeroInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        if (computer.GetConstOrRegisterValue(Parameter1) > 0)
        {
            return computer.GetConstOrRegisterValue(Parameter2);
        }
        return 1;
    }
}

public record SendInstruction : Instruction
{
    public string Parameter1 = "";

    public override long Execute(Computer computer)
    {
        computer.ToQueue.Add(computer.GetConstOrRegisterValue(Parameter1));
        return 1;
    }
}

public record ReceiveInstruction : Instruction
{
    public string Parameter1 = "";

    public override long Execute(Computer computer)
    {
        try
        {
            long value = computer.FromQueue.Take(computer.CancellationToken);
            computer.SetRegisterValue(Parameter1, value);
            return 1;
        }
        catch (OperationCanceledException)
        {
            return computer.Program.Length + 1;
        }
    }
}

