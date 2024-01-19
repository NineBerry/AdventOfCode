// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day08\Full.txt";
#endif

    Instruction[] program = 
        File.ReadAllLines(fileName)
        .Select(s => new Instruction(s))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(program));
    Console.WriteLine("Part 2: " + Part2(program));
    Console.ReadLine();
}

long Part1(Instruction[] program)
{
    Computer computer = new Computer();

    foreach (var instruction in program)
    {
        instruction.Execute(computer);
    }
    
    return computer.Registers.Values.Max();
}

long Part2(Instruction[] program)
{
    int max = int.MinValue;

    Computer computer = new Computer();

    foreach (var instruction in program)
    {
        instruction.Execute(computer);
        max = computer.Registers.Values.Concat([max]).Max();
    }

    return max;
}

public class Computer
{
    public Dictionary<string, int> Registers = [];

    public int GetRegister(string name)
    {
        Registers.TryGetValue(name, out int result);
        return result;
    }

    public void SetRegister(string name, int value)
    {
        Registers[name] = value;
    }
}

public partial record Instruction
{
    [GeneratedRegex(@"([a-z]+) (inc|dec) (-?[0-9]+) if ([a-z]+) (.{1,2}) (-?[0-9]+)")]
    public static partial Regex InstructionRegex();

    public Instruction(string line) 
    {
        Match match = InstructionRegex().Match(line);
        if (match.Success)
        {
            TargetRegister = match.Groups[1].Value;
            Increment = match.Groups[2].Value == "inc";
            Operand = int.Parse(match.Groups[3].Value);
            CheckRegister = match.Groups[4].Value;
            CheckOperator = match.Groups[5].Value;
            CheckOperand = int.Parse(match.Groups[6].Value);
        }
        else throw new ArgumentException("Invalid instruction");
    }

    public void Execute(Computer computer)
    {
        int targetRegisterValue = computer.GetRegister(TargetRegister);
        int checkRegisterValue = computer.GetRegister(CheckRegister);

        if(CheckCondition(CheckOperator, checkRegisterValue, CheckOperand))
        {
            int sign = Increment ? 1 : -1;
            computer.SetRegister(TargetRegister, targetRegisterValue + sign * Operand);
        }
    }

    private static bool CheckCondition(string op, int value1, int value2)
    {
        return op switch
        {
            ">" => value1 > value2,
            "<" => value1 < value2,
            ">=" => value1 >= value2,
            "<=" => value1 <= value2,
            "==" => value1 == value2,
            "!=" => value1 != value2,
            _ => throw new ApplicationException("Unknown operator")
        };
    }

    public string TargetRegister;
    public bool Increment;
    public int Operand;
    public string CheckRegister;
    public string CheckOperator;
    public int CheckOperand;
}