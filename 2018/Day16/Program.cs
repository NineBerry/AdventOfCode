// #define Sample

using TestCase = (int[] Before, int[] After, int[] instruction);
using Opcode = System.Func<int, int, int, int[], int[]>;
using Instruction = int[];
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day16\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day16\Full.txt";
#endif

    string[] inputParts = File.ReadAllText(fileName).ReplaceLineEndings("\n").Split("\n\n\n");

    TestCase[] testcases = GetTestCases(inputParts[0]);
    Console.WriteLine("Part 1: " + Part1(testcases));

    Instruction[] instructions = GetInstructions(inputParts[1]);
    Console.WriteLine("Part 2: " + Part2(testcases, instructions));
    Console.ReadLine();
}

long Part1(TestCase[] testcases)
{
    return testcases.Where(test => Computer.GetCompatibleOpcodes(test).Count >= 3).Count();
}

int Part2((int[] Before, int[] After, int[] instruction)[] testcases, int[][] instructions)
{
    Dictionary<int, Opcode> opcodes = Computer.DeriveOpcodes(testcases);

    int[] registers = [0, 0, 0, 0];
    registers = Computer.ExecuteProgram(opcodes, instructions, registers);

    return registers[0];
}

TestCase[] GetTestCases(string input)
{
    return Regex
        .Matches(input, "Before: \\[(\\d+), (\\d+), (\\d+), (\\d+)]\n(\\d+) (\\d+) (\\d+) (\\d+)\nAfter:  \\[(\\d+), (\\d+), (\\d+), (\\d+)]")
        .Select(GetTestCase)
        .ToArray();
}

TestCase GetTestCase(Match match)
{
    return ([G(1), G(2), G(3), G(4)], [G(9), G(10), G(11), G(12)], [G(5), G(6), G(7), G(8)]);

    int G(int index)
    {
        return int.Parse(match.Groups[index].Value);
    }
}

Instruction[] GetInstructions(string input)
{
    return Regex
        .Matches(input, "(\\d+) (\\d+) (\\d+) (\\d+)")
        .Select(GetInstruction)
        .ToArray();
}

Instruction GetInstruction(Match match)
{
    return [G(1), G(2), G(3), G(4)];

    int G(int index)
    {
        return int.Parse(match.Groups[index].Value);
    }
}



public class Computer
{
    public static Dictionary<int, Opcode> DeriveOpcodes(TestCase[] testcases)
    {
        Dictionary<int, Opcode> opcodes = [];
        HashSet<Opcode> unassignedOpcodes = [.. Computer.AllOpcodes];

        while (unassignedOpcodes.Any())
        {

            foreach (int code in Enumerable.Range(0, Computer.AllOpcodes.Count))
            {
                if (opcodes.ContainsKey(code)) continue;

                HashSet<Opcode> compatible = [.. unassignedOpcodes];

                foreach (var testcase in testcases.Where(tc => tc.instruction[0] == code))
                {
                    compatible.IntersectWith(GetCompatibleOpcodes(testcase));
                }

                if (compatible.Count == 1)
                {
                    opcodes.Add(code, compatible.Single());
                    unassignedOpcodes.Remove(compatible.Single());

                }
            }
        }

        return opcodes;
    }

    public static HashSet<Opcode> GetCompatibleOpcodes(TestCase testcase)
    {
        List<Opcode> opcodes = [];

        foreach (var opcode in Computer.AllOpcodes)
        {
            int[] after = opcode(testcase.instruction[1], testcase.instruction[2], testcase.instruction[3], testcase.Before);

            if (Enumerable.SequenceEqual(after, testcase.After))
            {
                opcodes.Add(opcode);
            }
        }

        return opcodes.ToHashSet();
    }

    public static int[] ExecuteProgram(Dictionary<int, Opcode> opcodes, Instruction[] instructions, int[] registers)
    {
        foreach (var instruction in instructions)
        {
            registers = opcodes[instruction[0]](instruction[1], instruction[2], instruction[3], registers);
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
