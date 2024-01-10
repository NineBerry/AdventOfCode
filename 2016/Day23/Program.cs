// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day23\Sample.txt";
    string resultRegister = "a";
    int initalValuePart1 = 2;
    int initalValuePart2 = 2;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day23\Full.txt";
    string resultRegister = "a";
    int initalValuePart1 = 7;
    int initalValuePart2 = 2;
#endif

    string[] input = File.ReadAllLines(fileName);
    Instruction[] program = input.Select(Instruction.Factory).ToArray();

    Console.WriteLine("Part 1: " + Part1(program, resultRegister, initalValuePart1));
    Console.WriteLine("Part 2: " + Part2(program, resultRegister, initalValuePart2));
    Console.ReadLine();
}

long Part1(Instruction[] program, string resultRegister, int initalValue)
{
    CopyInstruction copyInstruction = new CopyInstruction { From = initalValue.ToString(), Register = "a" };
    Instruction[] modifiedProgram = [copyInstruction, ..program];
    
    Computer computer = new();
    computer.ExecuteProgram(modifiedProgram);
    return computer.GetRegisterValue(resultRegister);
}

long Part2(Instruction[] program, string resultRegister, int initalValue)
{
    CopyInstruction copyInstruction = new CopyInstruction { From = initalValue.ToString(), Register = "a" };
    Instruction[] modifiedProgram = [copyInstruction, .. program];

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
            CheckPossibleOptimization();
            InstructionPointer += Program[InstructionPointer].Execute(this);
        }
    }

    private void CheckPossibleOptimization()
    {
        // throw new NotImplementedException();
    }

    internal void ToggleInstruction(int value)
    {
        int instructionIndex = InstructionPointer + value;
        if(instructionIndex >= 0 && instructionIndex < Program.Length)
        {
            Program[instructionIndex] = Program[instructionIndex].GetToggledInstruction();
        }
    }
}

public abstract record Instruction
{
    public abstract int Execute(Computer computer);
    public abstract Instruction GetToggledInstruction();

    public static Instruction Factory(string input)
    {
        string[] parts = input.Split(" ");
        string abbr = parts[0];

        return abbr switch
        {
            "cpy" => new CopyInstruction { From = parts[1], Register = parts[2] },
            "inc" => new IncrementInstruction { Register = parts[1] },
            "dec" => new DecrementInstruction { Register = parts[1] },
            "jnz" => new JumpIfNotZeroInstruction { ToCheck = parts[1], Offset = parts[2] },
            "tgl" => new ToggleInstruction { Register = parts[1] },
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
        if (!int.TryParse(From, out int value))
        {
            value = computer.GetRegisterValue(From);
        }

        computer.SetRegisterValue(Register, value);
        return +1;
    }

    public override Instruction GetToggledInstruction()
    {
            return new JumpIfNotZeroInstruction { ToCheck = From, Offset = Register};
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

    public override Instruction GetToggledInstruction()
    {
        return new DecrementInstruction { Register = Register };
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
    public override Instruction GetToggledInstruction()
    {
        return new IncrementInstruction { Register = Register };
    }
}

public record JumpIfNotZeroInstruction : Instruction
{
    public string ToCheck = "";
    public string Offset = "";

    public override int Execute(Computer computer)
    {
        if (!int.TryParse(ToCheck, out int valueToCheck))
        {
            valueToCheck = computer.GetRegisterValue(ToCheck);
        }

        if (!int.TryParse(Offset, out int valueOffset))
        {
            valueOffset = computer.GetRegisterValue(Offset);
        }

        if (valueToCheck != 0) return valueOffset;
        return +1;
    }

    public override Instruction GetToggledInstruction()
    {
        if(int.TryParse(Offset, out _))
        {
            return new NopInstruction();
        }

        return new CopyInstruction { From= ToCheck, Register = Offset };
    }
}

public record ToggleInstruction : Instruction
{
    public string Register = "";

    public override int Execute(Computer computer)
    {
        int value = computer.GetRegisterValue(Register);
        
        computer.ToggleInstruction(value);
        return +1;
    }

    public override Instruction GetToggledInstruction()
    {
        return new IncrementInstruction { Register = Register };
    }

}


public record NopInstruction : Instruction
{
    public override int Execute(Computer computer)
    {
        return +1;
    }

    public override Instruction GetToggledInstruction()
    {
        return this;
    }
}

