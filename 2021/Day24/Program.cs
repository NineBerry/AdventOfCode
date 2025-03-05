using System.Text.RegularExpressions;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day24\Full.txt";
    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string fullProgram)
{
    return FindFirstValidNumber(fullProgram, true);
}
long Part2(string fullProgram)
{
    return FindFirstValidNumber(fullProgram, false);
}

long FindFirstValidNumber(string fullProgram, bool topDown)
{
    string[] stringParts = 
        Regex
        .Matches(fullProgram, @"inp(.+?)(?=inp|$)", RegexOptions.Singleline)
        .Select(m => m.Value)
        .ToArray();
    Instruction[][] parts = stringParts.Select(s => Instruction.Factory(s.ReplaceLineEndings(Environment.NewLine).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))).ToArray();

    long firstFound = 0;
    HashSet<(int PartIndex, int InputZ)> unsuccessfulBranches = [];

    FindValidNumbersRecursive(0, "", 0);

    bool FindValidNumbersRecursive(int partIndex, string numbersSoFar, int inputZ)
    {
        if (unsuccessfulBranches.Contains((partIndex, inputZ)))
        {
           return false;
        }

        if (partIndex == parts.Length)
        {
            if (inputZ == 0)
            {
                firstFound = long.Parse(numbersSoFar);
                return true;
            }

            return false;
        }

        bool result = false;

        var possibleOutputs = GetPossibleOutputs(parts[partIndex], inputZ, topDown);
        foreach (var input in possibleOutputs)
        {
            result = result || FindValidNumbersRecursive(partIndex + 1, numbersSoFar + input.W, input.Z);
        }

        if (!result)
        {
            unsuccessfulBranches.Add((partIndex, inputZ));
        }

        return result;
    }

    return firstFound;
}

List<(int W, int Z)> GetPossibleOutputs(Instruction[] instructions, int inputZ, bool topDown)
{
    if (inputZ < 0) return [];

    List<(int W, int Z)> result = [];

    var range = Enumerable.Range(1, 9);
    if(topDown) range = range.Reverse();

    foreach (var w in range)
    {
        Computer computer = new();
        computer.SetRegisterValue("z", inputZ);
        computer.Input += c => w;

        computer.ExecuteProgram(instructions);

        var resultingZ = computer.GetRegisterValue("z");

        if (resultingZ >= 0)
        {
            result.Add((w, resultingZ));
        }
    }

    return result;
}

public record Computer
{
    public delegate int DoInput(Computer computer);
    public event DoInput? Input = null;

    public int w = 0;
    public int x = 0;
    public int y = 0;
    public int z = 0;

    public int GetRegisterValue(string register)
    {
        return register switch
        {
            "w" => w,
            "x" => x,
            "y" => y,
            "z" => z,
            _ => throw new ApplicationException("Unknown register: " + register)
        };
    }
    public void SetRegisterValue(string register, int value)
    {
        switch (register)
        {
            case "w":
                w = value;
                break;
            case "x":
                x = value;
                break;
            case "y":
                y = value;
                break;
            case "z":
                z = value;
                break;
            default:
                throw new ApplicationException("Unknown register: " + register);
        }
    }

    public int InstructionPointer = 0;
    public Instruction[] Program = [];

    public void ExecuteProgram(Instruction[] program)
    {
        InstructionPointer = 0;
        Program = program;

        while (InstructionPointer >= 0 && InstructionPointer < Program.Length)
        {
            Program[InstructionPointer].Execute(this);
            InstructionPointer += 1;
        }
    }

    public int GetInput()
    {
        if (Input == null) throw new ApplicationException("Input event not set");
        return Input(this);
    }

}

public abstract record Instruction
{
    public string Register = "";
    public string Operand = "";
    
    private bool? OperandIsRegister = null;
    private int OperandValue = 0;


    protected int GetOperandValue(Computer computer)
    {
        if(!OperandIsRegister.HasValue)
        {
            OperandIsRegister = !int.TryParse(Operand, out OperandValue);
        }

        return OperandIsRegister.Value ? computer.GetRegisterValue(Operand) : OperandValue;
    }

    public abstract void Execute(Computer computer);

    public static Instruction Factory(string input)
    {
        string[] parts = input.Split(" ");
        string abbr = parts[0];

        return abbr switch
        {
            "inp" => new InputInstruction { Register = parts[1] },
            "add" => new AddInstruction { Register = parts[1], Operand = parts[2] },
            "mul" => new MulInstruction { Register = parts[1], Operand = parts[2] },
            "div" => new DivInstruction { Register = parts[1], Operand = parts[2] },
            "mod" => new ModInstruction { Register = parts[1], Operand = parts[2] },
            "eql" => new EqlInstruction { Register = parts[1], Operand = parts[2] },
            _ => throw new ApplicationException("Unknown Instruction: " + abbr)
        };
    }

    public static Instruction[] Factory(string[] input)
    {
        return input.Select(Factory).ToArray();
    }
}

public record InputInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetInput());
    }
}

public record AddInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) + GetOperandValue(computer));
    }
}

public record MulInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) * GetOperandValue(computer));
    }
}

public record DivInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        if(GetOperandValue(computer) == 0) throw new Exception("Division by zero");

        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) / GetOperandValue(computer));
    }
}

public record ModInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        if (computer.GetRegisterValue(Register) < 0) throw new Exception("Division by zero");
        if (GetOperandValue(computer) <= 0) throw new Exception("Division by zero");
        
        computer.SetRegisterValue(Register, computer.GetRegisterValue(Register) %  GetOperandValue(computer));
    }
}

public record EqlInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        
        int res = computer.GetRegisterValue(Register) == GetOperandValue(computer) ? 1 : 0;
        computer.SetRegisterValue(Register, res);
    }
}


