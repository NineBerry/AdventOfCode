// #define Sample

using Opcode = System.Func<int, int, int, int[], int[]>;
using Instruction = (string Opcode, int A, int B, int C);
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day19\Sample.txt";
    int resultRegister = 5;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day19\Full.txt";
    int resultRegister = 0;
#endif

    string[] lines = File.ReadAllLines(fileName);

    int ipRegister = int.Parse(lines.First().Substring(4));
    Instruction[] program = lines.Skip(1).Select(ParseInstruction).ToArray();

    Console.WriteLine("Part 1: " + Part1(ipRegister, program, resultRegister));
    Console.WriteLine("Part 2: " + Part2(ipRegister, program, resultRegister));
    Console.ReadLine();
}

int Part1(int ipRegister, Instruction[] program, int resultRegister)
{
    Computer computer = new Computer(ipRegister);

    int[] registers = [0, 0, 0, 0, 0, 0];
    registers = computer.ExecuteProgram(program, registers);

    return registers[resultRegister];
}

int Part2(int ipRegister, Instruction[] program, int resultRegister)
{
    Computer computer = new Computer(ipRegister);

    int[] registers = [1, 0, 0, 0, 0, 0];
    registers = computer.ExecuteProgram(program, registers, 100);

    int bigNumber = registers[1];
    return SumOfFactors(bigNumber);
}

int SumOfFactors(int big)
{
    return Enumerable.Range(1, big).Where(x => big % x == 0).Sum();
}

Instruction ParseInstruction(string line)
{
    int[] ints = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    return (line.Substring(0, 4), ints[0], ints[1], ints[2]);
}

public class Computer
{
    public Computer(int ipRegister) 
    {
        IPRegister = ipRegister;

        Opcodes[nameof(addr)] = addr;
        Opcodes[nameof(addi)] = addi;
        Opcodes[nameof(mulr)] = mulr;
        Opcodes[nameof(muli)] = muli;
        Opcodes[nameof(banr)] = banr;
        Opcodes[nameof(bani)] = bani;
        Opcodes[nameof(borr)] = borr;
        Opcodes[nameof(bori)] = bori;
        Opcodes[nameof(setr)] = setr;
        Opcodes[nameof(seti)] = seti;
        Opcodes[nameof(gtir)] = gtir;
        Opcodes[nameof(gtri)] = gtri;
        Opcodes[nameof(gtrr)] = gtrr;
        Opcodes[nameof(eqir)] = eqir;
        Opcodes[nameof(eqri)] = eqri;
        Opcodes[nameof(eqrr)] = eqrr;
    }

    public int IPRegister = 0;
    Dictionary<string, Opcode> Opcodes = [];

    public int[] ExecuteProgram(Instruction[] program, int[] registers, int maxSteps = int.MaxValue)
    {
        int ip = 0;
        int steps = 0;

        while (ip >= 0 &&  ip < program.Length && steps < maxSteps)
        {
            steps++;
            Instruction instruction = program[ip];

            registers = Opcodes[instruction.Opcode](instruction.A, instruction.B, instruction.C, registers);
            registers[IPRegister] = registers[IPRegister] + 1;
            ip = registers[IPRegister];
        }

        return registers;
    }

    public static int[] addr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] + reg[B];

        return reg;
    }

    public static int[] addi(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] + B;

        return reg;
    }

    public static int[] mulr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] * reg[B];

        return reg;
    }

    public static int[] muli(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] * B;

        return reg;
    }

    public static int[] banr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] & reg[B];

        return reg;
    }

    public static int[] bani(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] & B;

        return reg;
    }

    public static int[] borr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] | reg[B];

        return reg;
    }

    public static int[] bori(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] | B;

        return reg;
    }

    public static int[] setr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A];

        return reg;
    }

    public static int[] seti(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = A;

        return reg;
    }

    public static int[] gtir(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = A > reg[B] ? 1 : 0;

        return reg;
    }

    public static int[] gtri(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] > B ? 1 : 0;

        return reg;
    }

    public static int[] gtrr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] > reg[B] ? 1 : 0;

        return reg;
    }

    public static int[] eqir(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = A == reg[B] ? 1 : 0;

        return reg;
    }

    public static int[] eqri(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] == B ? 1 : 0;

        return reg;
    }

    public static int[] eqrr(int A, int B, int C, int[] registersIn)
    {
        int[] reg = [.. registersIn];

        reg[C] = reg[A] == reg[B] ? 1 : 0;

        return reg;
    }

    public static readonly HashSet<Opcode> AllOpcodes = [addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr];
}
