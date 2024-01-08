//#define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day12\Sample.txt";
    string resultRegister = "a";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day12\Full.txt";
    string resultRegister = "a";
#endif

    string[] input = File.ReadAllLines(fileName);
    Instruction[] program = input.Select(Instruction.Factory).ToArray();

    Console.WriteLine("Part 1: " + Part1(program, resultRegister));
    Console.WriteLine("Part 2: " + Part2(program, resultRegister));
    Console.ReadLine();
}

long Part1(Instruction[] program, string resultRegister)
{
    Computer computer = new();
    computer.ExecuteProgram(program);
    return computer.GetRegisterValue(resultRegister);
}

long Part2(Instruction[] program, string resultRegister)
{
    IncrementInstruction incrementInstruction = new IncrementInstruction { Register = "c" };
    Instruction[] modifiedProgram = [incrementInstruction, .. program];

    Computer computer = new();
    computer.ExecuteProgram(modifiedProgram);
    return computer.GetRegisterValue(resultRegister);
}

public record Computer
{
    private Dictionary<string, int> registers = new();

    public int GetRegisterValue(string register)
    {
        registers.TryGetValue(register, out int value);
        return value;
    }
    public void SetRegisterValue(string register, int value)
    {
        registers[register] = value;
    }

    public int InstructionPointer = 0;

    public Instruction[] Program = [];

    public void ExecuteProgram(Instruction[] program)
    {
        registers.Clear();
        InstructionPointer = 0;
        Program = program;

        while (InstructionPointer >= 0 && InstructionPointer < Program.Length)
        {
            InstructionPointer += Program[InstructionPointer].Execute(this);
        }
    }
}

public abstract record Instruction
{
    public abstract int Execute(Computer computer);

    public static Instruction Factory(string input)
    {
        string[] parts = input.Split(" ");
        string abbr = parts[0];

        return abbr switch
        {
            "cpy" => new CopyInstruction { From = parts[1], Register = parts[2]},
            "inc" => new IncrementInstruction { Register = parts[1] },
            "dec" => new DecrementInstruction { Register = parts[1] },
            "jnz" => new JumpIfNotZeroInstruction { ToCheck = parts[1], Offset = int.Parse(parts[2]) },
            _ => throw new ApplicationException("Unknown Instruction: " + abbr)
        };
    }
}

public record CopyInstruction : Instruction
{
    public string From = "";
    public string Register = "";

    public override int Execute(Computer computer)
    {
        if(!int.TryParse(From, out int value))
        {
            value = computer.GetRegisterValue(From);
        }
        
        computer.SetRegisterValue(Register, value);
        return +1;
    }
}

public record IncrementInstruction : Instruction
{
    public string Register = "";

    public override int Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) + 1);
        return +1;
    }
}

public record DecrementInstruction : Instruction
{
    public string Register = "";

    public override int Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) - 1);
        return +1;
    }
}

public record JumpIfNotZeroInstruction : Instruction
{
    public string ToCheck = "";
    public int Offset;

    public override int Execute(Computer computer)
    {
        if (!int.TryParse(ToCheck, out int value))
        {
            value = computer.GetRegisterValue(ToCheck);
        }
        
        if (value != 0) return Offset;
        return +1;
    }
}

