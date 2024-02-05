// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day08\Full.txt";
#endif

    List<Instruction> program =
        File.ReadAllLines(fileName)
        .Select(Instruction.Factory)
        .ToList();

    Console.WriteLine("Part 1: " + Part1(program));
    Console.WriteLine("Part 2: " + Part2(program));
    Console.ReadLine();
}

long Part1(List<Instruction> program)
{
    TestProgram(program.ToArray(), out long accumulator);
    return accumulator;
}

long Part2(List<Instruction> program)
{
    for (int i = 0; i < program.Count; i++)
    {
        if (program[i] is AccumulatorInstruction) continue;

        List<Instruction> modified = new(program);
        
        if (program[i] is JumpInstruction)
        {
            modified[i] = new NOOPInstruction { Parameter = modified[i].Parameter };
        }
        else
        {
            modified[i] = new JumpInstruction { Parameter = modified[i].Parameter };
        }

        if(!TestProgram(modified.ToArray(), out long accumulator))
        {
            return accumulator;
        }
    }

    return 0;
}


bool TestProgram(Instruction[] program, out long accumultator)
{
    CancellationTokenSource cts = new CancellationTokenSource();
    long accumulatorEndValue = 0;
    bool aborted = false;
    HashSet<long> seenInstructions = [];

    Computer computer = new Computer();
    computer.CancellationToken = cts.Token;
    computer.BeforeInstruction += Computer_BeforeInstruction;
    computer.ExecuteProgram(program);

    if (!aborted)
    {
        accumulatorEndValue = computer.Accumulator;
    }
    accumultator = accumulatorEndValue;

    return aborted;

    void Computer_BeforeInstruction(Computer computer, Instruction instruction)
    {
        if (seenInstructions.Contains(computer.InstructionPointer))
        {
            accumulatorEndValue = computer.Accumulator;
            cts.Cancel();
            aborted = true;
        }
        else
        {
            seenInstructions.Add(computer.InstructionPointer);
        }
    }
}


public record Computer
{
    public delegate void BeforeInstructionHandler(Computer computer, Instruction instruction);
    public event BeforeInstructionHandler? BeforeInstruction;

    public long Accumulator = 0;
    public long InstructionPointer = 0;
    public CancellationToken CancellationToken;

    private Instruction[] Program = [];

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
    public int Parameter;
    
    public abstract long Execute(Computer computer);

    public static Instruction Factory(string input)
    {
        string[] parameters = input.Split(' ');
        string command = parameters[0];
        int valueParameter = int.Parse(parameters[1]);

        return command switch
        {
            "acc" => new AccumulatorInstruction { Parameter = valueParameter },
            "jmp" => new JumpInstruction { Parameter = valueParameter },
            "nop" => new NOOPInstruction { Parameter = valueParameter },
            _ => throw new ApplicationException("Unknown Instruction")
        };
    }
}

public record AccumulatorInstruction: Instruction
{
    public override long Execute(Computer computer)
    {
        computer.Accumulator += Parameter;
        return 1;
    }
}

public record JumpInstruction : Instruction
{
    public override long Execute(Computer computer)
    {
        return Parameter;
    }
}
public record NOOPInstruction : Instruction
{
    public override long Execute(Computer computer)
    {
        return 1;
    }
}