// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day10\Full.txt";
#endif

    Instruction[] instructions = File.ReadAllLines(fileName).Select(Instruction.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(instructions));

#if !Sample
    Console.WriteLine("Part 2: " + Part2(instructions));
    Console.WriteLine("Part 3: " + Part3(instructions));
#endif

    Console.ReadLine();
}

// Possibly get this from the program? We see that a register is set to that value at the start
// We found this by looking at experimental output for different r0/r1 combinations.
const int magicNumber = 16;

long Part1(Instruction[] instructions)
{
    Computer computer = new Computer();
    computer.Execute(instructions);

    return computer.GetRegister(0);
}

long Part2(Instruction[] instructions)
{
    return CountLoopingInputs(99, 0, instructions, magicNumber);
}

/*
long OriginalPart2(Instruction[] instructions)
{
    List<int> loopingInputs = [];

    foreach (var i in Enumerable.Range(0, 100))
    {
        try
        {
            Computer computer = new Computer();
            computer.SetRegister(0, (ushort)i);
            computer.Execute(instructions, 5000000);
        }
        catch (Computer.MaxInstructionsExceededException)
        {
            loopingInputs.Add(i);
        }
        catch (Computer.InfiniteLoopException)
        {
            loopingInputs.Add(i);
        }
    }

    return loopingInputs.Count;
}
*/

long Part3(Instruction[] instructions)
{
    long sum = 0;

    foreach(var i in Enumerable.Range(0, 16))
    {
        Console.WriteLine("Checking r1=" + i);
        sum += CountLoopingInputs(65535, (ushort)i, instructions, magicNumber);
    }

    return sum;
}

long CountLoopingInputs(ushort r0Max, ushort r1, Instruction[] instructions, int magicNumber)
{
    var sequence = FindSequence(r1, instructions, magicNumber);

    if (sequence == null) return 0;

    long number = sequence.Value.StartValue;
    long count = 0;
    int sequenceOffset = 0;

    do
    {
        count++;

        number += sequence.Value.Sequence[sequenceOffset];
        
        sequenceOffset++;
        if (sequenceOffset >= sequence.Value.Sequence.Length) sequenceOffset = 0;

    } while (number <= r0Max);

    return count;
}

(ushort StartValue, ushort[] Sequence)? FindSequence(ushort r1, Instruction[] instructions, int magicNumber)
{
    List<ushort> loopingInputs = [];
    List<ushort> differences = [];

    foreach (var r0Input in Enumerable.Range(0, ushort.MaxValue))
    {
        if (r0Input > 100)
        {
            // When no loop after 100 tries, we expect this r1-input is loop-free
            return null;
        }

        bool looping = false;

        try
        {
            Computer computer = new Computer();
            computer.SetRegister(0, (ushort)r0Input);
            computer.SetRegister(1, r1);
            computer.Execute(instructions, 5_000_000);
        }
        catch (Computer.MaxInstructionsExceededException)
        {
            looping = true;
        }

        if (looping)
        {
            loopingInputs.Add((ushort)r0Input);

            if (loopingInputs.Count > 1)
            {
                ushort difference = (ushort)(loopingInputs[^1] - loopingInputs[^2]);
                differences.Add(difference);

                if (differences.Sum(d => d) == magicNumber)
                {
                    break;
                }

            }
        }
    }

    return (loopingInputs.First(), differences.ToArray());
}


class Computer
{
    public void Execute(Instruction[] instructions, int maxInstructions = int.MaxValue)
    {
        LoadInstructions(instructions);

        InstructionPointer = 0;
        int instructionsExecuted = 0;

        while (InstructionPointer < Instructions.Length)
        {
            Instruction currentInstruction = Instructions[InstructionPointer];
            InstructionPointer++;

            currentInstruction.Execute(this);

            instructionsExecuted++;
            if (instructionsExecuted > maxInstructions)
            {
                throw new MaxInstructionsExceededException();
            }
        }
    }

    private Dictionary<ushort, ushort> Registers { get; } = [];
    
    private Instruction[] Instructions { get; set; } = [];

    Dictionary<ushort, int> Labels = [];

    private int InstructionPointer { get; set; } = 0;

    public void SetRegister(ushort registerNumber, ushort value)
    {
        Registers[registerNumber] = value;
    }

    public ushort GetRegister(ushort registerNumber)
    {
        Registers.TryGetValue(registerNumber, out ushort value);
        return value;
    }

    private HashSet<string> KnownStates = [];

    public void JumpToLabel(ushort labelId)
    {
        InstructionPointer = Labels[labelId];
    }

    private void LoadInstructions(Instruction[] instructions)
    {
        Labels.Clear();
        List<Instruction> realInstructions = [];

        foreach (var instruction in instructions)
        {
            if(instruction is Label label)
            {
                Labels.Add(label.LabelID, realInstructions.Count);
            }
            else
            {
                realInstructions.Add(instruction);
            }
        }
        
        Instructions = realInstructions.ToArray();
    }

    public class MaxInstructionsExceededException : Exception;
}

abstract class Instruction
{
    public abstract void Execute(Computer computer);

    public static Instruction Parse(string line)
    {
        List<ushort> numbers = [];

        if (line.StartsWith("ba"))
        {
            ushort instructionType = ReadNumber("ba");

            return instructionType switch
            {
                0 => new LoadInstructions { Value = ReadNumber(), DestinationRegister = ReadNumber()},
                1 => new CopyInstruction { SourceRegister = ReadNumber(), DestinationRegister = ReadNumber() },
                2 => new AddInstruction { SourceRegister1 = ReadNumber(), SourceRegister2 = ReadNumber(), DestinationRegister = ReadNumber() },
                3 => new SubInstruction { SourceRegister1 = ReadNumber(), SourceRegister2 = ReadNumber(), DestinationRegister = ReadNumber() },
                4 => new MulInstruction { SourceRegister1 = ReadNumber(), SourceRegister2 = ReadNumber(), DestinationRegister = ReadNumber() },
                5 => new ModInstruction { SourceRegister1 = ReadNumber(), SourceRegister2 = ReadNumber(), DestinationRegister = ReadNumber() },
                6 => new IncInstruction { DestinationRegister = ReadNumber() },
                7 => new DecInstruction { DestinationRegister = ReadNumber() },
                8 => new JumpInstruction { Label = ReadNumber() },
                9 => new JumpIfZeroInstruction { Register = ReadNumber(), Label = ReadNumber()},
                10 => new JumpIfNotZeroInstruction { Register = ReadNumber(), Label = ReadNumber() },

                _ => throw new Exception("Unknown instruction type")
            };
        }
        else if (line.StartsWith("be"))
        {
            return new Label { LabelID = ReadNumber("be") };
        }
        else throw new Exception("Unknown prefix");


        ushort ReadNumber(string separator = "ne")
        {
            if (line.StartsWith(separator))
            {
                line = line.Substring(separator.Length);
            }
            else throw new Exception($"Expected {separator} but not found");
            
            ushort counter = 0;

            while(line.Any() && line.StartsWith("na"))
            {
                counter++;
                line = line.Substring("na".Length);
            }

            return counter; 
        }

    }
}

class Label : Instruction
{
    public ushort LabelID { get; set; }
    public override void Execute(Computer computer) 
    {
    }
}

class LoadInstructions : Instruction
{
    public ushort Value { get; init; }
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer) 
    { 
        computer.SetRegister(DestinationRegister, Value);
    }
}

class CopyInstruction : Instruction
{
    public ushort SourceRegister { get; init; }
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort value = computer.GetRegister(SourceRegister);
        computer.SetRegister(DestinationRegister, value);
    }
}

class AddInstruction : Instruction
{
    public ushort SourceRegister1 { get; init; }
    public ushort SourceRegister2 { get; init; }
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort val1 = computer.GetRegister(SourceRegister1);
        ushort val2 = computer.GetRegister(SourceRegister2);
        ushort sum = (ushort)(val1 + val2);
        computer.SetRegister(DestinationRegister, sum);
    }
}

class SubInstruction : Instruction
{
    public ushort SourceRegister1 { get; init; }
    public ushort SourceRegister2 { get; init; }
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort val1 = computer.GetRegister(SourceRegister1);
        ushort val2 = computer.GetRegister(SourceRegister2);
        ushort res = (ushort)(val1 - val2);

        computer.SetRegister(DestinationRegister, res);
    }
}

class MulInstruction : Instruction
{
    public ushort SourceRegister1 { get; init; }
    public ushort SourceRegister2 { get; init; }
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort val1 = computer.GetRegister(SourceRegister1);
        ushort val2 = computer.GetRegister(SourceRegister2);
        ushort res = (ushort)(val1 * val2);

        computer.SetRegister(DestinationRegister, res);
    }
}

class ModInstruction : Instruction
{
    public ushort SourceRegister1 { get; init; }
    public ushort SourceRegister2 { get; init; }
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort val1 = computer.GetRegister(SourceRegister1);
        ushort val2 = computer.GetRegister(SourceRegister2);
        ushort res = (val2 == 0) ? (ushort)0 : (ushort)(val1 % val2);

        computer.SetRegister(DestinationRegister, res);
    }
}

class IncInstruction : Instruction
{
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort val = computer.GetRegister(DestinationRegister);
        ushort res = (ushort)(val + 1);
        computer.SetRegister(DestinationRegister, res);
    }
}

class DecInstruction : Instruction
{
    public ushort DestinationRegister { get; init; }

    public override void Execute(Computer computer)
    {
        ushort val = computer.GetRegister(DestinationRegister);
        ushort res = (ushort)(val - 1);
        computer.SetRegister(DestinationRegister, res);
    }
}

class JumpInstruction : Instruction
{
    public ushort Label { get; init; }

    public override void Execute(Computer computer)
    {
        computer.JumpToLabel(Label);
    }
}

class JumpIfZeroInstruction : Instruction
{
    public ushort Register { get; init; }
    public ushort Label { get; init; }

    public override void Execute(Computer computer)
    {
        if(computer.GetRegister(Register) == 0)
            computer.JumpToLabel(Label);
    }
}

class JumpIfNotZeroInstruction : Instruction
{
    public ushort Register { get; init; }
    public ushort Label { get; init; }

    public override void Execute(Computer computer)
    {
        if (computer.GetRegister(Register) != 0)
            computer.JumpToLabel(Label);
    }
}

