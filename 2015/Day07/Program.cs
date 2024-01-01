// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day07\Sample.txt";
    string targetRegister = "f";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day07\Full.txt";
    string targetRegister = "a";
#endif

    string[] input = File.ReadAllLines(fileName);

    ushort part1 = Part1(input, targetRegister);
    Console.WriteLine("Part 1: " + part1);
    Console.WriteLine("Part 2: " + Part2(input, targetRegister, part1));
    Console.ReadLine();
}

ushort Part1(string[] input, string register)
{
    Network network = BuildNetwork(input);
    return network.GetValue(register);
}

ushort Part2(string[] input, string register, ushort newRegisterValue)
{
    Network network = BuildNetwork(input);
    network["b"] = new Command { ResultRegister = register, Operator = "", Operand1 = newRegisterValue.ToString() };
    
    return network.GetValue(register);
}

Network BuildNetwork(string[] input)
{
    Network network = new();

    foreach (string line in input)
    {
        Command command = ParseLine(line);
        network.Add(command.ResultRegister, command);

    }

    return network;
}

Command ParseLine(string line)
{
    Command command = new Command();
    
    var parts = line.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    command.ResultRegister = parts[1];

    command.Operator = Regex.Matches(parts[0], "[A-Z]+").FirstOrDefault()?.Value ?? "";

    var operands = parts[0].Split(command.Operator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    command.Operand1 = operands.First();
    command.Operand2 = operands.Last();

    return command;
}

class Network : Dictionary<string, Command>
{
    Dictionary<string, ushort> cache = new();

    public ushort GetValue(string register)
    {
        if(cache.TryGetValue(register, out var result))
        {
            return result;
        }


        if (!ushort.TryParse(register, out result))
        {

            Command command = this[register];

            switch (command.Operator)
            {
                case "":
                    result = GetValue(command.Operand1);
                    break;
                case "AND":
                    result = (ushort)(GetValue(command.Operand1) & GetValue(command.Operand2));
                    break;
                case "OR":
                    result = (ushort)(GetValue(command.Operand1) | GetValue(command.Operand2));
                    break;
                case "NOT":
                    result = (ushort)~GetValue(command.Operand1);
                    break;
                case "LSHIFT":
                    result = (ushort)(GetValue(command.Operand1) << GetValue(command.Operand2));
                    break;
                case "RSHIFT":
                    result = (ushort)(GetValue(command.Operand1) >> GetValue(command.Operand2));
                    break;
                default:
                    throw new ApplicationException("Unknown Operator");
            }
        }

        cache[register] = result;
        return result;
    }
}

record struct Command
{
    public string ResultRegister;
    public string Operator;
    public string Operand1;
    public string Operand2;
}
