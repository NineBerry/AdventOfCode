// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day23\Sample.txt";
    string resultRegister = "a";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day23\Full.txt";
    string resultRegister = "b";
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
    IncrementInstruction incrementInstruction = new IncrementInstruction{ Register = "a"};
    Instruction[] modifiedProgram = [incrementInstruction, ..program];
    
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

        while(InstructionPointer >= 0 && InstructionPointer < Program.Length)
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
        string abbr = input.Substring(0, 3);
        string register = input.Substring(4, 1);
        int offset = Regex.Matches(input, "-?\\d+").Select(m => int.Parse(m.Value)).FirstOrDefault();

        return abbr switch
        {
            "hlf" => new HalfInstruction { Register = register },
            "tpl" => new TripleInstruction { Register = register },
            "inc" => new IncrementInstruction { Register = register },
            "jmp" => new JumpInstruction { Offset = offset },
            "jie" => new JumpIfEvenInstruction { Register = register, Offset = offset },
            "jio" => new JumpIfOneInstruction { Register = register, Offset = offset },
            _ => throw new ApplicationException("Unknown Instruction: " + abbr)
        };
    }
}

public record HalfInstruction : Instruction
{
    public string Register = "";

    public override int Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) / 2);
        return +1;
    }
}
public record TripleInstruction : Instruction
{
    public string Register = "";

    public override int Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) * 3);
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

public record JumpInstruction : Instruction
{
    public int Offset;

    public override int Execute(Computer computer)
    {
        return Offset;
    }
}

public record JumpIfEvenInstruction : Instruction
{
    public string Register = "";
    public int Offset;

    public override int Execute(Computer computer)
    {
        if (computer.GetRegisterValue(Register) % 2 == 0) return Offset;
        return +1;
    }
}

public record JumpIfOneInstruction : Instruction
{
    public string Register = "";
    public int Offset;

    public override int Execute(Computer computer)
    {
        if (computer.GetRegisterValue(Register) == 1) return Offset;
        return +1;
    }
}

