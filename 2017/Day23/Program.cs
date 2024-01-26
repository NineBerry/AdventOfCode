// #define Sample
{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day23\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day23\Full.txt";
#endif

    Instruction[] program =
        File.ReadAllLines(fileName)
        .Select(s => Instruction.Factory(s, false))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(program));
    Console.WriteLine("Part 2: " + Part2Cheat());
    Console.ReadLine();
}

long Part1(Instruction[] program)
{
    int mulCount = 0;
    Computer computer = new Computer();
    computer.BeforeInstruction += Computer_BeforeInstruction; ;    
    
    computer.ExecuteProgram(program);
    return mulCount;

    void Computer_BeforeInstruction(Computer computer, Instruction instruction)
    {
        if(instruction is MulInstruction)
        {
            mulCount++;
        }
    }
}

/// I analysed the program and found that it uses a very inefficient algorithm to 
/// check whether certain numbers are prime. It will count all non-prime numbers in 
/// a range of numbers with a certain distance.
/// In my input, it checked every 17th number from 109300 to 126300.
/// We can calculate that using a much better algorithm.

long Part2Cheat()
{
    int countNotPrime = 0;

    for (int b = 109300; b <= 126300; b += 17)
    {
        if (!IsPrime(b))
        {
            countNotPrime++;
        };
    }

    return countNotPrime;
}

bool IsPrime(int number)
{
    if (number <= 1) return false;
    if (number == 2) return true;
    if (number % 2 == 0) return false;

    var boundary = (int)Math.Floor(Math.Sqrt(number));

    for (int i = 3; i <= boundary; i += 2)
        if (number % i == 0)
            return false;

    return true;
}

long Part2(Instruction[] program)
{
    Computer computer = new Computer();
    computer.SetRegisterValue("a", 1);
    computer.ExecuteProgram(program);
    return computer.GetRegisterValue("h");
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
        if (!long.TryParse(input, out long result))
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

    public string GetRegistersAsString()
    {
        string s = "";
        foreach(var reg in registers.Keys.Order())
        {
            s += reg + ": " + registers[reg] + ",";
        }

        return s;
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
            "set" => new SetInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "sub" => new SubInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "mul" => new MulInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            "jnz" => new JumpNotZeroInstruction { Parameter1 = parameters[1], Parameter2 = parameters[2] },
            _ => throw new ApplicationException("Unknown Instruction")
        };
    }
}


public record SetInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        computer.SetRegisterValue(Parameter1, computer.GetConstOrRegisterValue(Parameter2));
        return 1;
    }
}

public record SubInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        computer.SetRegisterValue(Parameter1, computer.GetConstOrRegisterValue(Parameter1) - computer.GetConstOrRegisterValue(Parameter2));
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

public record JumpNotZeroInstruction : Instruction
{
    public string Parameter1 = "";
    public string Parameter2 = "";

    public override long Execute(Computer computer)
    {
        if (computer.GetConstOrRegisterValue(Parameter1) != 0)
        {
            return computer.GetConstOrRegisterValue(Parameter2);
        }
        return 1;
    }
}


